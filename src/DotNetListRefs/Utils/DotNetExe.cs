// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace DotNetListRefs.Utils
{
    /// <summary>
    /// Utility for finding the "dotnet.exe" file from the currently running .NET Core application.
    /// </summary>
    /// <remarks>
    /// Lightly modified version of https://github.com/natemcmaster/CommandLineUtils/blob/main/src/CommandLineUtils/Utilities/DotNetExe.cs
    ///
    /// Any errors/issues introduced are my responsibility and not the original author.
    /// </remarks>
    public static class DotNetExe
    {
        private const string FileName = "dotnet";

        static DotNetExe()
        {
            FullPath = TryFindDotNetExePath();
        }

        /// <summary>
        /// The full filepath to the .NET Core CLI executable.
        /// <para>
        /// May be <c>null</c> if the CLI cannot be found.
        /// </para>
        /// </summary>
        /// <returns>The path or null</returns>
        public static string FullPath { get; }


        /// <summary>
        /// Finds the full filepath to the .NET Core CLI executable,
        /// or returns a string containing the default name of the .NET Core muxer ('dotnet').
        /// <returns>The path or a string named 'dotnet'</returns>
        /// </summary>
        public static string FullPathOrDefault() => FullPath ?? FileName;


        private static string TryFindDotNetExePath()
        {
            var fileName = FileName;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                fileName += ".exe";
            }

            var mainModule = Process.GetCurrentProcess().MainModule;
            if (!string.IsNullOrEmpty(mainModule?.FileName)
                && Path.GetFileName(mainModule.FileName).Equals(fileName, StringComparison.OrdinalIgnoreCase))
            {
                return mainModule.FileName;
            }

            var dotnetRoot = Environment.GetEnvironmentVariable("DOTNET_ROOT");
            return !string.IsNullOrEmpty(dotnetRoot)
                ? Path.Combine(dotnetRoot, fileName)
                : null;
        }
    }
}
