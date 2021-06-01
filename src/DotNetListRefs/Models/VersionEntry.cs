// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace DotNetListRefs.Models
{
    public class VersionEntry
    {
        public string TargetFramework { get; set; }
        public string PackageName { get; set; }
        public string SpecifiedVersion { get; set; }
        public bool? IsOutdated { get; set; }

        public string LatestPrerelease { get; set; }
        public string LatestPatchUpdate { get; set; }
        public string LatestMinorUpdate { get; set; }
        public string LatestMajorUpdate { get; set; }
    }
}
