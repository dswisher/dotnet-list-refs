// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace DotNetListRefs.Models
{
    public class RefGraph
    {
        private readonly List<Node> nodes = new List<Node>();


        public IEnumerable<Node> Nodes { get { return nodes; } }


        public void AddNode(Node node)
        {
            nodes.Add(node);
        }
    }
}
