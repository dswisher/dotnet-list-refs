// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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

                foreach (var item in results)
                {
                    logger.LogInformation("   Target Framework: {Framework}", item.TargetFramework);

                    logger.LogInformation("      Project Assets File: {Path}", item.GetProperty("ProjectAssetsFile"));

                    foreach (var projRef in item.ProjectReferences)
                    {
                        logger.LogInformation("      Project Reference: {ProjRef}", projRef);
                    }

                    foreach (var projRef in item.References)
                    {
                        // logger.LogInformation("      Reference: {Ref}", projRef);
                    }

                    foreach (var packRef in item.PackageReferences)
                    {
                        // TODO - what about all the dictionary stuff?
                        logger.LogInformation("      Package Ref: {Ref}", packRef.Key);

                        foreach (var pair in packRef.Value)
                        {
                            logger.LogInformation("         {Key} -> {Value}", pair.Key, pair.Value);
                        }
                    }
                }
            }
        }
    }
}
