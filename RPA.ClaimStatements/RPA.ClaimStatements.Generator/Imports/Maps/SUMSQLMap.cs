using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Imports.Maps
{
    public static class SUMSQLMap
    {
        public static List<string> ColumnMapping()
        {
            var mappings = new List<string>();

            mappings.Add("SUMID,SUMID");
            mappings.Add("ClaimID,ClaimID");
            mappings.Add("SchemeYear,SchemeYear");
            mappings.Add("ApplicationID,ApplicationID");
            mappings.Add("CalculationDate,CalculationDate");
            mappings.Add("InvoiceNumber,InvoiceNumber");
            mappings.Add("CalculationRefNumber,CalculationRefNumber");
            mappings.Add("BusinessName,BusinessName");
            mappings.Add("SBI,SBI");
            mappings.Add("FRN,FRN");
            mappings.Add("AddressLine1,AddressLine1");
            mappings.Add("AddressLine2,AddressLine2");
            mappings.Add("AddressLine3,AddressLine3");
            mappings.Add("PostCode,PostCode");

            return mappings;
        }
    }
}
