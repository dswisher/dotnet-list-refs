// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using DotNetListRefs.Models;

namespace DotNetListRefs.Services
{
    public interface ISolutionProcessor
    {
        void AnalyzeSolutions(RefGraph graph);
    }
}
