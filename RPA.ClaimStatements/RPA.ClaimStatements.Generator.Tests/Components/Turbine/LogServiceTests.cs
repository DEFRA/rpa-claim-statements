using Moq;
using NUnit.Framework;
using RPA.ClaimStatements.Generator.Components.Turbine;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Models.Entities.DAX;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Tests.Components.Turbine
{
    [TestFixture]
    [Category("Log Service")]
    public class LogServiceTests
    {
        Mock<DbSet<Log>> mockLogSet;
        Mock<DbSet<AP>> mockAPSet;
        Mock<DbSet<AR>> mockARSet;
        Mock<ClaimStatementsContext> mockContext;
        LogService service;

        [SetUp]
        public void Setup()
        {
            mockLogSet = new Mock<DbSet<Log>>();

            var apData = new List<AP>
            {
                new AP(1234567890, 2016)
            };
            mockAPSet = new Mock<DbSet<AP>>().SetupData(apData);
            mockAPSet.Setup(x => x.Find(It.IsAny<object>())).Returns(apData.FirstOrDefault());

            var arData = new List<AR>
            {
                new AR(1234567890, 2016)
        };
            mockARSet = new Mock<DbSet<AR>>().SetupData(arData);
            mockARSet.Setup(x => x.Find(It.IsAny<object>())).Returns(arData.FirstOrDefault());

            mockContext = new Mock<ClaimStatementsContext>();
            mockContext.Setup(x => x.Log).Returns(mockLogSet.Object);
            mockContext.Setup(x => x.AP).Returns(mockAPSet.Object);
            mockContext.Setup(x => x.AR).Returns(mockARSet.Object);

            service = new LogService(mockContext.Object);
        }

        [Test]
        public void TestLog_Creates_Log()
        {
            service.Log(1234567890, 123456789, 2016, "Test Filepath", Guid.NewGuid());

            mockLogSet.Verify(x => x.Add(It.IsAny<Log>()), Times.Once);
        }

        [Test]
        public void TestLog_Updates_AP()
        {
            service.Log(1234567890, 123456789, 2016, "Test Filepath", Guid.NewGuid());

            var result = mockAPSet.Object.Find(1);

            Assert.IsNotNull(result.LogID);
        }

        [Test]
        public void TestLog_Updates_AR()
        {
            service.Log(1234567890, 123456789, 2016, "Test Filepath", Guid.NewGuid());

            var result = mockARSet.Object.Find(1);

            Assert.IsNotNull(result.LogID);
        }
    }
}
