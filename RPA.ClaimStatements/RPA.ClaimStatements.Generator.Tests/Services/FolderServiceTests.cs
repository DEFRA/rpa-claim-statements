using Moq;
using NUnit.Framework;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            mockContext = new Mock<ClaimStatementsContext>();
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
    }
}
