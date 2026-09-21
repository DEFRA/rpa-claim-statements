using RPA.ClaimStatements.Generator.Models.Generation;

namespace RPA.ClaimStatements.Generator.Components.FuelTank
{
    public interface IFuelTank
    {
        Request[] Fill(string statementType, int maximumBatchSize, bool dbFunctions = true);
    }
}
