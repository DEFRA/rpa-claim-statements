using RPA.ClaimStatements.Generator.Models.Entities.CS;

namespace RPA.ClaimStatements.Generator.Services
{
    public interface IFolderService
    {
        string Root();

        string GetPath(string name);

        Folder Get(string name);

        void CheckCreateWorking();
    }
}
