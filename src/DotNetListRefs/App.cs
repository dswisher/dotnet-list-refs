// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;

using DotNetListRefs.Services;

namespace DotNetListRefs
{
    public class App
    {
        private readonly IProjectDiscoveryService projectDiscoveryService;

        public App(IProjectDiscoveryService projectDiscoveryService)
        {
            this.projectDiscoveryService = projectDiscoveryService;
        }


        public async Task RunAsync(Options options, CancellationToken cancellationToken)
        {
            // Get the list of project paths that will be the starting point
            var projectPaths = projectDiscoveryService.DiscoverProjects(options.Path, options.Recursive);

            // TODO - for the moment, just dump out the projects.
            foreach (var proj in projectPaths)
            {
                Console.WriteLine("Artifact: {0}", proj);
            }

            // TODO - implement me!
            await Task.Delay(TimeSpan.FromSeconds(2));
        }
    }
}
