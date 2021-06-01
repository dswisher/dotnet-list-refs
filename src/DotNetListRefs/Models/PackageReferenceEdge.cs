// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace DotNetListRefs.Models
{
    public class PackageReferenceEdge : Edge
    {
        private readonly Dictionary<string, HashSet<string>> versions = new Dictionary<string, HashSet<string>>();

        public PackageReferenceEdge(PackageNode fromPackage, PackageNode toPackage)
            : this((Node)fromPackage, (Node)toPackage)
        {
        }


        public PackageReferenceEdge(ProjectNode fromProject, PackageNode toPackage)
            : this((Node)fromProject, (Node)toPackage)
        {
        }


        private PackageReferenceEdge(Node fromNode, Node toNode)
            : base(fromNode, toNode)
        {
        }


        public override string EdgeType { get { return "Package Reference"; } }

        public IDictionary<string, HashSet<string>> Versions { get { return versions; } }


        public void AddVersion(string version, string targetFramework)
        {
            if (versions.ContainsKey(version))
            {
                versions[version].Add(targetFramework);
            }
            else
            {
                versions.Add(version, new HashSet<string> { targetFramework });
            }
        }


        protected override void PopulateProperties(Dictionary<string, string> props)
        {
            foreach (var pair in versions)
            {
                props.Add("Version", $"{pair.Key} ({string.Join(", ", pair.Value)})");
            }
        }
    }
}
