using Moq;
using NUnit.Framework;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RPA.ClaimStatements.Web.Tests.Controllers
{
    [TestFixture]
    [Category("Configuration Controller")]
    public class ConfigurationControllerTests
    {
        Mock<IClaimStatementsContext> mockContext;
        ConfigurationController controller;

        [SetUp]
        public void Setup()
        {
            mockContext = new Mock<IClaimStatementsContext>();
            controller = new ConfigurationController(mockContext.Object);
        }

        [Test]
        public void Test_Index_Returns_View_On_Post()
        {
            var result = (RedirectToRouteResult)controller.Index(new List<Configuration>());

            Assert.AreEqual("Index", result.RouteValues["action"]);
        }        

        [Test]
        public void Test_SchemeYear_Returns_View_On_Post()
        {
            var result = (RedirectToRouteResult)controller.SchemeYear(new List<SchemeYear>());

            Assert.AreEqual("SchemeYear", result.RouteValues["action"]);
        }
    }
}
