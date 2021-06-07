// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;

using DotNetListRefs.Models;
using Microsoft.Extensions.Logging;

namespace DotNetListRefs.Writers
{
    public class DotGraphWriter
    {
        private readonly ILogger logger;

        public DotGraphWriter(ILogger<DotGraphWriter> logger)
        {
            this.logger = logger;
        }


        public void Write(RefGraph graph, string path)
        {
            using (var writer = new StreamWriter(path))
            {
                // Start it
                writer.WriteLine("digraph G {");
                writer.WriteLine("   node [fontsize=10, shape=box];");
                writer.WriteLine("   edge [fontsize=8];");
                writer.WriteLine("   rankdir=LR;");
                writer.WriteLine();

                // Write all the edges first. Note that we just do out-edges here, as we only want one copy
                // of each edge.
                foreach (var node in graph.Nodes)
                {
                    foreach (var edge in node.OutEdges)
                    {
                        writer.WriteLine("   {0} -> {1} [label=\"{2}\"];", edge.FromNode.Id, edge.ToNode.Id, edge.EdgeType);
                    }
                }

                writer.WriteLine();

                // Write all the nodes
                foreach (var node in graph.Nodes)
                {
                    writer.WriteLine("   {0} [label=\"{1}\"];", node.Id, node.Name);
                }

                // Done!
                writer.WriteLine("}");
            }

            logger.LogInformation("Graph written to {Path}.", path);
        }
    }
}
