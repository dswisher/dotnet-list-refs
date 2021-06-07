// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Linq;

using DotNetListRefs.Models;

namespace DotNetListRefs.Helpers
{
    public static class NodeHelpers
    {
        public static PackageNode AddPackageReference(this RefGraph graph, ProjectNode projectNode, string targetFramework, string packageName, string version)
        {
            // Look to see if this package already exists in the graph.
            var packageNode = graph.Nodes
                .OfType<PackageNode>()
                .Where(x => x.Name == packageName)
                .FirstOrDefault();

            if (packageNode == null)
            {
                packageNode = new PackageNode(packageName);
                graph.AddNode(packageNode);
            }

            // Look to see if there is already a link between the nodes. If not, create one.
            var edge = projectNode.OutEdges
                .Where(x => x.ToNode == packageNode)
                .Cast<PackageReferenceEdge>()
                .FirstOrDefault();

            if (edge == null)
            {
                // Edge does not exist - create it
                edge = new PackageReferenceEdge(projectNode, packageNode);

                graph.AddEdge(edge);
            }

            edge.AddVersion(version, targetFramework);

            return packageNode;
        }
    }
}
