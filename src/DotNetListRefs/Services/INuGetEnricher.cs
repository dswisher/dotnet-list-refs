// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Threading;
using System.Threading.Tasks;

using DotNetListRefs.Models;

namespace DotNetListRefs.Services
{
    public interface INuGetEnricher
    {
        Task EnrichAsync(RefGraph graph, CancellationToken cancellationToken);
    }
}
