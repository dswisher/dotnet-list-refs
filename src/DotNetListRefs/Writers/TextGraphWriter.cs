// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;

using DotNetListRefs.Models;

namespace DotNetListRefs.Writers
{
    public class TextGraphWriter : IGraphWriter
    {
        public void Write(RefGraph graph, string path)
        {
            using (var writer = new StreamWriter(path))
            {
                foreach (var node in graph.Nodes)
                {
                    node.Write(writer);
                }
            }

            Console.WriteLine("Graph written to {0}.", path);
        }
    }
}
