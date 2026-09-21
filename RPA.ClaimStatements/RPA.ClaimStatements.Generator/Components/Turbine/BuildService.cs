using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPA.ClaimStatements.Generator.Models.ClaimStatements;
using RPA.ClaimStatements.Generator.Models.Generation;

namespace RPA.ClaimStatements.Generator.Components.Turbine
{
    public class BuildService : IBuildService
    {
        public ClaimStatement Build(Request request)
        {
            Summary summary = new Summary();
            summary.Build(request.Claim.SUM);

            decimal conversionRate = request.Rates.Where(x => x.Description == "Euro Exchange").Select(x => x.Value).FirstOrDefault();

            PartA partA = new PartA();
            partA.Build(request.Claim, conversionRate);

            PartB partB;

            BPSPayment bpsPayment = new BPSPayment();
            bpsPayment.Build(request.Claim.SUM2, request.Claim.BPS, request.Claim.BPSPEN, request.Claim.SUM.SchemeYear);

            GreeningPayment greeningPayment = new GreeningPayment();
            greeningPayment.Build(request.Claim.SUM2, request.Claim.GR, request.Claim.GRPEN);

            

            if (request.Claim.SUM2.YoungFarmerValue > 0 && request.Claim.PR?.PRBTotalResult > 0)
            {
                ProgressiveReductionsReduction progressiveReductionsReduction = new ProgressiveReductionsReduction();
                progressiveReductionsReduction.Build(request.Claim.SUM2, request.Claim.PR);

                YoungFarmerPayment youngFarmerPayment = new YoungFarmerPayment();
                youngFarmerPayment.Build(request.Claim.SUM2, request.Claim.YF, request.Claim.SUM.SchemeYear);

                partB = new PartB(bpsPayment, greeningPayment, progressiveReductionsReduction, youngFarmerPayment);
            }
            else if (request.Claim.SUM2.YoungFarmerValue > 0)
            {
                YoungFarmerPayment youngFarmerPayment = new YoungFarmerPayment();
                youngFarmerPayment.Build(request.Claim.SUM2, request.Claim.YF, request.Claim.SUM.SchemeYear);

                partB = new PartB(bpsPayment, greeningPayment, youngFarmerPayment);
            }
            else if (request.Claim.PR?.PRBTotalResult > 0)
            {
                ProgressiveReductionsReduction progressiveReductionsReduction = new ProgressiveReductionsReduction();
                progressiveReductionsReduction.Build(request.Claim.SUM2, request.Claim.PR);
                partB = new PartB(bpsPayment, greeningPayment, progressiveReductionsReduction);
            }
            else
            {
                partB = new PartB(bpsPayment, greeningPayment);
            }

            PartC partC = new PartC();
            partC.Build(request.Invoices);

            PartD partD = new PartD();
            partD.Build(request.Claim.CLD);

            ClaimStatement statement = new ClaimStatement(request.Claim.ClaimID, summary, partA, partB, partC, partD, request.Rates);
            statement.Build();

            return statement;
        }

        public ClaimStatement BuildXB(Request request)
        {
            List<string> countries = new List<string>();

            if (request.XBData.EnglandBPSBPSGross > 0)
            {
                countries.Add("England");
            }
            if (request.XBData.WalesBPSRegion1Total > 0)
            {
                countries.Add("Wales");
            }
            if (request.XBData.ScotlandBPSGross > 0)
            {
                countries.Add("Scotland");
            }
            if (request.XBData.NIBPSRegion1Total > 0)
            {
                countries.Add("NI");
            }

            Summary summaryXB = new Summary();
            summaryXB.Build(request.Claim.SUM);

            decimal conversionRate = request.Rates.Where(x => x.Description == "Euro Exchange").Select(x => x.Value).FirstOrDefault();

            PartAXB partAXB = new PartAXB();
            partAXB.Build(request.Claim, request.XBData, conversionRate);

            PartBXB partBXB = new PartBXB();

            if (countries.Exists(x => x == "England"))
            {
                BPSPayment englandBPSPayment = new BPSPayment();
                englandBPSPayment.Build(request.XBData);
                partBXB.BPSPayment = englandBPSPayment;


                GreeningPayment englandGreeningPayment = new GreeningPayment();
                englandGreeningPayment.Build(request.XBData);
                partBXB.GreeningPayment = englandGreeningPayment;

                if (request.XBData.EnglandYFClaimValue > 0)
                {
                    YoungFarmerPayment englandYoungFarmerPayment = new YoungFarmerPayment();
                    englandYoungFarmerPayment.Build(request.XBData);
                    partBXB.YoungFarmerPayment = englandYoungFarmerPayment;
                }
            }

            if (countries.Exists(x => x == "Wales"))
            {
                WalesBPSPayment walesBPSPayment = new WalesBPSPayment();
                walesBPSPayment.Build(request.XBData);
                partBXB.WalesBPSPayment = walesBPSPayment;


                WalesGreeningPayment walesGreeningPayment = new WalesGreeningPayment();
                walesGreeningPayment.Build(request.XBData);
                partBXB.WalesGreeningPayment = walesGreeningPayment;

                if (request.XBData.WalesYFClaimValue > 0)
                {
                    WalesYoungFarmerPayment walesYoungFarmerPayment = new WalesYoungFarmerPayment();
                    walesYoungFarmerPayment.Build(request.XBData);
                    partBXB.WalesYoungFarmerPayment = walesYoungFarmerPayment;
                }
                if (request.XBData.WalesRedClaimValue > 0)
                {
                    WalesRedistributivePayment redistributivePayment = new WalesRedistributivePayment();
                    redistributivePayment.Build(request.XBData);
                    partBXB.WalesRedistributivePayment = redistributivePayment;
                }
            }

            if (countries.Exists(x => x == "Scotland"))
            {
                ScotlandBPSPayment scotlandBPSPayment = new ScotlandBPSPayment();
                scotlandBPSPayment.Build(request.XBData);
                partBXB.ScotlandBPSPayment = scotlandBPSPayment;


                ScotlandGreeningPayment scotlandGreeningPayment = new ScotlandGreeningPayment();
                scotlandGreeningPayment.Build(request.XBData);
                partBXB.ScotlandGreeningPayment = scotlandGreeningPayment;

                if (request.XBData.ScotlandYFClaimValue > 0)
                {
                    ScotlandYoungFarmerPayment scotlandYoungFarmerPayment = new ScotlandYoungFarmerPayment();
                    scotlandYoungFarmerPayment.Build(request.XBData);
                    partBXB.ScotlandYoungFarmerPayment = scotlandYoungFarmerPayment;
                }
            }

            if (countries.Exists(x => x == "NI"))
            {
                NIBPSPayment niBPSPayment = new NIBPSPayment();
                niBPSPayment.Build(request.XBData);
                partBXB.NIBPSPayment = niBPSPayment;


                NIGreeningPayment niGreeningPayment = new NIGreeningPayment();
                niGreeningPayment.Build(request.XBData);
                partBXB.NIGreeningPayment = niGreeningPayment;

                if (request.XBData.NIYFClaimValue > 0)
                {
                    NIYoungFarmerPayment niYoungFarmerPayment = new NIYoungFarmerPayment();
                    niYoungFarmerPayment.Build(request.XBData);
                    partBXB.NIYoungFarmerPayment = niYoungFarmerPayment;
                }
            }

            PartC partCXB = new PartC();
            partCXB.Build(request.Invoices);

            PartDXB partDXB = new PartDXB();
            partDXB.Build(request.XBData);

            ClaimStatementXB statement = new ClaimStatementXB(request.Claim.ClaimID, summaryXB, partAXB, partBXB, partCXB, partDXB, request.Rates);
            statement.Build(countries);

            return statement;
        }
    }
}
