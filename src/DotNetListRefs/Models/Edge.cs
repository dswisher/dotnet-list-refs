// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace DotNetListRefs.Models
{
    public abstract class Edge
    {
        protected Edge(Node fromNode, Node toNode)
        {
            FromNode = fromNode;
            ToNode = toNode;
        }


        public Node FromNode { get; private set; }
        public Node ToNode { get; private set; }
    }
}
