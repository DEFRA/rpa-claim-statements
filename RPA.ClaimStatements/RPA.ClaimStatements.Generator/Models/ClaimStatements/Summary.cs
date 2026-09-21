using RPA.ClaimStatements.Generator.Models.Entities.SITI;
using RPA.ClaimStatements.Generator.Models.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Models.ClaimStatements
{
    public class Summary
    {
        public string CustomerName { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string Address3 { get; set; }

        public string City { get; set; }

        public string PostCode { get; set; }

        public string Date { get; set; }

        public int SchemeYear { get; set; }

        public void Build(SUM sum)
        {
            CustomerName = sum.BusinessName;
            Address1 = sum.AddressLine1;
            Address2 = sum.AddressLine2;
            PostCode = sum.PostCode;
            Date = DateTime.Now.ToString("dd MMMM yyyy");
            SchemeYear = sum.SchemeYear;
            
            if (!string.IsNullOrEmpty(sum.AddressLine3))
            {
                int cIndex = sum.AddressLine3.IndexOf("CITY:");

                if (cIndex != -1)
                {
                    if (cIndex != 0)
                    {
                        Address3 = sum.AddressLine3.Substring(0, cIndex);
                    }

                    City = sum.AddressLine3.Substring(cIndex + 5);
                }
                else
                {
                    Address3 = sum.AddressLine3;
                }
            }
        }
    }
}
