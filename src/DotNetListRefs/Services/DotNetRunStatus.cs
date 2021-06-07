// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace DotNetListRefs.Services
{
    public class DotNetRunStatus
    {
        public DotNetRunStatus(string output, string errors, int exitCode)
        {
            Output = output;
            Errors = errors;
            ExitCode = exitCode;
        }

        public string Output { get; private set; }
        public string Errors { get; private set; }
        public int ExitCode { get; private set; }

        public bool IsSuccess => ExitCode == 0;
    }
}
