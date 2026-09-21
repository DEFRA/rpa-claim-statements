using RPA.ClaimStatements.Generator.Models.Entities.SITI;
using RPA.ClaimStatements.Generator.Models.Entities.XB;
using RPA.ClaimStatements.Generator.Models.ClaimStatements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RPA.ClaimStatements.Generator.Models.Generation
{
    public class Request
    {
        public Claim Claim { get; set; }

        public XBData XBData { get; set; }

        public List<Invoice> Invoices { get; set; }
        
        public DateTime SettlementDate { get; set; }
        
        public List<Rate> Rates { get; set; }

        public Request()
        {
            Claim = new Claim();
            XBData = new XBData();
            Invoices = new List<Invoice>();
            Rates = new List<Rate>();
        }

        public Request(Claim claim, DateTime settlementDate, List<Invoice> invoices, List<Rate> rates)
        {
            Claim = claim;
            SettlementDate = settlementDate;
            Invoices = invoices;
            Rates = rates;
        }

        public Request(Claim claim, XBData xbData, DateTime settlementDate, List<Invoice> invoices, List<Rate> rates) :this(claim, settlementDate, invoices, rates)
        {
            XBData = xbData;            
        }
    }
}