# dotnet-list-refs

This is a tool to help explore and maintain dependencies for a .NET project (primarily .NET core).


# Usage

The tool looks in the current directory and parent directories for solution and project files.
A project or solution may be explicitly specified using the `--project` or `--solution` options.

There are a few different "outputs" that may be generated/viewed, each controlled by an option.
You should only specify at most one of the following:

* `--outdated` - display a list of outdated dependencies. This is the default, if no other option is specified.
* `--tree` - output a tree of all dependencies. Can be quite long for many projects, especially if `--transitive` is specified.
* `--browse` - use a terminal (aka curses-like) interface to browse through the dependencies. TBD.

There are a few additional options that modify the behavior of the dependency scan:

* `--transitive` - include transitive dependencies.

Use the `--help` option for more details.


# Developer Notes

To test using the local bits:

    # Build the bits that are put into the nupkg folder 
    cd src/DotNetListRefs
    dotnet pack

    # Install the tool globally from the nupkg folder
    dotnet tool install --global --add-source ./nupkg dotnet-list-refs

    # After testing, uninstall
    dotnet tool uninstall -g dotnet-list-refs


* MSFT [Tutorial](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools-how-to-create) on creating .NET tools


# Prior Work

This builds on the work of others.

* [dotnet-oudated](https://github.com/dotnet-outdated/dotnet-outdated) - A .NET Core global tool to display and update outdated NuGet packages in a project
    * [Analyzing .NET Core project dependencies: Finding Package References](https://www.jerriepelser.com/blog/analyze-dotnet-project-dependencies-part-1/)
    * [Analyzing .NET Core project dependencies: Finding transitive dependencies](https://www.jerriepelser.com/blog/analyze-dotnet-project-dependencies-part-2/)
* [bjorkstromm/depends](https://github.com/bjorkstromm/depends) - Tool for generating dependency trees for .NET projects

Command line parsing

* [commandlineparser/commandline](https://github.com/commandlineparser/commandline) - have used in the past, generally happy with it
    * [Command Line Parser on .NET5](https://devblogs.microsoft.com/ifdef-windows/command-line-parser-on-net5/)
* [natemcmaster/CommandLineUtils](https://github.com/natemcmaster/CommandLineUtils) - used by `dotnet-outdated` and `depends`
* [dotnet/command-line-api](https://github.com/dotnet/command-line-api) - seems to be "official", but with many options, seems like it would get unwieldy

