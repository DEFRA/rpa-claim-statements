using Moq;
using NUnit.Framework;
using RPA.ClaimStatements.Generator.Components;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Services;
using RPA.ClaimStatements.Web.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace RPA.ClaimStatements.Web.Tests.Controllers
{
    [TestFixture]
    [Category("Home Controller")]
    public class HomeControllerTests
    {
        Mock<IPowerPlant> powerPlant;
        Mock<ISampleService> sampleService;
        Mock<ISampleXBService> sampleXBService;
        HomeController controller;

        [SetUp]
        public void Setup()
        {
            powerPlant = new Mock<IPowerPlant>();
            sampleService = new Mock<ISampleService>();
            sampleXBService = new Mock<ISampleXBService>();
            controller = new HomeController(powerPlant.Object, sampleService.Object, sampleXBService.Object);
        }

        [Test]
        public void Test_Index_Returns_View()
        {
            var result = (ViewResult)controller.Index();

            Assert.AreEqual("", result.ViewName);
        }

        [Test]
        public void Test_Trigger_Returns_View()
        {
            var result = (RedirectToRouteResult)controller.Trigger();

            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
    }
}
