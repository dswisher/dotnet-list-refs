// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;

namespace DotNetListRefs.Models
{
    public class SolutionNode : Node
    {
        public SolutionNode(string path)
            : base(Path.GetFileName(path))
        {
            SolutionPath = path;
        }


        public string SolutionPath { get; private set; }


        public override void Write(StreamWriter writer)
        {
            writer.WriteLine("[{0}] Solution", Id);
            writer.WriteLine("   Path: {0}", SolutionPath);
        }
    }
}
