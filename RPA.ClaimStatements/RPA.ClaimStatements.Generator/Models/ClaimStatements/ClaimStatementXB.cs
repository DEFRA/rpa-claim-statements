using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RPA.ClaimStatements.Generator.Models.ClaimStatements
{
    [Serializable]
    public class ClaimStatementXB: ClaimStatement
    {
        public PartAXB PartAXB { get; set; }

        public PartBXB PartBXB { get; set; }
        
        public PartDXB PartDXB { get; set; }      

        public bool England { get; set; }

        public bool Wales { get; set; }

        public bool Scotland { get; set; }

        public bool NI { get; set; }
        
        public ClaimStatementXB()
        {
            currencyService = new CurrencyService();
        }

        public ClaimStatementXB(Guid claimId):this()
        {
            ClaimID = claimId;

        }

        public ClaimStatementXB(Guid claimId, Summary summary, PartAXB partA, PartBXB partB, PartC partC, PartDXB partD, List<Rate> rates) : this(claimId)
        {
            Summary = summary;
            PartAXB = partA;
            PartBXB = partB;
            PartC = partC;
            PartDXB = partD;
            Rates = rates;
        }

        public ClaimStatementXB(ICurrencyService currencyService)
        {
            this.currencyService = currencyService;
        }

        public void Build(List<string> countries)
        {
            if (countries.Exists(x=>x == "England"))
            {
                England = true;
            }
            if (countries.Exists(x => x == "Wales"))
            {
                Wales = true; 
            }
            if (countries.Exists(x => x == "Scotland"))
            {
                Scotland = true;
            }
            if (countries.Exists(x => x == "NI"))
            {
                NI = true;
            }

            Currency = PartC.Invoices.OrderByDescending(x => x.DateDate).Select(x => x.Currency).FirstOrDefault();

            if (Currency == "Sterling")
            {
                bool tolerance = currencyService.CheckTolerance(PartAXB.TotalTotalSterling, PartC.TotalPayments);

                if (tolerance)
                {
                    PartAXB.TotalTotalSterling = PartC.TotalPayments;
                }
            }
        }  
    }   
    
    

    

    

    
}