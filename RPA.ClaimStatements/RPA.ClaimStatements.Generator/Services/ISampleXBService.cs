using RPA.ClaimStatements.Generator.Models.ClaimStatements;

namespace RPA.ClaimStatements.Generator.Services
{
    public interface ISampleXBService
    {
        string Sample(int? schemeYear = null);
    }
}