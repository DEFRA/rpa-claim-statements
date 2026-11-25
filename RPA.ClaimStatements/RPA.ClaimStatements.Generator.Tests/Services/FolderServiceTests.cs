using Moq;
using NUnit.Framework;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Services;
using RPA.ClaimStatements.Generator.Tests.TestSupport;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace RPA.ClaimStatements.Generator.Tests.Services
{
    [TestFixture]
    [Category("Folder Service")]
    public class FolderServiceTests
    {
        Mock<DbSet<Folder>> mockFolderSet;
        Mock<ClaimStatementsContext> mockContext;
        FolderService service;

        [SetUp]
        public void Setup()
        {
            var data = new List<Folder>
            {
                new Folder { Description = "Folder1", Mask = "Mask1", Path = "Path1" },
                new Folder { Description = "Folder2", Mask = "Mask2", Path = "Path2" }
            };

            mockFolderSet = new Mock<DbSet<Folder>>().SetupData(data);

            mockContext = new Mock<ClaimStatementsContext>(ClaimStatementsContextTestFactory.CreateSyntheticOptions());
            mockContext.Setup(x => x.Folders).Returns(mockFolderSet.Object);

            service = new FolderService(mockContext.Object);
        }

        [Test]
        public void Test_Get_Returns_Folder()
        {
            var result = service.Get("Folder1");

            Assert.IsNotNull(result);
        }

        [Test]
        public void Test_GetPath_Returns_Path()
        {            
            var result = service.GetPath("Folder1");

            Assert.AreEqual("Path1", result);
        }

        // FolderService.ResolveRoot is a pure resolver (no real environment variable or filesystem
        // access), so these cases run deterministically without setting process-wide state.

        [TestCase(null)]
        [TestCase("   ")]
        public void Test_ResolveRoot_Throws_When_Not_Configured_Or_Blank(string configuredRoot)
        {
            var error = Assert.Throws<InvalidOperationException>(() => FolderService.ResolveRoot(configuredRoot));

            StringAssert.Contains(FolderService.RootEnvironmentVariableName, error.Message);
        }

        [Test]
        public void Test_ResolveRoot_Returns_Configured_Absolute_Root()
        {
            string configuredRoot = Path.Combine(Path.GetTempPath(), "csg-configured-root");

            string result = FolderService.ResolveRoot(configuredRoot);

            Assert.AreEqual(Path.GetFullPath(configuredRoot), result);
        }

        [TestCase("relative/path")]
        [TestCase(".")]
        public void Test_ResolveRoot_Throws_When_Configured_Root_Is_Relative(string configuredRoot)
        {
            Assert.Throws<InvalidOperationException>(() => FolderService.ResolveRoot(configuredRoot));
        }

        [Test]
        [NonParallelizable]
        public void Test_Root_Reads_The_Environment_Variable()
        {
            string originalEnvValue = Environment.GetEnvironmentVariable(FolderService.RootEnvironmentVariableName);
            try
            {
                string configuredRoot = Path.Combine(Path.GetTempPath(), "csg-env-root");
                Environment.SetEnvironmentVariable(FolderService.RootEnvironmentVariableName, configuredRoot);

                Assert.AreEqual(Path.GetFullPath(configuredRoot), service.Root());

                Environment.SetEnvironmentVariable(FolderService.RootEnvironmentVariableName, null);

                Assert.Throws<InvalidOperationException>(() => service.Root());
            }
            finally
            {
                Environment.SetEnvironmentVariable(FolderService.RootEnvironmentVariableName, originalEnvValue);
            }
        }
    }
}
