using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Services
{
    public interface IFileService
    {
        void Copy(string from, string to, bool overwrite = false);

        void Move(string from, string to);

        void Delete(string filePath);

        string Archive(string filePath, string location, bool move = false, bool appendTimeStamp = false);        

        string GetName(string filePath);

        string AppendTimeStamp(string fileName);
    }
}
