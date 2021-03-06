// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
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


        public override string NodeType { get { return "Solution"; } }

        public string SolutionPath { get; private set; }


        protected override void PopulateProperties(Dictionary<string, string> props)
        {
            props.Add("Path", SolutionPath);
        }
    }
}
