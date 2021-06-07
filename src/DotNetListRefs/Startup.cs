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
            services.AddSingleton<INuGetEnricher, NuGetEnricher>();
            services.AddSingleton<IProjectDiscoveryService, ProjectDiscoveryService>();
            services.AddSingleton<ISolutionProcessor, BuildalyzerSolutionProcessor>();
            services.AddSingleton<IVersionChecker, VersionChecker>();

            if (options.UseMsbuild)
            {
                services.AddSingleton<IProjectProcessor, MsbuildProjectProcessor>();
                services.AddSingleton<IDotNetRunner, DotNetRunner>();
            }
            else
            {
                services.AddSingleton<IProjectProcessor, BuildalyzerProjectProcessor>();
            }

            services.AddSingleton<OutdatedWriter>();
            services.AddSingleton<TextGraphWriter>();
            services.AddSingleton<DotGraphWriter>();

            services.AddLogging(c =>
            {
                c.AddSerilog(dispose: true);
            });
        }
    }
}
