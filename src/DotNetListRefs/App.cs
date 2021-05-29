// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

using DotNetListRefs.Models;
using DotNetListRefs.Services;
using DotNetListRefs.Writers;

namespace DotNetListRefs
{
    public class App
    {
        private readonly IProjectDiscoveryService projectDiscoveryService;
        private readonly ISolutionProcessor solutionProcessor;
        private readonly IProjectProcessor projectProcessor;
        private readonly IGraphWriter graphWriter;

        public App(IProjectDiscoveryService projectDiscoveryService,
                   ISolutionProcessor solutionProcessor,
                   IProjectProcessor projectProcessor,
                   IGraphWriter graphWriter)
        {
            this.projectDiscoveryService = projectDiscoveryService;
            this.solutionProcessor = solutionProcessor;
            this.projectProcessor = projectProcessor;
            this.graphWriter = graphWriter;
        }


        public async Task RunAsync(Options options, CancellationToken cancellationToken)
        {
            // Create the graph that will be populated.
            var graph = new RefGraph();

            // Get the list of project paths that will be the starting point
            projectDiscoveryService.DiscoverProjects(options.Path, graph);

            // Process any solution nodes
            solutionProcessor.AnalyzeSolutions(graph);

            // Process any project nodes
            projectProcessor.AnalyzeProjects(graph);

            // TODO - pull in transitive dependencies

            // Enrich with info from nuget
            // TODO

            // If desired, dump the graph
            if (!string.IsNullOrEmpty(options.GraphFile))
            {
                graphWriter.Write(graph, options.GraphFile);
            }

            // TODO - implement me!
            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }
}
