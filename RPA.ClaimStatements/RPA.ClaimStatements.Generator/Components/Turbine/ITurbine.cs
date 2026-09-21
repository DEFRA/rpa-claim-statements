using RPA.ClaimStatements.Generator.Models.Generation;

namespace RPA.ClaimStatements.Generator.Components.Turbine
{
    public interface ITurbine
    {
        void Generate(Request[] requests, string statementType);
    }
}