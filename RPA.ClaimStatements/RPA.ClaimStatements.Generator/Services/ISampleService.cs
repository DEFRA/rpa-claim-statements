using RPA.ClaimStatements.Generator.Models.ClaimStatements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Services
{
    public interface ISampleService
    {
        string Sample(int? schemeYear = null);
    }
}
