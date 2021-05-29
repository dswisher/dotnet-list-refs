// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;

using DotNetListRefs.Models;
using Microsoft.Extensions.Logging;

namespace DotNetListRefs.Writers
{
    public class TextGraphWriter : IGraphWriter
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
                    }

                    node.Write(writer);
                }
            }

            logger.LogInformation("Graph written to {Path}.", path);
        }
    }
}
