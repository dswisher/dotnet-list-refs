// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Linq;

using DotNetListRefs.Models;

namespace DotNetListRefs.Writers
{
    public class OutdatedWriter
    {
        public void Write(RefGraph graph)
        {
            var datedCheck = graph.Nodes
                .OfType<ProjectNode>()
                .Where(x => x.Versions.Any(y => y.IsOutdated.GetValueOrDefault()))
                .Any();

            if (!datedCheck)
            {
                Console.WriteLine("No outdated packages found.");
                return;
            }

            Console.WriteLine("Project/Framework/Package                                 Specified Version   Prerelease          Patch       Minor       Major");
            Console.WriteLine("--------------------------------------------------------  ------------------  ------------------  ----------  ----------  ----------");

            foreach (var projectNode in graph.Nodes.OfType<ProjectNode>().OrderBy(x => x.Name))
            {
                var outdated = projectNode.Versions
                    .Where(x => x.IsOutdated.GetValueOrDefault());

                if (!outdated.Any())
                {
                    continue;
                }

                Console.WriteLine("{0}", projectNode.Name);

                var frameworks = outdated
                    .Select(x => x.TargetFramework)
                    .Distinct()
                    .OrderBy(x => x);

                foreach (var framework in frameworks)
                {
                    var frameworkOutdated = outdated
                        .Where(x => x.TargetFramework == framework)
                        .OrderBy(x => x.PackageName);

                    if (!frameworkOutdated.Any())
                    {
                        continue;
                    }

                    Console.WriteLine("   {0}", framework);

                    foreach (var version in frameworkOutdated)
                    {
                        var pre = version.SpecifiedVersion == version.LatestPrerelease ? string.Empty : version.LatestPrerelease;
                        var patch = version.SpecifiedVersion == version.LatestPatchUpdate ? string.Empty : version.LatestPatchUpdate;
                        var minor = version.SpecifiedVersion == version.LatestMinorUpdate ? string.Empty : version.LatestMinorUpdate;
                        var major = version.SpecifiedVersion == version.LatestMajorUpdate ? string.Empty : version.LatestMajorUpdate;

                        Console.WriteLine("      {0,-50}  {1,-18}  {2,-18}  {3,-10}  {4,-10}  {5,-10}",
                                version.PackageName, version.SpecifiedVersion, pre, patch, minor, major);
                    }
                }
            }
        }
    }
}
