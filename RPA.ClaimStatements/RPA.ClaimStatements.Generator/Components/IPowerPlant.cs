namespace RPA.ClaimStatements.Generator.Components
{
    public interface IPowerPlant
    {
        void Activate(string statementType, int maximumBatchSize = 0);
    }
}