// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace DotNetListRefs.Exceptions
{
    public class CommandLineArgumentException : Exception
    {
        public CommandLineArgumentException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }
    }
}
