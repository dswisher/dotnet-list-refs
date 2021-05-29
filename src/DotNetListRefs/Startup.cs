// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO.Abstractions;

using DotNetListRefs.Services;
using DotNetListRefs.Writers;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace DotNetListRefs
{
    public class Startup
    {
        public static void ConfigureServices(IServiceCollection services, Options options)
        {
            services.AddSingleton<App>();

            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<IProjectDiscoveryService, ProjectDiscoveryService>();
            services.AddSingleton<IProjectProcessor, BuildalyzerProjectProcessor>();
            services.AddSingleton<ISolutionProcessor, BuildalyzerSolutionProcessor>();

            // TODO - based on the output type settings, add the proper writer
            services.AddSingleton<IGraphWriter, TextGraphWriter>();

            services.AddLogging(c =>
            {
                c.AddSerilog(dispose: true);
            });
        }
    }
}
