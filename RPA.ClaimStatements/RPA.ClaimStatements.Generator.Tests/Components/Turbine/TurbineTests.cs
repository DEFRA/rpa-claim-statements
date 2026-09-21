using Moq;
using NUnit.Framework;
using RPA.ClaimStatements.Generator.Components.FuelTank;
using RPA.ClaimStatements.Generator.Components.Turbine;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.ClaimStatements;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Models.Entities.SITI;
using RPA.ClaimStatements.Generator.Models.Entities.XB;
using RPA.ClaimStatements.Generator.Models.Generation;
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
    [Category("Turbine")]
    public class TurbineTests
    {        
        Mock<IConfigurationService> configurationService;
        Mock<IValidationService> validationService;
        Mock<IFileService> fileService;
        Mock<IErrorService> errorService;
        Mock<IFolderService> folderService;
        Mock<ITransformService> transformService;
        Mock<IMonitorService> monitorService;
        Mock<IBuildService> buildService;
        ITurbine turbine;

        Mock<Claim> claim;
        Mock<SUM> sum;
        Mock<SUM2> sum2;
        Mock<BPS> bps;
        Mock<BPSPEN> bpspen;
        Mock<GR> gr;
        Mock<GRPEN> grpen;
        Mock<YF> yf;
        Mock<CLD> cld;

        Mock<XBData> xbData;
        Mock<Invoice> invoice;
        Mock<List<Invoice>> invoices;

        [SetUp]
        public void Setup()
        {   
            configurationService = new Mock<IConfigurationService>();
            validationService = new Mock<IValidationService>();
            fileService = new Mock<IFileService>();
            errorService = new Mock<IErrorService>();
            folderService = new Mock<IFolderService>();
            transformService = new Mock<ITransformService>();
            monitorService = new Mock<IMonitorService>();
            buildService = new Mock<IBuildService>();

            claim = new Mock<Claim>();
            sum = new Mock<SUM>();
            sum2 = new Mock<SUM2>();
            bps = new Mock<BPS>();
            bpspen = new Mock<BPSPEN>();
            gr = new Mock<GR>();
            grpen = new Mock<GRPEN>();
            yf = new Mock<YF>();
            cld = new Mock<CLD>();
            claim.Setup(x => x.SUM).Returns(sum.Object);
            claim.Setup(x => x.SUM2).Returns(sum2.Object);
            claim.Setup(x => x.BPS).Returns(bps.Object);
            claim.Setup(x => x.BPSPEN).Returns(bpspen.Object);
            claim.Setup(x => x.GR).Returns(gr.Object);
            claim.Setup(x => x.GRPEN).Returns(grpen.Object);
            claim.Setup(x => x.YF).Returns(yf.Object);
            claim.Setup(x => x.CLD).Returns(cld.Object);

            xbData = new Mock<XBData>();
            invoice = new Mock<Invoice>();
            invoices = new Mock<List<Invoice>>();
            invoices.Object.Add(invoice.Object);
            
            turbine = new Generator.Components.Turbine.Turbine(monitorService.Object, validationService.Object, transformService.Object, errorService.Object, configurationService.Object, buildService.Object);                
        }
        
        [Test]
        public void Test_Start_Does_Not_Call_Transformation_If_No_Claims()
        {            
            turbine.Generate(new Request[0], "BPS");

            transformService.Verify(x => x.Transform(It.IsAny<Request>(), It.IsAny<ClaimStatement>(), It.IsAny<string>()), Times.Never);                
        }

        [Test]
        public void Test_Start_Does_Call_Transformation_If_Claims()
        {
            Request[] requests = new Request[1];
            Mock<Request> request = new Mock<Request>(claim.Object, xbData.Object, new DateTime(2016, 1, 1), invoices.Object, new List<Rate>());
            requests[0] = request.Object;
                        
            turbine.Generate(requests, "BPS");

            transformService.Verify(x => x.Transform(It.IsAny<Request>(), It.IsAny<ClaimStatement>(), It.IsAny<string>()));
        }

        [Test]
        public void Test_Start_Does_Not_Call_Transformation_For_Null_Requests()
        {
            Request[] requests = new Request[2];
            Mock<Request> request = new Mock<Request>(claim.Object, xbData.Object, new DateTime(2016, 1, 1), invoices.Object, new List<Rate>());
            requests[0] = request.Object;
            requests[1] = null; // Can be null when there is no SITI data for invoice.

            turbine.Generate(requests, "BPS");

            transformService.Verify(x => x.Transform(It.IsAny<Request>(), It.IsAny<ClaimStatement>(), It.IsAny<string>()), Times.Exactly(1));
        }

        [Test]
        public void Test_Start_Does_Calls_Validate_If_Validation_Active()
        {
            configurationService.Setup(x => x.IsActive("Validation")).Returns(true);

            turbine = new Generator.Components.Turbine.Turbine(monitorService.Object, validationService.Object, transformService.Object, errorService.Object, configurationService.Object, buildService.Object);

            Request[] requests = new Request[1];
            Mock<Request> request = new Mock<Request>(claim.Object, xbData.Object, new DateTime(2016, 1, 1), invoices.Object, new List<Rate>());
            requests[0] = request.Object;
                        
            turbine.Generate(requests, "BPS");

            validationService.Verify(x => x.Validate(It.IsAny<ClaimStatement>(), "BPS"));
        }

        [Test]
        public void Test_Start_Does_Calls_Validate_If_Validation_Active_Multiple_Claims()
        {
            configurationService.Setup(x => x.IsActive("Validation")).Returns(true);

            turbine = new Generator.Components.Turbine.Turbine(monitorService.Object, validationService.Object, transformService.Object, errorService.Object, configurationService.Object, buildService.Object);

            Request[] requests = new Request[4];
            requests[0] = new Mock<Request>(claim.Object, xbData.Object, new DateTime(2016, 1, 1), invoices.Object, new List<Rate>()).Object;
            requests[1] = new Mock<Request>(claim.Object, xbData.Object, new DateTime(2016, 1, 1), invoices.Object, new List<Rate>()).Object;
            requests[2] = new Mock<Request>(claim.Object, xbData.Object, new DateTime(2016, 1, 1), invoices.Object, new List<Rate>()).Object;
            requests[3] = new Mock<Request>(claim.Object, xbData.Object, new DateTime(2016, 1, 1), invoices.Object, new List<Rate>()).Object;

            turbine.Generate(requests, "BPS");

            validationService.Verify(x => x.Validate(It.IsAny<ClaimStatement>(), "BPS"), Times.Exactly(requests.Length));
            transformService.Verify(x => x.Transform(It.IsAny<Request>(), It.IsAny<ClaimStatement>(), It.IsAny<string>()), Times.Exactly(requests.Length));
        }

        [Test]
        public void Test_Start_Does_Not_Call_Validate_If_Validation_Inactive()
        {            
            Request[] requests = new Request[1];
            Mock<Request> request = new Mock<Request>(claim.Object, xbData.Object, new DateTime(2016, 1, 1), invoices.Object, new List<Rate>());
            requests[0] = request.Object;
                        
            turbine.Generate(requests, "BPS");

            validationService.Verify(x => x.Validate(It.IsAny<ClaimStatement>(), "BPS"), Times.Never);
        }        
    }
}
