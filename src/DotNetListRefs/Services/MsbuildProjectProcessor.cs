// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO.Abstractions;
using System.Linq;

using DotNetListRefs.Helpers;
using DotNetListRefs.Models;
using Microsoft.Extensions.Logging;
using NuGet.ProjectModel;

namespace DotNetListRefs.Services
{
    /// <summary>
    /// Use the dotnet CLI to generate the project dependency graph
    /// </summary>
    /// <remarks>
    /// Based heavily on the code here: https://github.com/dotnet-outdated/dotnet-outdated/blob/master/src/DotNetOutdated.Core/Services/DependencyGraphService.cs
    ///
    /// That code gives credit to the https://github.com/jaredcnance/dotnet-status project
    /// </remarks>
    public class MsbuildProjectProcessor : IProjectProcessor
    {
        private readonly IDotNetRunner dotNetRunner;
        private readonly IFileSystem fileSystem;
        private readonly ILogger logger;

        public MsbuildProjectProcessor(IDotNetRunner dotNetRunner,
                                       IFileSystem fileSystem,
                                       ILogger<MsbuildProjectProcessor> logger)
        {
            this.dotNetRunner = dotNetRunner;
            this.fileSystem = fileSystem;
            this.logger = logger;
        }


        public void AnalyzeProjects(RefGraph graph)
        {
            foreach (var projectNode in graph.Nodes.OfType<ProjectNode>().ToList())
            {
                logger.LogInformation("Analyzing project {Name}...", projectNode.Name);

                // var dgOutput = fileSystem.Path.Combine(fileSystem.Path.GetTempPath(), fileSystem.Path.GetTempFileName());
                var dgOutput = "/tmp/graph.dg";     // TODO! Use temp location!

                try
                {
                    logger.LogInformation(" -> running MSBUILD to generate project output, dgOutput={Path}", dgOutput);

                    // TODO - dotnet-outdated does a restore here - why? Needed?
                    // string[] arguments = {"msbuild", $"\"{projectNode.ProjectPath}\"", "/t:Restore,GenerateRestoreGraphFile", $"/p:RestoreGraphOutputPath=\"{dgOutput}\""};
                    string[] arguments = { "msbuild", $"\"{projectNode.ProjectPath}\"", "/t:GenerateRestoreGraphFile", $"/p:RestoreGraphOutputPath=\"{dgOutput}\"" };

                    var runStatus = dotNetRunner.Run(fileSystem.Path.GetDirectoryName(projectNode.ProjectPath), arguments);

                    if (!runStatus.IsSuccess)
                    {
                        // TODO - better exception
                        throw new System.Exception("Boom!");
                    }

                    // Load the model that MSBuild created
                    var model = DependencyGraphSpec.Load(dgOutput);

                    // Go through each project
                    foreach (var proj in model.Projects)
                    {
                        // Add the project to the graph (even though we don't quite know where it fits)
                        AddProject(graph, proj);

                        // TODO

                        foreach (var fw in proj.TargetFrameworks)
                        {
                            foreach (var dep in fw.Dependencies)
                            {
                                // LibraryRange has version info: dep.LibraryRange.VersionRange
                                // TODO - preserve the VersionRange! The AddPackageReference method should take a VersionRange, and not a string!
                                var version = dep.LibraryRange.VersionRange.MinVersion.ToString();

                                graph.AddPackageReference(projectNode, fw.FrameworkName.ToString(), dep.Name, version);

                                logger.LogInformation("       -> dependency {Name}, include type {IncType}", dep.Name, dep.IncludeType);
                            }
                        }
                    }

                    // TODO - add project references
                }
                finally
                {
                    if (fileSystem.File.Exists(dgOutput))
                    {
                        // TODO - put this back!
                        // fileSystem.File.Delete(dgOutput);
                    }
                }
            }
        }


        private void AddProject(RefGraph graph, PackageSpec project)
        {
            // Look to see if this project already exists in the graph.
            var refProjectNode = graph.Nodes
                .OfType<ProjectNode>()
                .Where(x => x.ProjectPath == project.FilePath)
                .FirstOrDefault();

            if (refProjectNode == null)
            {
                refProjectNode = new ProjectNode(project.FilePath);
                graph.AddNode(refProjectNode);
            }
        }
    }
}
