// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DotNetListRefs.Tests.Services
{
    public class StartupTests
    {
        [Fact]
        public void CanResolveApp()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            Startup.ConfigureServices(services, new Options());

            App app;
            using (var provider = services.BuildServiceProvider())
            {
                app = provider.GetRequiredService<App>();
            }

            // Assert
            app.Should().NotBeNull();
        }
    }
}
