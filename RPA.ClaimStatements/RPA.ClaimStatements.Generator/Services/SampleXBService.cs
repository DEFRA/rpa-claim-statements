using FizzWare.NBuilder;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Models.Entities.SITI;
using RPA.ClaimStatements.Generator.Models.ClaimStatements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using RPA.ClaimStatements.Generator.Components.Turbine;

namespace RPA.ClaimStatements.Generator.Services
{
    public class SampleXBService : ISampleXBService
    { 
        IClaimStatementsContext db;
        IFileService fileService;
        ITransformService transformService;

        public SampleXBService(IClaimStatementsContext context, IFileService fileService, ITransformService transformService)
        {
            db = context;
            this.fileService = fileService;
            this.transformService = transformService;
        }

        public string Sample(int? schemeYear = null)
        {
            string docxFilePath = null;

            if (schemeYear == null)
            {
                schemeYear = db.SchemeYears.AsNoTracking().Where(x => x.Active).OrderByDescending(x => x.SchemeYearNumber).Select(x => x.SchemeYearNumber).FirstOrDefault();
            }

            ClaimStatementXB statement = Builder<ClaimStatementXB>.CreateNew().With(x => x.England = true).And(x => x.NI = true).And(x => x.Wales = true).And(x => x.Scotland = true).And(x => x.Currency = "Euros").Build();

            Summary summaryXB = Builder<Summary>.CreateNew().With(x => x.SchemeYear = schemeYear.Value).Build();

            PartAXB partAXB = Builder<PartAXB>.CreateNew().Build();

            PartBXB partBXB = Builder<PartBXB>.CreateNew().Build();

            BPSPayment bpsXB = Builder<BPSPayment>.CreateNew().With(x => x.Regions = 3).Build();
            GreeningPayment greeningXB = Builder<GreeningPayment>.CreateNew().With(x => x.Regions = 3).And(x => x.SDANumber = 0).And(x => x.SDATotal = 0).And(x => x.MoorlandNumber = 0).And(x => x.MoorlandTotal = 0).Build();
            YoungFarmerPayment yfXB = Builder<YoungFarmerPayment>.CreateNew().Build();

            NIBPSPayment niBps = Builder<NIBPSPayment>.CreateNew().Build();
            NIGreeningPayment niGreening = Builder<NIGreeningPayment>.CreateNew().Build();
            NIYoungFarmerPayment niYf = Builder<NIYoungFarmerPayment>.CreateNew().Build();

            ScotlandBPSPayment scotlandBps = Builder<ScotlandBPSPayment>.CreateNew().With(x => x.Regions = 3).Build();
            ScotlandGreeningPayment scotlandGreening = Builder<ScotlandGreeningPayment>.CreateNew().With(x => x.Regions = 3).And(x => x.Region2Number = 0).And(x => x.Region2Total = 0).And(x => x.Region3Number = 0).And(x => x.Region3Total = 0).Build();
            ScotlandYoungFarmerPayment scotlandYf = Builder<ScotlandYoungFarmerPayment>.CreateNew().Build();

            WalesBPSPayment walesBps = Builder<WalesBPSPayment>.CreateNew().Build();
            WalesGreeningPayment walesGreening = Builder<WalesGreeningPayment>.CreateNew().Build();
            WalesYoungFarmerPayment walesYf = Builder<WalesYoungFarmerPayment>.CreateNew().Build();
            WalesRedistributivePayment walesRed = Builder<WalesRedistributivePayment>.CreateNew().Build();

            partBXB.BPSPayment = bpsXB;
            partBXB.GreeningPayment = greeningXB;
            partBXB.YoungFarmerPayment = yfXB;

            partBXB.NIBPSPayment = niBps;
            partBXB.NIGreeningPayment = niGreening;
            partBXB.NIYoungFarmerPayment = niYf;

            partBXB.ScotlandBPSPayment = scotlandBps;
            partBXB.ScotlandGreeningPayment = scotlandGreening;
            partBXB.ScotlandYoungFarmerPayment = scotlandYf;

            partBXB.WalesBPSPayment = walesBps;
            partBXB.WalesGreeningPayment = walesGreening;
            partBXB.WalesYoungFarmerPayment = walesYf;
            partBXB.WalesRedistributivePayment = walesRed;

            PartC partCXB = Builder<PartC>.CreateNew().Build();
            List<Invoice> invoicesXB = Builder<Invoice>.CreateListOfSize(1).Build().ToList();

            partCXB.Invoices = invoicesXB;

            PartDXB partDXB = Builder<PartDXB>.CreateNew().Build();

            statement.Summary = summaryXB;
            statement.PartAXB = partAXB;
            statement.PartBXB = partBXB;
            statement.PartC = partCXB;
            statement.PartDXB = partDXB;
            

            Claim claimXB = Builder<Claim>.CreateNew().Build();
            SUM sumXB = Builder<SUM>.CreateNew().Build();

            claimXB.SUM = sumXB;

            if (statement != null)
            {
                Claim claim = Builder<Claim>.CreateNew().Build();
                SUM sum = Builder<SUM>.CreateNew().Build();
                claim.SUM = sum;

                string xmlFilePath = transformService.ConvertToXML(statement, "XB");

                Transformation transformation = db.Transformations.Where(x => x.StatementType == "XB").FirstOrDefault();

                XmlDocument newXml = transformService.TransformXML(xmlFilePath, transformation.XSLT);

                docxFilePath = transformService.CopyTemplate(claim.ClaimID, transformation.Template);

                transformService.CreateDOCX(docxFilePath, newXml, claim.SUM.SBI, "XB");

                fileService.Delete(xmlFilePath);
            }

            return docxFilePath;
        }
    }
}
