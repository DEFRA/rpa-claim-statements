using Moq;
using NUnit.Framework;
using RPA.ClaimStatements.Generator.Components.PumpingStation;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Services;
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
    [Category("Import Controller")]
    public class ImportControllerTests
    {
        [Test]
        public void Test_Import_Returns_View()
        {
            Mock<IClaimStatementsContext> mockContext = new Mock<IClaimStatementsContext>();
            Mock<IPumpingStation> pumpingStation = new Mock<IPumpingStation>();

            ImportController controller = new ImportController(mockContext.Object, pumpingStation.Object);
            controller.TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());

            var result = (RedirectToActionResult)controller.Import();

            Assert.AreEqual("Index", result.ActionName);
        }
    }
}
