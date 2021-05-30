// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace DotNetListRefs.Models
{
    public class ProjectReferenceEdge : Edge
    {
        public ProjectReferenceEdge(ProjectNode fromProject, ProjectNode toProject)
            : base(fromProject, toProject)
        {
            TargetFrameworks = new List<string>();
        }


        public override string EdgeType { get { return "Project Reference"; } }

        public List<string> TargetFrameworks { get; private set; }


        protected override void PopulateProperties(Dictionary<string, string> props)
        {
            props.Add("Target Frameworks", string.Join(", ", TargetFrameworks));
        }
    }
}
