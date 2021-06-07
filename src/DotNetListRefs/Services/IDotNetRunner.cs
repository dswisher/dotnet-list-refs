// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace DotNetListRefs.Services
{
    public interface IDotNetRunner
    {
        DotNetRunStatus Run(string workingDirectory, string[] arguments);
    }
}
