// Copyright (c) Doug Swisher. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;

using DotNetListRefs.Exceptions;
using DotNetListRefs.Models;
using DotNetListRefs.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
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

        private readonly Mock<ILogger<ProjectDiscoveryService>> logger = new Mock<ILogger<ProjectDiscoveryService>>();

        private readonly ProjectDiscoveryService service;
        private readonly RefGraph graph;

        public ProjectDiscoveryServiceTests()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { Project1, MockFileData.NullObject },
                { Project2, MockFileData.NullObject },
                { Solution1, MockFileData.NullObject },
                { EmptyDir, MockFileData.NullObject }
            });

            service = new ProjectDiscoveryService(fileSystem, logger.Object);

            graph = new RefGraph();
        }


        [Theory]
        [InlineData(Project1, typeof(ProjectNode))]
        [InlineData(Solution1, typeof(SolutionNode))]
        public void ExplicitItemsReturnsItem(string path, Type expectedType)
        {
            // Act
            service.DiscoverProjects(path, graph);

            // Assert
            VerifySingleNode(path, expectedType);
        }


        [Fact]
        public void EmptyDirectoryThrows()
        {
            // Arrange
            Action act = () => service.DiscoverProjects(EmptyDir, graph);

            // Act
            act.Should().Throw<CommandLineArgumentException>()
                .Where(x => x.Message.Contains("could not locate", StringComparison.OrdinalIgnoreCase));
        }


        [Fact]
        public void NonExistantPathThrows()
        {
            // Arrange
            Action act = () => service.DiscoverProjects(NoPath, graph);

            // Act
            act.Should().Throw<CommandLineArgumentException>()
                .Where(x => x.Message.Contains("does not exist", StringComparison.OrdinalIgnoreCase));
        }

        [Theory]
        [InlineData(Dir1, Solution1, typeof(SolutionNode))]
        [InlineData(Dir2, Project2, typeof(ProjectNode))]
        public void DirectoryWithStuffReturnsStuff(string path, string expectedPath, Type expectedType)
        {
            // Act
            service.DiscoverProjects(path, graph);

            // Assert
            VerifySingleNode(expectedPath, expectedType);
        }


        private void VerifySingleNode(string expectedPath, Type expectedType)
        {
            graph.Nodes.Should().HaveCount(1);

            var node = graph.Nodes.First();

            node.Should().BeOfType(expectedType);

            if (node is SolutionNode sln)
            {
                sln.SolutionPath.Should().Be(expectedPath);
            }
            else if (node is ProjectNode proj)
            {
                proj.ProjectPath.Should().Be(expectedPath);
            }
        }
    }
}
