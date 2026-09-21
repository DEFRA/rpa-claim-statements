using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RPA.ClaimStatements.Generator.Models.Generation
{
    public class Skip
    {
        public Int64 FRN { get; set; }

        public int? SchemeYear { get; set; }

        public Skip(Int64 frn)
        {
            FRN = frn;
        }

        public Skip(Int64 frn, int schemeYear):this(frn)
        {
            SchemeYear = schemeYear;
        }
    }
}