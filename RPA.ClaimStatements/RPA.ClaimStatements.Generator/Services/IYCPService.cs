namespace RPA.ClaimStatements.Generator.Services
{
    public interface IYCPService
    {
        string GetRate(int schemeYear, decimal overDeclarationPercent, decimal OverDeclarationHectares, bool previous);
    }
}