// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Linq;

using DotNetListRefs.Models;
using Microsoft.Extensions.Logging;
using NuGet.Frameworks;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace DotNetListRefs
{
    public class VersionChecker : IVersionChecker
    {
        private readonly ILogger logger;

        public VersionChecker(ILogger<VersionChecker> logger)
        {
            this.logger = logger;
        }


        public void Check(RefGraph graph)
        {
            foreach (var projectNode in graph.Nodes.OfType<ProjectNode>())
            {
                foreach (var packageEdge in projectNode.OutEdges.OfType<PackageReferenceEdge>())
                {
                    var packageNode = (PackageNode)packageEdge.ToNode;

                    foreach (var versionSpec in packageEdge.Versions)
                    {
                        var specifiedVersion = versionSpec.Key;

                        foreach (var targetFramework in versionSpec.Value)
                        {
                            var entry = new VersionEntry
                            {
                                PackageName = packageNode.Name,
                                TargetFramework = targetFramework,
                                SpecifiedVersion = specifiedVersion
                            };

                            projectNode.Versions.Add(entry);

                            PopulateVersions(entry, packageNode.PackageMetadata);
                        }
                    }
                }
            }
        }


        private void PopulateVersions(VersionEntry entry, System.Collections.Generic.IEnumerable<IPackageSearchMetadata> packageMetadata)
        {
            // Filter out versions that do not apply to the target framework.
            // TODO - skip this check for development dependencies?
            var reducer = new FrameworkReducer();
            var framework = NuGetFramework.Parse(entry.TargetFramework);

            var depSets = packageMetadata
                .Select(x => x.DependencySets)
                .Where(x => x != null)
                .SelectMany(x => x)
                .Select(x => x.TargetFramework.ToString())
                .Distinct();

            packageMetadata = packageMetadata
                .Where(x => (x.DependencySets == null)
                        || !x.DependencySets.Any()
                        || reducer.GetNearest(framework, x.DependencySets.Select(ds => ds.TargetFramework)) != null);

            var allVersions = packageMetadata
                .Select(x => x.Identity.Version)
                .ToList();

            if (allVersions.Count == 0)
            {
                logger.LogWarning("Found 0 versions for package {PackageName} after target-framework filter", entry.PackageName);
            }

            // Set up a few things for version-finding
            // TODO - should the package node be storing NuGetVersion instead of string?
            var referencedVersion = new NuGetVersion(entry.SpecifiedVersion);

            // TODO - the project scan should be providing a version range, we shouldn't have to construct it here
            var currentVersionRange = new VersionRange(referencedVersion);

            // Find the latest pre-release version
            var latestVersionRange = new VersionRange(currentVersionRange, new FloatRange(NuGetVersionFloatBehavior.AbsoluteLatest, referencedVersion));
            var latestVersion = latestVersionRange.FindBestMatch(allVersions);

            if (latestVersion.IsPrerelease)
            {
                entry.LatestPrerelease = latestVersion?.ToString();
            }

            // Find the latest patch version
            latestVersionRange = new VersionRange(currentVersionRange, new FloatRange(NuGetVersionFloatBehavior.Patch, referencedVersion));
            latestVersion = latestVersionRange.FindBestMatch(allVersions);

            if (!latestVersion.IsPrerelease)
            {
                entry.LatestPatchUpdate = latestVersion?.ToString();
            }

            // Find the latest minor version
            latestVersionRange = new VersionRange(currentVersionRange, new FloatRange(NuGetVersionFloatBehavior.Minor, referencedVersion));
            latestVersion = latestVersionRange.FindBestMatch(allVersions);

            if (!latestVersion.IsPrerelease)
            {
                entry.LatestMinorUpdate = latestVersion?.ToString();
            }

            // Find the latest major version
            latestVersionRange = new VersionRange(currentVersionRange, new FloatRange(NuGetVersionFloatBehavior.Major, referencedVersion));
            latestVersion = latestVersionRange.FindBestMatch(allVersions);

            if (!latestVersion.IsPrerelease)
            {
                entry.LatestMajorUpdate = latestVersion?.ToString();
            }

            // Set the outdated flag
            // TODO - add CLI options to prevent flagging as outdated if the version is pre-release, or patch, or minor or major
            if (!string.IsNullOrEmpty(entry.LatestPrerelease) && (entry.SpecifiedVersion != entry.LatestPrerelease))
            {
                entry.IsOutdated = true;
            }
            else if (!string.IsNullOrEmpty(entry.LatestPatchUpdate) && (entry.SpecifiedVersion != entry.LatestPatchUpdate))
            {
                entry.IsOutdated = true;
            }
            else if (!string.IsNullOrEmpty(entry.LatestMinorUpdate) && (entry.SpecifiedVersion != entry.LatestMinorUpdate))
            {
                entry.IsOutdated = true;
            }
            else if (!string.IsNullOrEmpty(entry.LatestMajorUpdate) && (entry.SpecifiedVersion != entry.LatestMajorUpdate))
            {
                entry.IsOutdated = true;
            }
            else
            {
                entry.IsOutdated = false;
            }
        }
    }
}
