using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPA.ClaimStatements.Generator.Models.ClaimStatements;
using RPA.ClaimStatements.Generator.Models.Generation;

namespace RPA.ClaimStatements.Generator.Services
{
    public class BuildService : IBuildService
    {
        public virtual ClaimStatement Build(Request request)
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

            if (request.Claim.SUM2.YoungFarmerValue > 0)
            {
                YoungFarmerPayment youngFarmerPayment = new YoungFarmerPayment();
                youngFarmerPayment.Build(request.Claim.SUM2, request.Claim.YF, request.Claim.SUM.SchemeYear);

                partB = new PartB(bpsPayment, greeningPayment, youngFarmerPayment);
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
    }
}
