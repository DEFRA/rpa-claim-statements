using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPA.ClaimStatements.Generator.Models.Entities.SITI;
using RPA.ClaimStatements.Generator.Models.Entities.XB;
using RPA.ClaimStatements.Generator.Services;

namespace RPA.ClaimStatements.Generator.Models.ClaimStatements
{
    public class PartAXB : PartA
    {
        public decimal WalesBPSValue { get; set; }

        public decimal WalesGreeningValue { get; set; }

        public decimal WalesYoungFarmerValue { get; set; }

        public decimal WalesRedistributiveValue { get; set; }

        public decimal WalesSubTotal { get; set; }

        public decimal WalesCrossCompliancePercent { get; set; }

        public decimal WalesCrossComplianceReduction { get; set; }

        public decimal WalesCrossComplianceTotal { get; set; }

        public decimal WalesTotalEuro { get; set; }

        public decimal WalesTotalSterling { get; set; }

        public decimal ScotlandBPSValue { get; set; }

        public decimal ScotlandGreeningValue { get; set; }

        public decimal ScotlandYoungFarmerValue { get; set; }

        public decimal ScotlandSubTotal { get; set; }

        public decimal ScotlandCrossCompliancePercent { get; set; }

        public decimal ScotlandCrossComplianceReduction { get; set; }

        public decimal ScotlandCrossComplianceTotal { get; set; }

        public decimal ScotlandTotalEuro { get; set; }

        public decimal ScotlandTotalSterling { get; set; }

        public decimal NIBPSValue { get; set; }

        public decimal NIGreeningValue { get; set; }

        public decimal NIYoungFarmerValue { get; set; }

        public decimal NISubTotal { get; set; }

        public decimal NICrossCompliancePercent { get; set; }

        public decimal NICrossComplianceReduction { get; set; }

        public decimal NICrossComplianceTotal { get; set; }

        public decimal NITotalEuro { get; set; }

        public decimal NITotalSterling { get; set; }

        public decimal TotalTotalEuro { get; set; }

        public decimal TotalTotalSterling { get; set; }

        public PartAXB()
        {
            currencyService = new CurrencyService();
        }

        public PartAXB(ICurrencyService currencyService)
        {
            this.currencyService = currencyService;
        }

        public void Build(Claim claim, XBData xbData, decimal conversionRate)
        {
            SBI = claim.SUM.SBI;
            FRN = claim.SUM.FRN;
            SchemeYear = claim.SUM.SchemeYear;
            ApplicationID = claim.SUM.ApplicationID;
            DateOfCalculation = xbData.CalculationDate.ToString("dd MMMM yyyy");
            CalculationReferenceNumber = claim.SUM.CalculationRefNumber;
            BPSValue = xbData.EnglandBPSTotalBPSPayment;
            GreeningValue = xbData.EnglandGreeningTotalGreeningPayment;
            YoungFarmerValue = xbData.EnglandYFTotalYoungFarmerPayment;
            SubTotal = xbData.EnglandSubTotal;
            CrossCompliancePercent = xbData.EnglandCrossCompliancePercent;
            CrossComplianceReduction = xbData.EnglandCrossComplianceReduction;
            CrossComplianceTotal = SubTotal - CrossComplianceReduction;
            TotalEuro = xbData.EnglandTotalEuro;
            WalesBPSValue = xbData.WalesBPSTotalBPSPayment;
            WalesGreeningValue = xbData.WalesGreeningTotalGreeningPayment;
            WalesYoungFarmerValue = xbData.WalesYFTotalYoungFarmerPayment;
            WalesRedistributiveValue = xbData.WalesRedTotalRedistributionPayment;
            WalesSubTotal = xbData.WalesSubTotal;
            WalesCrossCompliancePercent = xbData.WalesCrossCompliancePercent;
            WalesCrossComplianceReduction = xbData.WalesCrossComplianceReduction;
            WalesCrossComplianceTotal = WalesSubTotal - WalesCrossComplianceReduction;
            WalesTotalEuro = xbData.WalesTotalEuro;
            ScotlandBPSValue = xbData.ScotlandBPSTotalBPSPayment;
            ScotlandGreeningValue = xbData.ScotlandGreeningTotalGreeningPayment;
            ScotlandYoungFarmerValue = xbData.ScotlandYFTotalYoungFarmerPayment;
            ScotlandSubTotal = xbData.ScotlandSubTotal;
            ScotlandCrossCompliancePercent = xbData.ScotlandCrossCompliancePercent;
            ScotlandCrossComplianceReduction = xbData.ScotlandCrossComplianceReduction;
            ScotlandCrossComplianceTotal = ScotlandSubTotal - ScotlandCrossComplianceReduction;
            ScotlandTotalEuro = xbData.ScotlandTotalEuro;
            NIBPSValue = xbData.NIBPSTotalBPSPayment;
            NIGreeningValue = xbData.NIGreeningTotalGreeningPayment;
            NIYoungFarmerValue = xbData.NIYFTotalYoungFarmerPayment;
            NISubTotal = xbData.NISubTotal;
            NICrossCompliancePercent = xbData.NICrossCompliancePercent;
            NICrossComplianceReduction = xbData.NICrossComplianceReduction;
            NICrossComplianceTotal = NISubTotal - NICrossComplianceReduction;
            NITotalEuro = xbData.NITotalEuro;
            
            TotalSterling = currencyService.Convert(TotalEuro, conversionRate);
            WalesTotalSterling = currencyService.Convert(WalesTotalEuro, conversionRate);
            ScotlandTotalSterling = currencyService.Convert(ScotlandTotalEuro, conversionRate);
            NITotalSterling = currencyService.Convert(NITotalEuro, conversionRate);                

            TotalTotalEuro = TotalEuro + WalesTotalEuro + ScotlandTotalEuro + NITotalEuro;
            TotalTotalSterling = currencyService.Convert(TotalTotalEuro, conversionRate);
        }
    }
}
