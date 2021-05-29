// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Linq;

using Buildalyzer;
using DotNetListRefs.Models;
using Microsoft.Extensions.Logging;

namespace DotNetListRefs
{
    public class BuildalyzerSolutionProcessor : ISolutionProcessor
    {
        private readonly ILogger logger;

        public BuildalyzerSolutionProcessor(ILogger<BuildalyzerSolutionProcessor> logger)
        {
            this.logger = logger;
        }


        public void AnalyzeSolutions(RefGraph graph)
        {
            var numSolutions = 0;
            var numProjects = 0;

            foreach (var solutionNode in graph.Nodes.OfType<SolutionNode>().ToList())
            {
                numSolutions += 1;

                var manager = new AnalyzerManager(solutionNode.SolutionPath);

                foreach (var proj in manager.Projects)
                {
                    // Add the project node
                    var projNode = new ProjectNode(proj.Key);

                    graph.AddNode(projNode);

                    numProjects += 1;

                    // Add an edge between the solution node and the project node
                    // TODO
                }
            }

            logger.LogInformation("Analyzed {NumSolutions} solutions and added {NumProjects} projects to the graph.", numSolutions, numProjects);
        }
    }
}
