// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using CommandLine;

namespace DotNetListRefs
{
    public class Options
    {
        [Option("path", Required = false, HelpText = "The csproj, solution file, or directory to use as input.")]
        public string Path { get; set; }

        [Option("recursive", Required = false, HelpText = "Scan the directory and all subdirectories for project files.")]
        public bool Recursive { get; set; }

        [Option("outdated", Required = false, HelpText = "Output a list of outdated dependencies, if any.")]
        public bool ShowOutdated { get; set; }

        [Option("tree", Required = false, HelpText = "Output a tree of dependencies.")]
        public bool ShowTree { get; set; }

        [Option("browse", Required = false, HelpText = "Start a text UI to browse the dependency graph.")]
        public bool Browse { get; set; }

        [Option("transitive", Required = false, HelpText = "Include transitive dependencies.")]
        public bool Transitive { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    }
}
