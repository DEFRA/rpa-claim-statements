using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RPA.ClaimStatements.Generator.Models.Entities.XB
{
    [Table("XB", Schema ="XB")]
    public class XB
    {
        public Guid XBID { get; set; }

        public Guid XBDataID { get; set; }

        public Int64 FRN { get; set; }

        public int SBI { get; set; }

        public string InvoiceNumber { get; set; }

        public DateTime CalculationDate { get; set; }

        public int SchemeYear { get; set; }        

        public bool Active { get; set; }

        public virtual XBData XBData { get; set; }

        public void Inactivate()
        {
            Active = false;
        }

        public XB()
        {
            XBID = Guid.NewGuid();
            Active = true;
        }

        public XB(long frn, int schemeYear):this()
        {
            FRN = frn;
            SchemeYear = schemeYear;
        }

        public XB(long frn, int schemeYear, string invoice, DateTime calculationDate) : this(frn, schemeYear)
        {
            InvoiceNumber = invoice;
            CalculationDate = calculationDate;
        }

        public XB(Guid xbDataID, long frn, int schemeYear, string invoice, DateTime calculationDate) : this(frn, schemeYear, invoice, calculationDate)
        {
            XBDataID = xbDataID;
        }
    }
}