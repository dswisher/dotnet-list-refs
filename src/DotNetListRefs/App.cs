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
        private readonly IGraphWriter graphWriter;

        public App(IProjectDiscoveryService projectDiscoveryService,
                   IGraphWriter graphWriter)
        {
            this.projectDiscoveryService = projectDiscoveryService;
            this.graphWriter = graphWriter;
        }


        public async Task RunAsync(Options options, CancellationToken cancellationToken)
        {
            // Create the graph that will be populated.
            var graph = new RefGraph();

            // Get the list of project paths that will be the starting point
            projectDiscoveryService.DiscoverProjects(options.Path, graph);

            // Visit all the project/solution nodes, and expand them.
            // TODO

            // TODO - add more stuff (nuget, etc) to the graph

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
