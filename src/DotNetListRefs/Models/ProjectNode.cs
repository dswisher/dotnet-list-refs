// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.IO;

namespace DotNetListRefs.Models
{
    public class ProjectNode : Node
    {
        public ProjectNode(string path)
            : base(Path.GetFileName(path))
        {
            ProjectPath = path;

            Versions = new List<VersionEntry>();
        }


        public override string NodeType { get { return "Project"; } }

        public string ProjectPath { get; private set; }
        public string ProjectAssetsFile { get; set; }

        public List<VersionEntry> Versions { get; private set; }


        protected override void PopulateProperties(Dictionary<string, string> props)
        {
            props.Add("Path", ProjectPath);
            props.Add("Assets File", ProjectAssetsFile);
        }
    }
}
