// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace DotNetListRefs.Models
{
    public abstract class Node
    {
        private static int nextId = 1;

        protected Node(string name)
        {
            Name = name;
            Id = nextId++;

            InEdges = new List<Edge>();
            OutEdges = new List<Edge>();
        }


        public int Id { get; private set; }
        public string Name { get; private set; }

        public List<Edge> InEdges { get; private set; }
        public List<Edge> OutEdges { get; private set; }

        public abstract string NodeType { get; }


        public void AddInEdge(Edge edge)
        {
            InEdges.Add(edge);
        }


        public void AddOutEdge(Edge edge)
        {
            OutEdges.Add(edge);
        }


        public IDictionary<string, string> GetProperties()
        {
            var props = new Dictionary<string, string>();

            PopulateProperties(props);

            return props;
        }


        protected abstract void PopulateProperties(Dictionary<string, string> props);
    }
}
