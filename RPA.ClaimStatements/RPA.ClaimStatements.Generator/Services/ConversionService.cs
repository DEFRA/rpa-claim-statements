using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Services
{
    public class ConversionService : IConversionService
    {
        public string TransformInvoice(string invoice, string to)
        {
            string prefix = null;

            switch(to)
            {
                case "SITI":
                    prefix = "SITI";
                    break;

                case "XB":
                    prefix = "PFSY";
                    break;

                default:
                    break;
            }

            if (invoice.Length >= 8)
            {
                return string.Format("{0}{1}", prefix, invoice.Substring(1, 7));
            }

            return invoice;
        }
    }
}
