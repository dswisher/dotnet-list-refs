// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using CommandLine;

namespace DotNetListRefs
{
    public class Options
    {
        [Value(0, Required = false, HelpText = "The project file, solution file, or directory to use as input.")]
        public string Path { get; set; }

        [Option("outdated", Required = false, HelpText = "Output a list of outdated dependencies, if any.")]
        public bool ShowOutdated { get; set; }

        [Option("graph-file", Required = false, HelpText = "Write the reference graph to the specified file.")]
        public string GraphFile { get; set; }

        [Option("browse", Required = false, HelpText = "Start a text UI to browse the dependency graph.")]
        public bool Browse { get; set; }

        [Option("transitive", Required = false, HelpText = "Include transitive dependencies.")]
        public bool Transitive { get; set; }

        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }
    }
}
