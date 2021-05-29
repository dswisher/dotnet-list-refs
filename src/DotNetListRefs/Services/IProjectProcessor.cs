// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using DotNetListRefs.Models;

namespace DotNetListRefs
{
    public interface IProjectProcessor
    {
        void AnalyzeProjects(RefGraph graph);
    }
}
