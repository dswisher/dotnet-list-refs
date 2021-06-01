// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

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
        private readonly INuGetEnricher nuGetEnricher;
        private readonly IVersionChecker versionChecker;
        private readonly OutdatedWriter outdatedWriter;
        private readonly TextGraphWriter textGraphWriter;

        public App(IProjectDiscoveryService projectDiscoveryService,
                   ISolutionProcessor solutionProcessor,
                   IProjectProcessor projectProcessor,
                   INuGetEnricher nuGetEnricher,
                   IVersionChecker versionChecker,
                   OutdatedWriter outdatedWriter,
                   TextGraphWriter graphWriter)
        {
            this.projectDiscoveryService = projectDiscoveryService;
            this.solutionProcessor = solutionProcessor;
            this.projectProcessor = projectProcessor;
            this.nuGetEnricher = nuGetEnricher;
            this.versionChecker = versionChecker;
            this.outdatedWriter = outdatedWriter;
            this.textGraphWriter = graphWriter;
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
            await nuGetEnricher.EnrichAsync(graph, cancellationToken);

            // Go through and populate the package versions for each project
            versionChecker.Check(graph);

            // Unless they have requested otherwise, show the list of outdated packages
            if (!options.HideOutdated)
            {
                outdatedWriter.Write(graph);
            }

            // If requested, write the graph to a text file
            if (!string.IsNullOrEmpty(options.TextOutputPath))
            {
                textGraphWriter.Write(graph, options.TextOutputPath);
            }

            // If requested, write the graph to a JSON file
            // TODO - write JSON

            // If requested, write the graph to a DOT file (GraphViz)
            // TODO - write DOT
        }
    }
}
