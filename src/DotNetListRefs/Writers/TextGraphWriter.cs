// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;

using DotNetListRefs.Models;
using Microsoft.Extensions.Logging;

namespace DotNetListRefs.Writers
{
    public class TextGraphWriter
    {
        private readonly ILogger logger;

        public TextGraphWriter(ILogger<TextGraphWriter> logger)
        {
            this.logger = logger;
        }


        public void Write(RefGraph graph, string path)
        {
            using (var writer = new StreamWriter(path))
            {
                var first = true;

                foreach (var node in graph.Nodes)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        writer.WriteLine();
                        writer.WriteLine("---------------------------------------------");
                        writer.WriteLine();
                    }

                    WriteNode(writer, node);
                }
            }

            logger.LogInformation("Graph written to {Path}.", path);
        }


        private void WriteNode(TextWriter writer, Node node)
        {
            // Write the "banner" for this node
            writer.WriteLine($"[{node.Id}] {node.NodeType} {node.Name}");

            // Write any properties for this node
            var props = node.GetProperties();

            if (props.Any())
            {
                var titleLen = props.Keys.Select(x => x.Length).Max();

                writer.WriteLine("   PROPERTIES");
                foreach (var pair in props)
                {
                    var title = $"{pair.Key}:".PadRight(titleLen + 1);

                    writer.WriteLine($"      {title} {pair.Value}");
                }
            }

            // Write any incoming edges
            WriteEdges(writer, "INCOMING EDGES", node.InEdges);

            // Write any outgoing edges
            WriteEdges(writer, "OUTGOING EDGES", node.OutEdges);
        }


        private void WriteEdges(TextWriter writer, string title, List<Edge> edges)
        {
            if (!edges.Any())
            {
                return;
            }

            var typeLen = edges.Select(x => x.EdgeType.Length).Max();

            writer.WriteLine();
            writer.WriteLine($"   {title}");

            foreach (var edge in edges)
            {
                var edgeType = $"{edge.EdgeType}:".PadRight(typeLen + 1);

                writer.WriteLine($"      {edgeType} [{edge.FromNode.Id}] {edge.FromNode.NodeType} {edge.FromNode.Name} -> [{edge.ToNode.Id}] {edge.ToNode.NodeType} {edge.ToNode.Name}");

                var props = edge.GetProperties();
                if (props.Any())
                {
                    var keyLen = props.Keys.Select(x => x.Length).Max();

                    foreach (var pair in props)
                    {
                        var key = $"{pair.Key}:".PadRight(keyLen + 1);

                        writer.WriteLine($"         {key} {pair.Value}");
                    }
                }
            }
        }
    }
}
