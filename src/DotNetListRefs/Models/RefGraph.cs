// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace DotNetListRefs.Models
{
    public class RefGraph
    {
        private readonly List<Node> nodes = new List<Node>();
        private readonly List<Edge> edges = new List<Edge>();


        public IEnumerable<Node> Nodes { get { return nodes; } }
        public IEnumerable<Edge> Edges { get { return edges; } }


        public void AddNode(Node node)
        {
            nodes.Add(node);
        }


        public void AddEdge(Edge edge)
        {
            edges.Add(edge);

            edge.FromNode.AddOutEdge(edge);
            edge.ToNode.AddInEdge(edge);
        }
    }
}
