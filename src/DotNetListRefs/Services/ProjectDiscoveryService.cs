// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

using DotNetListRefs.Exceptions;
using DotNetListRefs.Models;
using Microsoft.Extensions.Logging;

namespace DotNetListRefs.Services
{
    public class ProjectDiscoveryService : IProjectDiscoveryService
    {
        private readonly IFileSystem fileSystem;
        private readonly ILogger logger;

        private readonly string[] projectExtensions = new string[] { ".csproj", ".fsproj" };
        private readonly string[] solutionExtensions = new string[] { ".sln" };

        public ProjectDiscoveryService(IFileSystem fileSystem,
                                       ILogger<ProjectDiscoveryService> logger)
        {
            this.fileSystem = fileSystem;
            this.logger = logger;
        }


        public void DiscoverProjects(string path, RefGraph graph)
        {
            // If the path is empty, use the current directory.
            if (string.IsNullOrEmpty(path))
            {
                path = Environment.CurrentDirectory;
            }

            // If the path does not exist, report an error.
            if (!(fileSystem.File.Exists(path) || fileSystem.Directory.Exists(path)))
            {
                throw new CommandLineArgumentException("The directory or file '{0}' does not exist.", path);
            }

            logger.LogDebug("Scanning for projects/solutions, path: {Path}.", path);

            // If the path is a directory, do some scanning, otherwise process a file.
            var fileAttributes = fileSystem.File.GetAttributes(path);

            if (fileAttributes.HasFlag(FileAttributes.Directory))
            {
                // Look for solution files
                var solutionFiles = fileSystem.Directory.GetFiles(path, "*.sln");

                if (solutionFiles.Length == 1)
                {
                    AddSolutionNode(graph, fileSystem.Path.GetFullPath(solutionFiles[0]));
                    return;
                }

                if (solutionFiles.Length > 1)
                {
                    throw new CommandLineArgumentException(string.Format("Specify which solution file to use because the directory '{0}' contains more than one.", path));
                }

                // Look for project files
                var projectFiles = fileSystem
                    .Directory.GetFiles(path, "*.csproj")
                    .Concat(fileSystem.Directory.GetFiles(path, "*.fsproj"))
                    .ToList();

                if (projectFiles.Count == 1)
                {
                    AddProjectNode(graph, fileSystem.Path.GetFullPath(projectFiles[0]));
                    return;
                }

                if (projectFiles.Count > 1)
                {
                    throw new CommandLineArgumentException("Specify which project file to use because the directory '{0}' contains more than one.", path);
                }
            }
            else
            {
                // Not a directory, so must be a file? If it is a project or solution, use it.
                if (solutionExtensions.Contains(fileSystem.Path.GetExtension(path).ToLower()))
                {
                    AddSolutionNode(graph, fileSystem.Path.GetFullPath(path));
                    return;
                }

                if (projectExtensions.Contains(fileSystem.Path.GetExtension(path).ToLower()))
                {
                    AddProjectNode(graph, fileSystem.Path.GetFullPath(path));
                    return;
                }
            }

            // At this point, we didn't find anything, so throw.
            throw new CommandLineArgumentException("Could not locate a project or solution starting from '{0}'.", path);
        }


        private void AddSolutionNode(RefGraph graph, string path)
        {
            var node = new SolutionNode(path);

            graph.AddNode(node);
        }


        private void AddProjectNode(RefGraph graph, string path)
        {
            var node = new ProjectNode(path);

            graph.AddNode(node);
        }
    }
}
