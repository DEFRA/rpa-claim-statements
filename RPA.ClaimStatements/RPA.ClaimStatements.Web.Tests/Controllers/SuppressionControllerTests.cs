using Moq;
using NUnit.Framework;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Services;
using RPA.ClaimStatements.Web.Controllers;
using RPA.ClaimStatements.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RPA.ClaimStatements.Web.Tests.Controllers
{
    [TestFixture]
    [Category("Suppression Controller")]
    public class SuppressionControllerTests
    {
        Mock<IClaimStatementsContext> mockContext;
        Mock<ISuppressionService> suppressionService;
        SuppressionController controller;

        [SetUp]
        public void Setup()
        {
            mockContext = new Mock<IClaimStatementsContext>();
            suppressionService = new Mock<ISuppressionService>();
            controller = new SuppressionController(mockContext.Object, suppressionService.Object);
        }        

        [Test]
        public void Test_Create_Returns_View()
        {
            var result = (PartialViewResult)controller._Create();

            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void Test_CreateSuppression_Returns_View()
        {
            var result = (PartialViewResult)controller._CreateSuppression();

            Assert.AreEqual("", result.ViewName);
        }        

        [Test]
        public void Test_BulkSuppression_Returns_View()
        {
            var result = (PartialViewResult)controller._BulkSuppression();

            Assert.AreEqual("", result.ViewName);
        }        
    }
}
