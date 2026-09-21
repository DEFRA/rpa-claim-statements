using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Services
{
    public interface IConversionService
    {
        string TransformInvoice(string invoice, string to);
    }
}
