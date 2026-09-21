using Moq;
using NUnit.Framework;
using RPA.ClaimStatements.Data.Context;
using RPA.ClaimStatements.Data.Entities.CS;
using RPA.ClaimStatements.Data.Entities.DAX;
using RPA.ClaimStatements.Data.Entities.SITI;
using RPA.ClaimStatements.Data.Entities.XB;
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
    [Category("Load Service")]
    public class LoadServiceTests
    {
        Mock<ClaimStatementsContext> mockContext;
        Mock<IConfigurationService> configurationService;
        Mock<IErrorService> errorService;
        ConversionService conversionService;
        CurrencyService currencyService;
        Mock<DbSet<SchemeYear>> mockSchemeYearSet;
        Mock<DbSet<Claim>> mockClaimSet;
        Mock<DbSet<SUM>> mockSUMSet;
        Mock<DbSet<SUM2>> mockSUM2Set;
        Mock<DbSet<AP>> mockAPSet;
        Mock<DbSet<AR>> mockARSet;
        Mock<DbSet<Error>> mockErrorSet;
        Mock<DbSet<Suppression>> mockSuppressionSet;
        Mock<DbSet<XB>> mockXBSet;
        Mock<DbSet<XBData>> mockXBDataSet;
        Mock<DbSet<ConversionRate>> mockConversionRateSet;
        LoadService loadService;

        List<Claim> claimData;
        List<SUM> sumData;
        List<SUM2> sum2Data;
        List<AP> apData;
        List<AR> arData;
        List<Error> errorData;
        List<XB> xbData;
        List<XBData> xbDataData;
        List<Suppression> suppressionData;
        List<SchemeYear> schemeYearData;

        [SetUp]
        public void Setup()
        {
            mockContext = new Mock<ClaimStatementsContext>();
            configurationService = new Mock<IConfigurationService>();
            errorService = new Mock<IErrorService>();
            conversionService = new ConversionService();
            currencyService = new CurrencyService();

            schemeYearData = new List<SchemeYear>
            {
                new SchemeYear(2015),
                new SchemeYear(2016)
            };
            mockSchemeYearSet = new Mock<DbSet<SchemeYear>>().SetupData(schemeYearData);            
            mockContext.Setup(x => x.SchemeYears).Returns(mockSchemeYearSet.Object);

            var conversionRateData = new List<ConversionRate>
            {
                new ConversionRate("Euro Exchange", 1, 2016),
                new ConversionRate("Euro Exchange", 1, 2015)
            };
            mockConversionRateSet = new Mock<DbSet<ConversionRate>>().SetupData(conversionRateData);
            mockContext.Setup(x => x.ConversionRates).Returns(mockConversionRateSet.Object);

            claimData = new List<Claim>();
            mockClaimSet = new Mock<DbSet<Claim>>().SetupData(claimData);
            mockContext.Setup(x => x.Claims).Returns(mockClaimSet.Object);

            sumData = new List<SUM>();
            mockSUMSet = new Mock<DbSet<SUM>>().SetupData(sumData);
            mockContext.Setup(x => x.SUM).Returns(mockSUMSet.Object);

            sum2Data = new List<SUM2>();
            mockSUM2Set = new Mock<DbSet<SUM2>>().SetupData(sum2Data);
            mockContext.Setup(x => x.SUM2).Returns(mockSUM2Set.Object);            

            apData = new List<AP>();
            mockAPSet = new Mock<DbSet<AP>>().SetupData(apData);
            mockContext.Setup(x => x.AP).Returns(mockAPSet.Object);

            arData = new List<AR>();
            mockARSet = new Mock<DbSet<AR>>().SetupData(arData);
            mockContext.Setup(x => x.AR).Returns(mockARSet.Object);

            errorData = new List<Error>();
            mockErrorSet = new Mock<DbSet<Error>>().SetupData(errorData);
            mockContext.Setup(x => x.Errors).Returns(mockErrorSet.Object);

            suppressionData = new List<Suppression>();
            mockSuppressionSet = new Mock<DbSet<Suppression>>().SetupData(suppressionData);
            mockContext.Setup(x => x.Suppressions).Returns(mockSuppressionSet.Object);

            xbData = new List<XB>();
            mockXBSet = new Mock<DbSet<XB>>().SetupData(xbData);
            mockContext.Setup(x => x.XB).Returns(mockXBSet.Object);

            xbDataData = new List<XBData>();
            mockXBDataSet = new Mock<DbSet<XBData>>().SetupData(xbDataData);
            mockContext.Setup(x => x.XBData).Returns(mockXBDataSet.Object);

            configurationService.Setup(x => x.GetValue("Maximum Batch Size")).Returns(10.ToString());

            loadService = new LoadService(mockContext.Object, configurationService.Object, errorService.Object, conversionService, currencyService);
        }

        [Test]
        public void Test_GetClaims_Returns_AP()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            apData.Add(new AP(1234567890, 2016, "S1234567C123456V001"));

            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(1, result.Length);
        }

        [Test]
        public void Test_GetClaims_Returns_AR()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            arData.Add(new AR(1234567890, 2016, "S1234567C123456V001"));

            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(1, result.Length);
        }

        [Test]
        public void Test_GetClaims_Returns_Only_One_Request_Per_FRN_When_AP_And_AR()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            arData.Add(new AR(1234567890, 2016, "S1234567C123456V001"));
            apData.Add(new AP(1234567890, 2016, "S1234567C123456V001"));
            
            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(1, result.Length);
        }

        [Test]
        public void Test_GetClaims_Returns_Only_One_Request_Per_FRN_When_AP_And_AP()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            apData.Add(new AP(1234567890, 2016, "S1234567C123456V001"));
            apData.Add(new AP(1234567890, 2016, "S1234567C123456V001"));
            
            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(1, result.Length);
        }

        [Test]
        public void Test_GetClaims_Returns_Only_One_Request_Per_FRN_When_AR_And_AR()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            arData.Add(new AR(1234567890, 2016, "S1234567C123456V001"));
            arData.Add(new AR(1234567890, 2016, "S1234567C123456V001"));           

            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(1, result.Length);
        }

        [Test]
        public void Test_GetClaims_Returns_Multiple_Requests_When_Multiple_Scheme_Years()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            claimData.Add(new Claim(Guid.Parse("6A0C3FBD-8322-46A6-9096-B255E865DF2A")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sumData.Add(new SUM(Guid.Parse("6A0C3FBD-8322-46A6-9096-B255E865DF2A"), 1234567890, 2015, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            sum2Data.Add(new SUM2(Guid.Parse("6A0C3FBD-8322-46A6-9096-B255E865DF2A"), 0));
            apData.Add(new AP(1234567890, 2016, "S1234567C123456V001"));
            apData.Add(new AP(1234567890, 2015, "S1234567C123456V001"));
            
            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(2, result.Length);
        }

        [Test]
        public void Test_GetClaims_Ignores_Inactive_Scheme_Years_AP()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            apData.Add(new AP(1234567890, 2016, "S1234567C123456V001"));
            schemeYearData.Where(x=>x.SchemeYearNumber == 2016).FirstOrDefault().Active = false;            

            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void Test_GetClaims_Ignores_Inactive_Scheme_Years_AR()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            arData.Add(new AR(1234567890, 2016, "S1234567C123456V001"));
            schemeYearData.Where(x => x.SchemeYearNumber == 2016).FirstOrDefault().Active = false;

            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void Test_GetClaims_Omits_Suppressions_AP()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            apData.Add(new AP(1234567890, 2016, "S1234567C123456V001"));
            suppressionData.Add(new Suppression(1234567890, 2016));            

            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void Test_GetClaims_Omits_Suppressions_AR()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            arData.Add(new AR(1234567890, 2016, "S1234567C123456V001"));
            suppressionData.Add(new Suppression(1234567890, 2016));           

            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void Test_GetClaims_Ignores_Suppressions_For_Other_Scheme_Years_AP()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            apData.Add(new AP(1234567890, 2016, "S1234567C123456V001"));
            suppressionData.Add(new Suppression(1234567890, 2015));            

            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(1, result.Length);
        }

        [Test]
        public void Test_GetClaims_Ignores_Suppressions_For_Other_Scheme_Years_AR()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            arData.Add(new AR(1234567890, 2016, "S1234567C123456V001"));
            suppressionData.Add(new Suppression(1234567890, 2015));
            
            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(1, result.Length);
        }

        [Test]
        public void Test_GetClaims_Ignores_End_Dated_Suppressions_AP()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            apData.Add(new AP(1234567890, 2016, "S1234567C123456V001"));
            
            Suppression suppression = new Suppression(1234567890, 2016);
            suppression.End();

            suppressionData.Add(suppression);

            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(1, result.Length);
        }

        [Test]
        public void Test_GetClaims_Ignores_Ignores_End_Dated_Suppressions_AR()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            arData.Add(new AR(1234567890, 2016, "S1234567C123456V001"));
            
            Suppression suppression = new Suppression(1234567890, 2016);
            suppression.End();

            suppressionData.Add(suppression);

            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(1, result.Length);
        }

        [Test]
        public void Test_GetClaims_Omits_Errors_AP()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            apData.Add(new AP(1234567890, 2016, "S1234567C123456V001"));
            errorData.Add(new Error(1234567890, 2016));
            
            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void Test_GetClaims_Omits_Errors_AR()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            arData.Add(new AR(1234567890, 2016, "S1234567C123456V001"));
            errorData.Add(new Error(1234567890, 2016));
            
            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void Test_GetClaims_Ignores_Errors_For_Other_Scheme_Years_AP()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            apData.Add(new AP(1234567890, 2016, "S1234567C123456V001"));
            errorData.Add(new Error(1234567890, 2015));
            
            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(1, result.Length);
        }

        [Test]
        public void Test_GetClaims_Ignores_Errors_For_Other_Scheme_Years_AR()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            arData.Add(new AR(1234567890, 2016, "S1234567C123456V001"));
            errorData.Add(new Error(1234567890, 2015));            

            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(1, result.Length);
        }

        [Test]
        public void Test_GetClaims_Omits_XB_AP()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            apData.Add(new AP(1234567890, 2016, "S1234567C123456V001"));
            xbData.Add(new XB(1234567890, 2016));
           
            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void Test_GetClaims_Omits_XB_AR()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            arData.Add(new AR(1234567890, 2016, "S1234567C123456V001"));
            xbData.Add(new XB(1234567890, 2016));

            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(0, result.Length);
        }

        [Test]
        public void Test_GetClaims_Ignores_XB_For_Other_Scheme_Years_AP()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            apData.Add(new AP(1234567890, 2016, "S1234567C123456V001"));
            xbData.Add(new XB(1234567890, 2015));
            
            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(1, result.Length);
        }

        [Test]
        public void Test_GetClaims_Ignores_XB_For_Other_Scheme_Years_AR()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            arData.Add(new AR(1234567890, 2016, "S1234567C123456V001"));
            xbData.Add(new XB(1234567890, 2015));
            
            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(1, result.Length);
        }

        [Test]
        public void Test_GetClaims_Ignores_Inactive_XB_AP()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            apData.Add(new AP(1234567890, 2016, "S1234567C123456V001"));
            
            XB xb = new XB(1234567890, 2016);
            xb.Active = false;

            xbData.Add(xb);
            
            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(1, result.Length);
        }

        [Test]
        public void Test_GetClaims_Ignores_Inactive_XB_AR()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            arData.Add(new AR(1234567890, 2016, "S1234567C123456V001"));
            
            XB xb = new XB(1234567890, 2016);
            xb.Active = false;

            xbData.Add(xb);
            
            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(1, result.Length);
        }

        [Test]
        public void Test_GetClaims_Logs_Error_If_No_Claim_Data_AP()
        {
            apData.Add(new AP(1234567890, 2016, "S1234567C123456V001"));
            
            var result = loadService.GetClaims("BPS", 100, false);

            errorService.Verify(x => x.Log(1234567890, 2016, It.IsAny<string>(), null));
        }

        [Test]
        public void Test_GetClaims_Logs_Error_If_No_Claim_Data_AR()
        {
            arData.Add(new AR(1234567890, 2016, "S1234567C123456V001"));
            
            var result = loadService.GetClaims("BPS", 100, false);

            errorService.Verify(x => x.Log(1234567890, 2016, It.IsAny<string>(), null));
        }

        [Test]
        public void Test_GetClaims_Returns_Invoices_AP()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            apData.Add(new AP(1234567890, 2016, "S1234567C123456V001"));
            
            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(1, result[0].Invoices.Count());
        }

        [Test]
        public void Test_GetClaims_Returns_Invoices_AR()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            arData.Add(new AR(1234567890, 2016, "S1234567C123456V001"));
            
            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(1, result[0].Invoices.Count());
        }

        [Test]
        public void Test_GetClaims_Returns_Invoices_AP_And_AR()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            arData.Add(new AR(1234567890, 2016, "S1234567C123456V001"));
            apData.Add(new AP(1234567890, 2016, "S1234567C123456V001"));
            
            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(2, result[0].Invoices.Count());
        }

        [Test]
        public void Test_GetClaims_Returns_Invoices_AP_And_AP()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            apData.AddRange(new List<AP>
            {
                new AP(1234567890, 2016, "S1234567C123456V001"),
                new AP(1234567890, 2016, "S1234567C123456V001")
            });
            
            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(2, result[0].Invoices.Count());
        }

        [Test]
        public void Test_GetClaims_Returns_Invoices_AR_And_AR()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            arData.AddRange(new List<AR>
            {
                new AR(1234567890, 2016, "S1234567C123456V001"),
                new AR(1234567890, 2016, "S1234567C123457V001")
            });

            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(2, result[0].Invoices.Count());
        }

        [Test]
        public void Test_GetClaims_Groups_AR()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            arData.AddRange(new List<AR>
            {
                new AR(1234567890, 2016, "S1234567C123456V001"),
                new AR(1234567890, 2016, "S1234567C123456V001")
            });
            
            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(1, result[0].Invoices.Count());
        }

        [Test]
        public void Test_GetClaims_Populates_ConversionRate()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            apData.Add(new AP(1234567890, 2016, "S1234567C123456V001"));
            
            var result = loadService.GetClaims("BPS", 100, false);

            Assert.AreEqual(1, result[0].Rates.Where(x=>x.Description == "Euro Exchange").FirstOrDefault().Value);
        }

        [Test]
        public void Test_GetClaims_Returns_AP_XB()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            apData.Add(new AP(1234567890, 2016, "S1234567C123456V001"));
            xbData.Add(new XB(1234567890, 2016));
            xbDataData.Add(new XBData(1234567890, 2016, "PFSY1234567", new DateTime(2016, 1, 1)));
        
            var result = loadService.GetClaims("XB", 100, false);

            Assert.AreEqual(1, result.Length);
        }

        [Test]
        public void Test_GetClaims_Returns_AR_XB()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            arData.Add(new AR(1234567890, 2016, "S1234567C123456V001"));
            xbData.Add(new XB(1234567890, 2016));
            xbDataData.Add(new XBData(1234567890, 2016, "PFSY1234567", new DateTime(2016, 1, 1)));
        
            var result = loadService.GetClaims("XB", 100, false);

            Assert.AreEqual(1, result.Length);
        }

        [Test]
        public void Test_GetClaims_Logs_Error_If_No_XBData_AP_XB()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            apData.Add(new AP(1234567890, 2016, "S1234567C123456V001"));
            xbData.Add(new XB(1234567890, 2016));
        
            var result = loadService.GetClaims("XB", 100, false);

            errorService.Verify(x => x.Log(1234567890, 2016, It.IsAny<string>(), It.IsAny<string>()));
        }

        [Test]
        public void Test_GetClaims_Logs_Error_If_No_XBData_AR_XB()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 0));
            arData.Add(new AR(1234567890, 2016, "S1234567C123456V001"));
            xbData.Add(new XB(1234567890, 2016));
            
            var result = loadService.GetClaims("XB", 100, false);

            errorService.Verify(x => x.Log(1234567890, 2016, It.IsAny<string>(), null));
        }

        [Test]
        public void Test_GetClaim_Returns_Claim()
        {
            Claim claim = new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"));

            claimData.Add(claim);
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            
            var result = loadService.GetClaim("S1234567C123456V001", 1234567890, 2016);

            Assert.AreEqual(claim, result);
        }

        [Test]
        public void Test_GetClaim_Returns_Null_If_No_Claim()
        {            
            var result = loadService.GetClaim("S1234567C123456V001", 1234567890, 2016);

            Assert.IsNull(result);
        }

        [Test]
        public void Test_GetXBData_Returns_XBData()
        {
            var xbData = new XBData(1234567890, 2016, "PFSY1234567", new DateTime(2016, 1, 1));

            xbDataData.Add(xbData);

            var result = loadService.GetXBData("S1234567C123456V001", 1234567890, 2016, new DateTime(2016, 1, 1), false);

            Assert.AreEqual(xbData, result);
        }

        [Test]
        public void Test_GetXBData_Returns_Latest_Calculation_Before_Settlement()
        {
            XBData xbData = new XBData(1234567890, 2016, "PFSY1234567", new DateTime(2016, 1, 1));
            XBData xbData2 = new XBData(1234567890, 2016, "PFSY1234567", new DateTime(2016, 1, 2));
            XBData xbData3 = new XBData(1234567890, 2016, "PFSY1234567", new DateTime(2016, 1, 3));

            xbDataData.AddRange(new List<XBData>
            {
                xbData,
                xbData2,
                xbData3
            });
            
            var result = loadService.GetXBData("S1234567C123456V001", 1234567890, 2016, new DateTime(2016, 1, 2), false);

            Assert.AreEqual(xbData2, result);
        }

        [Test]
        public void Test_GetXBData_Returns_Null_If_No_XBData()
        {
            var result = loadService.GetXBData("S1234567C123456V001", 1234567890, 2016, new DateTime(2016, 1, 1), false);

            Assert.IsNull(result);
        }

        [Test]
        public void Test_GetInvoices_Gets_AP()
        {
            apData.Add(new AP(1234567890, 2016, "S1234567C123456V001"));
            
            var result = loadService.GetInvoices(1234567890, 2016, 1, "BPS", false);

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void Test_GetInvoices_Gets_AR()
        {
            arData.Add(new AR(1234567890, 2016, "S1234567C123456V001"));
            
            var result = loadService.GetInvoices(1234567890, 2016, 1, "BPS", false);

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void Test_GetInvoices_Gets_Multiple_AP()
        {
            apData.AddRange(new List<AP>
            {
                new AP(1234567890, 2016, "S1234567C123456V001"),
                new AP(1234567890, 2016, "S1234567C123456V001")
            });
            
            var result = loadService.GetInvoices(1234567890, 2016, 1, "BPS", false);

            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void Test_GetInvoices_Gets_Multiple_AR()
        {
            arData.AddRange(new List<AR>
            {
                new AR(1234567890, 2016, "S1234567C123456V001"),
                new AR(1234567890, 2016, "S1234567C123457V001")
            });
            
            var result = loadService.GetInvoices(1234567890, 2016, 1, "BPS", false);

            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void Test_GetInvoices_Gets_Multiple_AP_And_AR()
        {
            arData.Add(new AR(1234567890, 2016, "S1234567C123456V001"));
            apData.Add(new AP(1234567890, 2016, "S1234567C123456V001"));
            
            var result = loadService.GetInvoices(1234567890, 2016, 1, "BPS", false);

            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void Test_GetInvoices_Groups_AR()
        {
            arData.AddRange(new List<AR>
            {
                new AR(1234567890, 2016, "S1234567C123456V001"),
                new AR(1234567890, 2016, "S1234567C123456V001")
            });
            
            var result = loadService.GetInvoices(1234567890, 2016, 1, "BPS", false);

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void Test_GetClaimValue_Returns_Value()
        {
            claimData.Add(new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")));
            sumData.Add(new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)));
            sum2Data.Add(new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 100));
            
            var result = loadService.GetClaimValue("S1234567C123456V001", 1234567890, 2016, new DateTime(2016, 1, 1), "BPS", false);

            Assert.AreEqual(100, result);
        }

        [Test]
        public void Test_GetClaimValue_Returns_Value_Multiple_Claims()
        {
            claimData.AddRange(new List<Claim>
            {
                new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")),
                new Claim(Guid.Parse("D379C9B4-45B5-4720-B772-EC50B3078DFF"))
            });
            
            sumData.AddRange(new List<SUM>
            {
                new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)),
                new SUM(Guid.Parse("D379C9B4-45B5-4720-B772-EC50B3078DFF"), 1234567890, 2016, "SITI1234568", new DateTime(2016, 1, 1))
            });
            
            sum2Data.AddRange(new List<SUM2>
            {
                new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 100),
                new SUM2(Guid.Parse("D379C9B4-45B5-4720-B772-EC50B3078DFF"), 150)
            });
            
            var result = loadService.GetClaimValue("S1234567C123456V001", 1234567890, 2016, new DateTime(2016, 1, 1), "BPS", false);

            Assert.AreEqual(100, result);
        }

        [Test]
        public void Test_GetClaimValue_Returns_Value_Multiple_Claims_Same_Invoice()
        {
            claimData.AddRange(new List<Claim>
            {
                new Claim(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D")),
                new Claim(Guid.Parse("D379C9B4-45B5-4720-B772-EC50B3078DFF"))
            });            
            sumData.AddRange(new List<SUM>
            {
                new SUM(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 1)),
                new SUM(Guid.Parse("D379C9B4-45B5-4720-B772-EC50B3078DFF"), 1234567890, 2016, "SITI1234567", new DateTime(2016, 1, 2))
            });
            sum2Data.AddRange(new List<SUM2>
            {
                new SUM2(Guid.Parse("DFB2E833-5753-40D3-AA68-1CB723593C3D"), 100),
                new SUM2(Guid.Parse("D379C9B4-45B5-4720-B772-EC50B3078DFF"), 150)
            });
            
            var result = loadService.GetClaimValue("S1234567C123456V001", 1234567890, 2016, new DateTime(2016, 1, 1), "BPS", false);

            Assert.AreEqual(100, result);
        }

        [Test]
        public void Test_GetClaimValue_Returns_Value_XB()
        {
            xbDataData.Add(new XBData(1234567890, 2016, "PFSY1234567", new DateTime(2016, 1, 1), 100, 100, 100, 100));
            
            var result = loadService.GetClaimValue("S1234567C123456V001", 1234567890, 2016, new DateTime(2016, 1, 1), "XB", false);

            Assert.AreEqual(400, result);
        }

        [Test]
        public void Test_GetClaimValue_Returns_Value_Multiple_Claims_XB()
        {
            xbDataData.AddRange(new List<XBData>
            {
                new XBData(1234567890, 2016, "PFSY1234567", new DateTime(2016, 1, 1), 100, 100, 100, 100),
                new XBData(1234567890, 2016, "PFSY1234568", new DateTime(2016, 1, 1), 200, 200, 200, 200)
            });
                        
            var result = loadService.GetClaimValue("S1234567C123456V001", 1234567890, 2016, new DateTime(2016, 1, 1), "XB", false);

            Assert.AreEqual(400, result);
        }

        [Test]
        public void Test_GetClaimValue_Returns_Value_Multiple_Claims_Same_Invoice_XB()
        {
            xbDataData.AddRange(new List<XBData>
            {
                new XBData(1234567890, 2016, "PFSY1234567", new DateTime(2016, 1, 1), 100, 100, 100, 100),
                new XBData(1234567890, 2016, "PFSY1234567", new DateTime(2016, 1, 2), 200, 200, 200, 200)
            });
                        
            var result = loadService.GetClaimValue("S1234567C123456V001", 1234567890, 2016, new DateTime(2016, 1, 1), "XB", false);

            Assert.AreEqual(400, result);
        }

        [Test]
        public void Test_GetAR_Returns_AR()
        {
            arData.Add(new AR(1234567890, 2016, "S1234567C123456V001"));

            var result = loadService.GetAR(1234567890, 2016);

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void Test_GetAR_Returns_Multiple_AR()
        {
            arData.AddRange(new List<AR>
            {
                new AR(1234567890, 2016, "S1234567C123456V001"),
                new AR(1234567890, 2016, "S1234568C123456V001")
            });
            
            var result = loadService.GetAR(1234567890, 2016);

            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public void Test_GetAR_Groups_AR()
        {
            arData.AddRange(new List<AR>
            {
                new AR(1234567890, 2016, "S1234567C123456V001"),
                new AR(1234567890, 2016, "S1234567C123456V001")
            });
           
            var result = loadService.GetAR(1234567890, 2016);

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void Test_GetAR_Returns_Only_Matching_Scheme_Year()
        {
            arData.AddRange(new List<AR>
            {
                new AR(1234567890, 2016, "S1234567C123456V001"),
                new AR(1234567890, 2015, "S1234567C123456V001")
            });
            
            var result = loadService.GetAR(1234567890, 2016);

            Assert.AreEqual(1, result.Count);
        }
        
        [Test]
        public void Test_GetTriggers_Returns_Empty_List_If_No_Data()
        {
            var result = loadService.GetTriggers("BPS");

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void Test_BuildSkips_Includes_Errors()
        {
            List<Error> errors = new List<Error>
            {
                new Error(1234567890, 2016)
            };

            var result = loadService.BuildSkips(errors, new List<Suppression>(), new List<XB>(), "BPS");

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void Test_BuildSkips_Includes_Suppressions()
        {            
            List<Suppression> suppressions = new List<Suppression>
            {
                new Suppression(1234567890, 2016)
            };

            var result = loadService.BuildSkips(new List<Error>(), suppressions, new List<XB>(), "BPS");

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void Test_BuildSkips_Includes_XB_For_BPS()
        {
            List<XB> xbList = new List<XB>
            {
                new XB(1234567890, 2016)
            };

            var result = loadService.BuildSkips(new List<Error>(), new List<Suppression>(), xbList, "BPS");

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void Test_BuildSkips_Does_Not_Include_XB_For_XB()
        {
            List<XB> xbList = new List<XB>
            {
                new XB(1234567890, 2016)
            };

            var result = loadService.BuildSkips(new List<Error>(), new List<Suppression>(), xbList, "XB");

            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void Test_RefineMaximumBatchSize_Returns_Original_If_Valid()
        {
            configurationService.Setup(x => x.GetValue("Maximum Batch Size")).Returns(1000.ToString());
                        
            LoadService loadService = new LoadService(mockContext.Object, configurationService.Object, errorService.Object, conversionService, currencyService);

            var result = loadService.RefineMaximumBatchSize(50, 100);

            Assert.AreEqual(50, result);
        }

        [Test]
        public void Test_RefineMaximumBatchSize_Returns_Master_If_Master_Is_Lower()
        {   
            configurationService.Setup(x => x.GetValue("Maximum Batch Size")).Returns(20.ToString());
            
            LoadService loadService = new LoadService(mockContext.Object, configurationService.Object, errorService.Object, conversionService, currencyService);

            var result = loadService.RefineMaximumBatchSize(50, 100);

            Assert.AreEqual(20, result);
        }

        [Test]
        public void Test_RefineMaximumBatchSize_Returns_TotalDAX_If_TotalDAX_Is_Lower()
        {
            configurationService.Setup(x => x.GetValue("Maximum Batch Size")).Returns(1000.ToString());
            
            LoadService loadService = new LoadService(mockContext.Object, configurationService.Object, errorService.Object, conversionService, currencyService);

            var result = loadService.RefineMaximumBatchSize(50, 20);

            Assert.AreEqual(20, result);
        }

        [Test]
        public void Test_OrderDAX_Does_Not_Order_If_Total_Less_Than_Maximum()
        {
            List<DAXOutstanding> daxOutstanding = new List<DAXOutstanding>
            {
                new DAXOutstanding("Invoice 1", 1234567890, 2016, new DateTime(2016, 1, 5)),
                new DAXOutstanding("Invoice 2", 1234567890, 2016, new DateTime(2016, 1, 1)),
            };

            var result = loadService.OrderDAX(daxOutstanding, 10);

            Assert.AreEqual("Invoice 1", result[0].Invoice);
        }

        [Test]
        public void Test_OrderDAX_Does_Not_Order_If_Total_Equals_Maximum()
        {
            List<DAXOutstanding> daxOutstanding = new List<DAXOutstanding>
            {
                new DAXOutstanding("Invoice 1", 1234567890, 2016, new DateTime(2016, 1, 5)),
                new DAXOutstanding("Invoice 2", 1234567890, 2016, new DateTime(2016, 1, 1)),
            };

            var result = loadService.OrderDAX(daxOutstanding, 2);

            Assert.AreEqual("Invoice 1", result[0].Invoice);
        }

        [Test]
        public void Test_OrderDAX_Orders_If_Total_Greater_Than_Maximum()
        {
            List<DAXOutstanding> daxOutstanding = new List<DAXOutstanding>
            {
                new DAXOutstanding("Invoice 1", 1234567890, 2016, new DateTime(2016, 1, 5)),
                new DAXOutstanding("Invoice 2", 1234567890, 2016, new DateTime(2016, 1, 1)),
            };

            var result = loadService.OrderDAX(daxOutstanding, 1);

            Assert.AreEqual("Invoice 2", result[0].Invoice);
        }

        [Test]
        public void Test_OrderDAX_Does_Not_Error_If_Only_One_DAX()
        {
            List<DAXOutstanding> daxOutstanding = new List<DAXOutstanding>
            {
                new DAXOutstanding("Invoice 1", 1234567890, 2016, new DateTime(2016, 1, 5))
            };

            Assert.DoesNotThrow(() => loadService.OrderDAX(daxOutstanding, 0));
        }

        [Test]
        public void Test_OrderDAX_Does_Not_Error_If_No_DAX_Zero_Maximum()
        {
            List<DAXOutstanding> daxOutstanding = new List<DAXOutstanding>();

            Assert.DoesNotThrow(() => loadService.OrderDAX(daxOutstanding, 0));
        }

        [Test]
        public void Test_OrderDAX_Does_Not_Error_If_No_DAX_With_Maximum()
        {
            List<DAXOutstanding> daxOutstanding = new List<DAXOutstanding>();

            Assert.DoesNotThrow(() => loadService.OrderDAX(daxOutstanding, 1));
        }
    }
}
