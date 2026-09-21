using Moq;
using NUnit.Framework;
using RPA.ClaimStatements.Data.UnitOfWork;
using RPA.ClaimStatements.Generator.Services;
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
    [Category("AR Controller")]
    public class ARControllerTests
    {
        [Test]
        public void Test_Import_Returns_View()
        {
            FakeUnitOfWork uow = new FakeUnitOfWork();
            Mock<IImportService> importService = new Mock<IImportService>();

            ARController controller = new ARController(uow, importService.Object);

            var result = (RedirectToRouteResult)controller.Import();

            Assert.AreEqual("Index", result.RouteValues["action"]);
        }
    }
}
