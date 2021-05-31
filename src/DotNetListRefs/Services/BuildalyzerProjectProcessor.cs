// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;

using Buildalyzer;
using DotNetListRefs.Models;
using Microsoft.Extensions.Logging;

namespace DotNetListRefs
{
    public class BuildalyzerProjectProcessor : IProjectProcessor
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger logger;

        public BuildalyzerProjectProcessor(ILoggerFactory loggerFactory,
                                           ILogger<BuildalyzerProjectProcessor> logger)
        {
            this.loggerFactory = loggerFactory;
            this.logger = logger;
        }


        public void AnalyzeProjects(RefGraph graph)
        {
            foreach (var projectNode in graph.Nodes.OfType<ProjectNode>().ToList())
            {
                logger.LogInformation("Analyzing project {Name}...", projectNode.Name);

                var opts = new AnalyzerManagerOptions
                {
                    // LoggerFactory = loggerFactory
                };

                var manager = new AnalyzerManager(opts);
                var analyzer = manager.GetProject(projectNode.ProjectPath);

                var results = analyzer.Build();

                foreach (var result in results)
                {
                    // The assets file _should_ be the same, regardless of the framework, as it contains info on
                    // each framework build. Verify that fact.
                    var projectAssetsFile = result.GetProperty("ProjectAssetsFile");
                    if (!string.IsNullOrEmpty(projectNode.ProjectAssetsFile)
                            && !string.IsNullOrEmpty(projectAssetsFile)
                            && projectNode.ProjectAssetsFile != projectAssetsFile)
                    {
                        // TODO - throw?
                        logger.LogWarning("Inconsistent project assets file: already have {Old}, found {New}!", projectNode.ProjectAssetsFile, projectAssetsFile);
                    }

                    projectNode.ProjectAssetsFile = projectAssetsFile;

                    // Create nodes and links for each project reference
                    foreach (var projRef in result.ProjectReferences)
                    {
                        AddProjectReference(graph, projectNode, result.TargetFramework, projRef);
                    }

                    // Create nodes and links for each package reference
                    foreach (var packRef in result.PackageReferences)
                    {
                        // TODO - add package references -framework-specific!
                        AddPackageReference(graph, projectNode, result.TargetFramework, packRef.Key, packRef.Value);
                    }
                }
            }
        }


        private void AddPackageReference(RefGraph graph, ProjectNode projectNode, string targetFramework, string packageName, IReadOnlyDictionary<string, string> packageProps)
        {
            // Grab the version out of the properties. If we can't find a version, ignore this package.
            var version = packageProps
                .Where(x => x.Key == "Version")
                .Select(x => x.Value)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(version))
            {
                logger.LogWarning("Package {PackageName} lacks a version, ignoring.", packageName);
                return;
            }

            // Look to see if this package already exists in the graph.
            var packageNode = graph.Nodes
                .OfType<PackageNode>()
                .Where(x => x.Name == packageName)
                .FirstOrDefault();

            if (packageNode == null)
            {
                packageNode = new PackageNode(packageName);
                graph.AddNode(packageNode);
            }

            // Look to see if there is already a link between the nodes. If not, create one.
            var edge = projectNode.OutEdges
                .Where(x => x.ToNode == packageNode)
                .Cast<PackageReferenceEdge>()
                .FirstOrDefault();

            if (edge == null)
            {
                // Edge does not exist - create it
                edge = new PackageReferenceEdge(projectNode, packageNode);

                graph.AddEdge(edge);
            }

            edge.AddVersion(version, targetFramework);
        }


        private void AddProjectReference(RefGraph graph, ProjectNode projectNode, string targetFramework, string projRef)
        {
            // Look to see if this project already exists in the graph.
            var refProjectNode = graph.Nodes
                .OfType<ProjectNode>()
                .Where(x => x.ProjectPath == projRef)
                .FirstOrDefault();

            if (refProjectNode == null)
            {
                // TODO - the referenced project does not exist in the graph - should we analyze it, too?
                refProjectNode = new ProjectNode(projRef);
                graph.AddNode(refProjectNode);
            }

            // Look to see if we already have a link between these two nodes. If not, create one.
            var edge = projectNode.OutEdges
                .Where(x => x.ToNode == refProjectNode)
                .Cast<ProjectReferenceEdge>()
                .FirstOrDefault();

            if (edge == null)
            {
                // Edge does not exist - create it
                edge = new ProjectReferenceEdge(projectNode, refProjectNode);

                graph.AddEdge(edge);
            }

            edge.TargetFrameworks.Add(targetFramework);
        }
    }
}
