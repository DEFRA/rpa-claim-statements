using RPA.ClaimStatements.Generator.Models.ClaimStatements;
using RPA.ClaimStatements.Generator.Models.Generation;

namespace RPA.ClaimStatements.Generator.Components.Turbine
{
    public interface IBuildService
    {
        ClaimStatement Build(Request request);

        ClaimStatement BuildXB(Request request);
    }
}
