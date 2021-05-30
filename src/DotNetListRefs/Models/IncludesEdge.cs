// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace DotNetListRefs.Models
{
    public class IncludesEdge : Edge
    {
        public IncludesEdge(SolutionNode solutionNode, ProjectNode projectNode)
            : base(solutionNode, projectNode)
        {
        }


        public override string EdgeType { get { return "Includes"; } }


        protected override void PopulateProperties(Dictionary<string, string> props)
        {
            // No properties - yet
        }
    }
}
