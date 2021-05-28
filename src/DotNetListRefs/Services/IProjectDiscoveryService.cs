// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace DotNetListRefs.Services
{
    public interface IProjectDiscoveryService
    {
        IList<string> DiscoverProjects(string path, bool recursive);
    }
}
