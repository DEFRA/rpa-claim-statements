namespace RPA.ClaimStatements.Generator.Services
{
    public interface IConfigurationService
    {
        bool IsActive(string name);

        string GetValue(string name);        
    }
}
