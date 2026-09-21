using RPA.ClaimStatements.Generator.Models.ClaimStatements;
using RPA.ClaimStatements.Generator.Models.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Components.Turbine
{
    public interface IBuildService
    {
        ClaimStatement Build(Request request);

        ClaimStatement BuildXB(Request request);
    }
}
