namespace RPA.ClaimStatements.Generator.Imports
{
    public interface IFTPService
    {
        void Download(string from, string to, string mask, string controlPrefix);
    }
}