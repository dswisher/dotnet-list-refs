// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;

namespace DotNetListRefs.Models
{
    public class ProjectNode : Node
    {
        public ProjectNode(string path)
            : base(System.IO.Path.GetFileName(path))
        {
            ProjectPath = path;
        }


        public string ProjectPath { get; private set; }


        public override void Write(StreamWriter writer)
        {
            writer.WriteLine("[{0}] Project", Id);
            writer.WriteLine("   Path: {0}", ProjectPath);
        }
    }
}
