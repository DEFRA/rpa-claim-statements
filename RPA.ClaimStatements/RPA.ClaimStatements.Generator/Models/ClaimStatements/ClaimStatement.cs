using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace RPA.ClaimStatements.Generator.Models.ClaimStatements
{    
    [Serializable]
    public class ClaimStatement
    {
        protected ICurrencyService currencyService;
        public Guid ClaimID { get; set; }        

        public Summary Summary { get; set; }

        public PartA PartA { get; set; }

        public PartB PartB { get; set; }

        public PartC PartC { get; set; }

        public PartD PartD { get; set; }

        public string Currency { get; set; }    
        
        public List<Rate> Rates { get; set; }
        
        public ClaimStatement()
        {
            currencyService = new CurrencyService();
        }

        public ClaimStatement(Guid claimId):this()
        {
            ClaimID = claimId;            
        }        

        public ClaimStatement(Guid claimId, Summary summary, PartA partA, PartB partB, PartC partC, PartD partD, List<Rate> rates):this(claimId)
        {
            Summary = summary;
            PartA = partA;
            PartB = partB;
            PartC = partC;
            PartD = partD;
            Rates = rates;
        }

        public ClaimStatement(ICurrencyService currencyService)
        {
            this.currencyService = currencyService;
        }

        public virtual void Build()
        {
            Currency = PartC.Invoices.OrderByDescending(x => x.DateDate).Select(x => x.Currency).FirstOrDefault();
            
            if (Currency == "Sterling")
            {
                bool tolerance = currencyService.CheckTolerance(PartA.TotalSterling, PartC.TotalPayments);

                if (tolerance)
                {
                    PartA.TotalSterling = PartC.TotalPayments;
                }
            } 
        }
    }
}