using RPA.ClaimStatements.Generator.Models.Entities.CS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
