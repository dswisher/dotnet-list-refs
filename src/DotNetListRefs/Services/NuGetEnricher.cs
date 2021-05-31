// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DotNetListRefs.Models;
using Microsoft.Extensions.Logging;
using NuGet.Configuration;
using NuGet.Protocol.Core.Types;

namespace DotNetListRefs.Services
{
    public sealed class NuGetEnricher : INuGetEnricher, IDisposable
    {
        private readonly ILogger logger;
        private readonly SourceCacheContext cacheContext;
        private readonly ConcurrentDictionary<string, PackageMetadataResource> resourceCache = new ConcurrentDictionary<string, PackageMetadataResource>();
        private readonly ConcurrentDictionary<string, IEnumerable<IPackageSearchMetadata>> metaCache = new ConcurrentDictionary<string, IEnumerable<IPackageSearchMetadata>>();

        private int metaCacheHits;
        private int metaCacheMisses;

        public NuGetEnricher(ILogger<NuGetEnricher> logger)
        {
            this.logger = logger;

            cacheContext = new SourceCacheContext()
            {
                NoCache = true
            };
        }


        public async Task EnrichAsync(RefGraph graph, CancellationToken cancellationToken)
        {
            var projectTasks = graph.Nodes
                .OfType<ProjectNode>()
                .Select(x => EnrichProjectAsync(x, cancellationToken))
                .ToArray();

            logger.LogInformation("Enriching package info from NuGet...");

            await Task.WhenAll(projectTasks);

            logger.LogInformation("NuGet enrichment complete, {Hits} cache hits, {Misses} cache misses.", metaCacheHits, metaCacheMisses);
        }


        public void Dispose()
        {
            cacheContext?.Dispose();
        }


        private async Task EnrichProjectAsync(ProjectNode projectNode, CancellationToken cancellationToken)
        {
            var projectPath = projectNode.ProjectPath;

            var settings = Settings.LoadDefaultSettings(projectPath);
            var enabledSources = SettingsUtility.GetEnabledSources(settings);

            var fetchTasks = new List<Task>();

            foreach (var source in enabledSources)
            {
                // Get the metadata resource for this source
                PackageMetadataResource sourceMeta;

                if (!resourceCache.TryGetValue(source.Name, out sourceMeta))
                {
                    var sourceRepo = new SourceRepository(source, Repository.Provider.GetCoreV3());
                    sourceMeta = await sourceRepo.GetResourceAsync<PackageMetadataResource>(cancellationToken);

                    resourceCache.TryAdd(source.Name, sourceMeta);
                }

                // Go through all the package references from this project
                foreach (var packageEdge in projectNode.OutEdges.OfType<PackageReferenceEdge>())
                {
                    var packageNode = (PackageNode)packageEdge.ToNode;
                    var packageKey = $"{source.Name}-{packageNode.Name}";

                    IEnumerable<IPackageSearchMetadata> packageMeta;

                    if (metaCache.TryGetValue(packageKey, out packageMeta))
                    {
                        packageNode.PackageMetadata = packageMeta;

                        Interlocked.Increment(ref metaCacheHits);
                    }
                    else
                    {
                        fetchTasks.Add(FetchPackageMetadataAsync(sourceMeta, source.Name, packageKey, packageNode, cancellationToken));

                        Interlocked.Increment(ref metaCacheMisses);
                    }
                }
            }

            await Task.WhenAll(fetchTasks);
        }


        private async Task FetchPackageMetadataAsync(PackageMetadataResource sourceMeta, string sourceName,
                string packageKey, PackageNode packageNode, CancellationToken cancellationToken)
        {
            logger.LogDebug("...fetching metadata for package {PackageName} from {SourceName}...", packageNode.Name, sourceName);

            // TODO - how to pass our logger as a NuGet logger?
            var packageMetadata = await sourceMeta.GetMetadataAsync(packageNode.Name, true, false, cacheContext, NuGet.Common.NullLogger.Instance, cancellationToken);
            var metaList = packageMetadata.ToList();

            logger.LogDebug("...received metadata for package {PackageName}, {NumItems} items from {SourceName}.",
                    packageNode.Name, metaList.Count, sourceName);

            metaCache.TryAdd(packageKey, metaList);

            if ((metaList.Count > 0) || (packageNode.PackageMetadata == null))
            {
                packageNode.PackageMetadata = metaList;
            }
        }
    }
}
