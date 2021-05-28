// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

using DotNetListRefs.Exceptions;

namespace DotNetListRefs.Services
{
    public class ProjectDiscoveryService : IProjectDiscoveryService
    {
        private readonly IFileSystem fileSystem;
        private readonly string[] desiredExtensions = new string[] { ".csproj", ".fsproj", ".sln" };

        public ProjectDiscoveryService(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }


        public IList<string> DiscoverProjects(string path, bool recursive)
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

            // If the path is a directory, do some scanning, otherwise process a file.
            var fileAttributes = fileSystem.File.GetAttributes(path);

            if (fileAttributes.HasFlag(FileAttributes.Directory))
            {
                // TODO - implement recursive search

                // Look for solution files
                var solutionFiles = fileSystem.Directory.GetFiles(path, "*.sln");

                if (solutionFiles.Length == 1)
                {
                    return new List<string> { fileSystem.Path.GetFullPath(solutionFiles[0]) };
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
                    return new List<string> { fileSystem.Path.GetFullPath(projectFiles[0]) };
                }

                if (projectFiles.Count > 1)
                {
                    throw new CommandLineArgumentException("Specify which project file to use because the directory '{0}' contains more than one.", path);
                }
            }
            else
            {
                // Not a directory, so must be a file? If it is a project or solution, use it.
                if (desiredExtensions.Contains(fileSystem.Path.GetExtension(path).ToLower()))
                {
                    return new List<string> { path };
                }
            }

            // At this point, we didn't find anything, so throw.
            throw new CommandLineArgumentException("Could not locate a project or solution starting from '{0}'.", path);
        }
    }
}
