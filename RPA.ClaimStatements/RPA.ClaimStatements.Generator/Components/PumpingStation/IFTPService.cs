namespace RPA.ClaimStatements.Generator.Components.PumpingStation
{
    public interface IFTPService
    {
        void SetOptions(string hostName, string userName, string password);
        void Download(string from, string to, string mask, string controlPrefix);
    }
}