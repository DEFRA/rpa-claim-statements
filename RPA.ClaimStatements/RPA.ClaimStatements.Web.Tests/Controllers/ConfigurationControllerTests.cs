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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

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
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
        }

        [Test]
        public void Test_Index_Returns_View_On_Post()
        {
            var result = (RedirectToActionResult)controller.Index(new List<Configuration>());

            Assert.AreEqual("Index", result.ActionName);
        }        

        [Test]
        public void Test_SchemeYear_Returns_View_On_Post()
        {
            var result = (RedirectToActionResult)controller.SchemeYear(new List<SchemeYear>());

            Assert.AreEqual("SchemeYear", result.ActionName);
        }
    }
}
