// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace DotNetListRefs.Models
{
    public class PackageNode : Node
    {
        public PackageNode(string name)
            : base(name)
        {
        }


        public override string NodeType { get { return "Package"; } }


        protected override void PopulateProperties(Dictionary<string, string> props)
        {
        }
    }
}
