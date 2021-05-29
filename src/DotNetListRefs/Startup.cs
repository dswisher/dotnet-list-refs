// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO.Abstractions;

using DotNetListRefs.Services;
using DotNetListRefs.Writers;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetListRefs
{
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services, Options options)
        {
            services.AddSingleton<App>();

            services.AddSingleton<IProjectDiscoveryService, ProjectDiscoveryService>();
            services.AddSingleton<IFileSystem, FileSystem>();

            // TODO - based on the output type settings, add the proper writer
            services.AddSingleton<IGraphWriter, TextGraphWriter>();
        }
    }
}
