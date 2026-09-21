using Moq;
using NUnit.Framework;
using RPA.ClaimStatements.Data.Context;
using RPA.ClaimStatements.Data.Entities.CS;
using RPA.ClaimStatements.Data.Entities.SITI;
using RPA.ClaimStatements.Data.Entities.XB;
using RPA.ClaimStatements.Generator.Imports;
using RPA.ClaimStatements.Generator.Models.ClaimStatements;
using RPA.ClaimStatements.Generator.Models.Generation;
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
    [Category("Generation Service")]
    public class GenerationServiceTests
    {
        Mock<ClaimStatementsContext> mockContext;
        Mock<ILoadService> loadService;
        Mock<IImportService> importService;
        Mock<IConfigurationService> configurationService;
        Mock<IValidationService> validationService;
        Mock<IFileService> fileService;
        Mock<IErrorService> errorService;
        Mock<IReportService> reportService;
        Mock<IFolderService> folderService;
        Mock<ITransformService> transformService;
        Mock<IMonitorService> monitorService;
        Mock<IBuildService> buildService;
        Mock<ISampleService> sampleService;

        GenerationService generationService;

        Mock<DbSet<MonitorProcess>> mockMonitorProcessSet;
        Mock<DbSet<Monitor>> mockMonitorSet;
        Mock<DbSet<Transformation>> mockTransformationSet;
        Mock<DbSet<SchemeYear>> mockSchemeYearSet;
        Mock<DbSet<ConversionRate>> mockConversionRateSet;

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
            mockContext = new Mock<ClaimStatementsContext>();

            loadService = new Mock<ILoadService>();
            importService = new Mock<IImportService>();
            configurationService = new Mock<IConfigurationService>();
            validationService = new Mock<IValidationService>();
            fileService = new Mock<IFileService>();
            errorService = new Mock<IErrorService>();
            reportService = new Mock<IReportService>();
            folderService = new Mock<IFolderService>();
            transformService = new Mock<ITransformService>();
            monitorService = new Mock<IMonitorService>();
            buildService = new Mock<IBuildService>();
            sampleService = new Mock<ISampleService>();

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

            var monitorProcessdata = new List<MonitorProcess>
            {
                new MonitorProcess { ProcessName = "Load Claim Statement Data" },
                new MonitorProcess { ProcessName = "Claim Statement Generation" }
            };

            mockMonitorProcessSet = new Mock<DbSet<MonitorProcess>>().SetupData(monitorProcessdata);            
            mockContext.Setup(x => x.MonitorProcesses).Returns(mockMonitorProcessSet.Object);

            var monitorData = new List<Monitor>();

            mockMonitorSet = new Mock<DbSet<Monitor>>().SetupData(monitorData);
            mockContext.Setup(x => x.Monitor).Returns(mockMonitorSet.Object);

            var transformData = new List<Transformation>
            {
                new Transformation("BPS", "XSLT", "Template"),
                new Transformation("XB", "XSLT", "Template")
            };

            mockTransformationSet = new Mock<DbSet<Transformation>>().SetupData(transformData);
            mockContext.Setup(x => x.Transformations).Returns(mockTransformationSet.Object);

            var schemeYearData = new List<SchemeYear>
            {
                new SchemeYear(2015,true),
                new SchemeYear(2016,true),
                new SchemeYear(2017,true)
            };

            mockSchemeYearSet = new Mock<DbSet<SchemeYear>>().SetupData(schemeYearData);
            mockContext.Setup(x => x.SchemeYears).Returns(mockSchemeYearSet.Object);

            var conversionRateData = new List<ConversionRate>();

            mockConversionRateSet = new Mock<DbSet<ConversionRate>>().SetupData(conversionRateData);
            mockContext.Setup(x => x.ConversionRates).Returns(mockConversionRateSet.Object);

            generationService = new GenerationService(mockContext.Object, loadService.Object, importService.Object, importService.Object, importService.Object, importService.Object, configurationService.Object, validationService.Object,
                fileService.Object, errorService.Object, reportService.Object, folderService.Object, transformService.Object, monitorService.Object, buildService.Object, buildService.Object, sampleService.Object, sampleService.Object);
        }

        [Test]
        public void Test_Start_Calls_ImportAR()
        {
            generationService.Start("BPS", 100);

            importService.Verify(x => x.Import());
        }

        [Test]
        public void Test_Start_Does_Not_Call_Load_If_Generation_Inactive()
        {   
            configurationService.Setup(x => x.IsActive("Generation")).Returns(false);            
            
            generationService.Start("BPS", 1);

            loadService.Verify(x => x.GetClaims("BPS", 1, It.IsAny<bool>()), Times.Never);
        }

        [Test]
        public void Test_Start_Does_Not_Call_Transformation_If_No_Claims()
        {            
            loadService.Setup(x => x.GetClaims(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>())).Returns(new Request[0]);
            
            generationService.Generate("BPS", 100);

            transformService.Verify(x => x.Transform(It.IsAny<Request>(), It.IsAny<ClaimStatement>(), It.IsAny<string>()), Times.Never);                
        }

        [Test]
        public void Test_Start_Does_Call_Transformation_If_Claims()
        {
            Request[] requests = new Request[1];
            Mock<Request> request = new Mock<Request>(claim.Object, xbData.Object, new DateTime(2016, 1, 1), invoices.Object, new List<Rate>());
            requests[0] = request.Object;

            loadService.Setup(x => x.GetClaims(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>())).Returns(requests);
            
            generationService.Generate("BPS", 100);

            transformService.Verify(x => x.Transform(It.IsAny<Request>(), It.IsAny<ClaimStatement>(), It.IsAny<string>()));
        }

        [Test]
        public void Test_Start_Does_Calls_Validate_If_Validation_Active()
        {
            configurationService.Setup(x => x.IsActive("Validation")).Returns(true);

            generationService = new GenerationService(mockContext.Object, loadService.Object, importService.Object, importService.Object, importService.Object, importService.Object, configurationService.Object, validationService.Object,
                fileService.Object, errorService.Object, reportService.Object, folderService.Object, transformService.Object, monitorService.Object, buildService.Object, buildService.Object, sampleService.Object, sampleService.Object);

            Request[] requests = new Request[1];
            Mock<Request> request = new Mock<Request>(claim.Object, xbData.Object, new DateTime(2016, 1, 1), invoices.Object, new List<Rate>());
            requests[0] = request.Object;

            loadService.Setup(x => x.GetClaims(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>())).Returns(requests);
            
            generationService.Generate("BPS", 100);

            validationService.Verify(x => x.Validate(It.IsAny<ClaimStatement>(), "BPS"));
        }

        [Test]
        public void Test_Start_Does_Not_Call_Validate_If_Validation_Inactive()
        {            
            Request[] requests = new Request[1];
            Mock<Request> request = new Mock<Request>(claim.Object, xbData.Object, new DateTime(2016, 1, 1), invoices.Object, new List<Rate>());
            requests[0] = request.Object;

            loadService.Setup(x => x.GetClaims(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>())).Returns(requests);
            
            generationService.Generate("BPS", 100);

            validationService.Verify(x => x.Validate(It.IsAny<ClaimStatement>(), "BPS"), Times.Never);
        }        
    }
}
