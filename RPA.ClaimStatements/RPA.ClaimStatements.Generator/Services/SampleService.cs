using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPA.ClaimStatements.Generator.Models.ClaimStatements;
using FizzWare.NBuilder;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Models.Entities.SITI;
using System.Xml;
using RPA.ClaimStatements.Generator.Components.Turbine;

namespace RPA.ClaimStatements.Generator.Services
{
    public class SampleService : ISampleService
    {
        IClaimStatementsContext db;
        IFileService fileService;
        ITransformService transformService;
        
        public SampleService(IClaimStatementsContext context, IFileService fileService, ITransformService transformService)
        {
            db = context;
            this.fileService = fileService;
            this.transformService = transformService;
        }

        public string Sample(int? schemeYear = null)
        {
            string docxFilePath = null;

            if(schemeYear == null)
            {
                schemeYear = db.SchemeYears.AsNoTracking().Where(x => x.Active).OrderByDescending(x => x.SchemeYearNumber).Select(x=>x.SchemeYearNumber).FirstOrDefault();
            }

            ClaimStatement statement = Builder<ClaimStatement>.CreateNew().With(x => x.Currency = "Euros").Build();

            Summary summary = Builder<Summary>.CreateNew().With(x => x.SchemeYear = schemeYear.Value).Build();

            PartA partA = Builder<PartA>.CreateNew().Build();

            PartB partB = Builder<PartB>.CreateNew().Build();

            BPSPayment bps = Builder<BPSPayment>.CreateNew().With(x => x.Regions = 3).Build();
            GreeningPayment greening = Builder<GreeningPayment>.CreateNew().With(x => x.Regions = 3).Build();
            YoungFarmerPayment yf = Builder<YoungFarmerPayment>.CreateNew().With(x => x.Regions = 3).Build();
            ProgressiveReductionsReduction pr = Builder<ProgressiveReductionsReduction>.CreateNew().Build();

            partB.BPSPayment = bps;
            partB.GreeningPayment = greening;
            partB.YoungFarmerPayment = yf;
            partB.ProgressiveReductions = pr;

            PartC partC = Builder<PartC>.CreateNew().Build();
            List<Invoice> invoices = Builder<Invoice>.CreateListOfSize(1).Build().ToList();

            partC.Invoices = invoices;

            PartD partD = Builder<PartD>.CreateNew().Build();

            List<Rate> rates = new List<Rate>();

            var conversionRates = db.ConversionRates.AsNoTracking().Where(x => x.SchemeYear == schemeYear);

            foreach (var cRate in conversionRates)
            {
                rates.Add(new Rate(cRate.Description, cRate.Rate));
            }

            statement.Summary = summary;
            statement.PartA = partA;
            statement.PartB = partB;
            statement.PartC = partC;
            statement.PartD = partD;
            statement.Rates = rates;

            if (statement != null)
            {
                Claim claim = Builder<Claim>.CreateNew().Build();
                SUM sum = Builder<SUM>.CreateNew().Build();
                claim.SUM = sum;

                string xmlFilePath = transformService.ConvertToXML(statement, "BPS");

                Transformation transformation = db.Transformations.Where(x => x.StatementType == "BPS").FirstOrDefault();

                XmlDocument newXml = transformService.TransformXML(xmlFilePath, transformation.XSLT);

                docxFilePath = transformService.CopyTemplate(claim.ClaimID, transformation.Template);

                transformService.CreateDOCX(docxFilePath, newXml, claim.SUM.SBI, "BPS");

                fileService.Delete(xmlFilePath);
            }

            return docxFilePath;
        }
    }
}
