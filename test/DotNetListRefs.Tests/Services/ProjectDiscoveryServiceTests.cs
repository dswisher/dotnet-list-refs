// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;

using DotNetListRefs.Exceptions;
using DotNetListRefs.Services;
using FluentAssertions;
using Xunit;

namespace DotNetListRefs.Tests.Services
{
    public class ProjectDiscoveryServiceTests
    {
        private const string NoPath = "/home/missing";
        private const string Dir1 = "/home/user/proj1/";
        private const string Project1 = Dir1 + "proj1.csproj";
        private const string Solution1 = Dir1 + "coolbeans.sln";
        private const string Dir2 = "/home/user/proj2/";
        private const string Project2 = Dir2 + "proj2.csproj";
        private const string EmptyDir = "/home/user/pending";

        private readonly ProjectDiscoveryService service;

        public ProjectDiscoveryServiceTests()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Project1, MockFileData.NullObject },
                { Project2, MockFileData.NullObject },
                { Solution1, MockFileData.NullObject },
                { EmptyDir, MockFileData.NullObject }
            });

            service = new ProjectDiscoveryService(fileSystem);
        }


        [Theory]
        [InlineData(Project1)]
        [InlineData(Solution1)]
        public void ExplicitItemsReturnsItem(string path)
        {
            // Act
            var projects = service.DiscoverProjects(path, false);

            // Assert
            projects.Should().HaveCount(1);
            projects.First().Should().Be(path);
        }


        [Fact]
        public void EmptyDirectoryThrows()
        {
            // Arrange
            Action act = () => service.DiscoverProjects(EmptyDir, false);

            // Act
            act.Should().Throw<CommandLineArgumentException>()
                .Where(x => x.Message.Contains("could not locate", StringComparison.OrdinalIgnoreCase));
        }


        [Fact]
        public void NonExistantPathThrows()
        {
            // Arrange
            Action act = () => service.DiscoverProjects(NoPath, false);

            // Act
            act.Should().Throw<CommandLineArgumentException>()
                .Where(x => x.Message.Contains("does not exist", StringComparison.OrdinalIgnoreCase));
        }

        [Theory]
        [InlineData(Dir1, Solution1)]
        [InlineData(Dir2, Project2)]
        public void DirectoryWithStuffReturnsStuff(string path, string expectedItem)
        {
            // Act
            var projects = service.DiscoverProjects(path, false);

            // Assert
            projects.Should().HaveCount(1);
            projects.First().Should().Be(expectedItem);
        }
    }
}
