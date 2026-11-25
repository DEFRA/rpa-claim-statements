namespace RPA.ClaimStatements.Generator.Services
{
    public interface IErrorService
    {
        void Log(long frn, int schemeYear, string message, string filePath = null);        
    }
}
