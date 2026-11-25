using Moq;
using NUnit.Framework;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Services;
using RPA.ClaimStatements.Generator.Tests.TestSupport;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RPA.ClaimStatements.Generator.Tests.Services
{
    [TestFixture]
    [Category("Error Service")]
    public class ErrorServiceTests
    {
        Mock<DbSet<Error>> mockErrorSet;
        Mock<ClaimStatementsContext> mockContext;
        ErrorService service;

        [SetUp]
        public void Setup()
        {
            var data = new List<Error>();

            mockErrorSet = new Mock<DbSet<Error>>().SetupData(data);

            mockContext = new Mock<ClaimStatementsContext>(ClaimStatementsContextTestFactory.CreateSyntheticOptions());
            mockContext.Setup(x => x.Errors).Returns(mockErrorSet.Object);

            service = new ErrorService(mockContext.Object);
        }

        [Test]
        public void Test_Log_Creates_Log_Without_FilePath()
        {
            service.Log(1234567890, 2016, "Test Log");

            mockErrorSet.Verify(x => x.Add(It.IsAny<Error>()), Times.Once);
        }
    }
}
