using System.Collections.Generic;

namespace RPA.ClaimStatements.Generator.Components.PumpingStation.Maps
{
    public static class XBSQLMap
    {
        public static List<string> ColumnMapping()
        {
            var mappings = new List<string>();

            mappings.Add("XBID,XBID");
            mappings.Add("XBDataID,XBDataID");
            mappings.Add("FRN,FRN");
            mappings.Add("SBI,SBI");
            mappings.Add("InvoiceNumber,InvoiceNumber");
            mappings.Add("CalculationDate,CalculationDate");
            mappings.Add("SchemeYear,SchemeYear");
            mappings.Add("Active,Active");            

            return mappings;
        }
    }
}
