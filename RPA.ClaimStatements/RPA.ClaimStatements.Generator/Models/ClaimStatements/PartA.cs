using RPA.ClaimStatements.Generator.Models.Entities.SITI;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Models.ClaimStatements
{
    public class PartA
    {
        protected ICurrencyService currencyService;

        public string BusinessName { get; set; }

        public string Address1 { get; set; }

        public int SBI { get; set; }

        public Int64 FRN { get; set; }

        public int SchemeYear { get; set; }

        public string ApplicationID { get; set; }

        public string DateOfCalculation { get; set; }

        public string CalculationReferenceNumber { get; set; }

        public decimal BPSValue { get; set; }

        public decimal GreeningValue { get; set; }

        public decimal YoungFarmerValue { get; set; }

        public decimal SubTotal { get; set; }

        public decimal CrossCompliancePercent { get; set; }

        public decimal CrossComplianceReduction { get; set; }

        public decimal CrossComplianceTotal { get; set; }

        public decimal ProgressiveReduction { get; set; }

        public decimal ProgressiveReductionTotal { get; set; }

        public decimal TotalEuro { get; set; }

        public decimal TotalSterling { get; set; }

        public PartA()
        {
            this.currencyService = new CurrencyService();
        }

        public PartA(ICurrencyService currencyService)
        {
            this.currencyService = currencyService;
        }

        public virtual void Build(Claim claim, decimal conversionRate)
        {
            BusinessName = claim.SUM.BusinessName;
            Address1 = claim.SUM.AddressLine1;
            SBI = claim.SUM.SBI;
            FRN = claim.SUM.FRN;
            SchemeYear = claim.SUM.SchemeYear;
            ApplicationID = claim.SUM.ApplicationID;
            DateOfCalculation = claim.SUM.CalculationDate.ToString("dd MMMM yyyy");
            CalculationReferenceNumber = claim.SUM.CalculationRefNumber;
            BPSValue = claim.BPSPEN.TotalBPS;
            GreeningValue = claim.GRPEN.TotalGreening;
            SubTotal = claim.SUM2.SubTotal;
            CrossCompliancePercent = claim.SUM2.CrossCompliancePercent;
            CrossComplianceReduction = claim.SUM2.CrossComplianceReduction;

            if (claim.PR != null)
            {
                ProgressiveReduction = claim.PR.PRBTotalResult;
            }

            CrossComplianceTotal = SubTotal - CrossComplianceReduction;
            if (claim.PR != null)
            {
                ProgressiveReductionTotal = CrossComplianceTotal - ProgressiveReduction;
            }

            TotalEuro = claim.SUM2.TotalClaimEuro;
            TotalSterling = currencyService.Convert(TotalEuro, conversionRate);

            if (claim.YF != null)
            {
                YoungFarmerValue = claim.YF.TotalYF;
            }
        }
    }
}
