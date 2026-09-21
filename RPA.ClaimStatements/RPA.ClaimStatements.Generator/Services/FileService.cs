using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Services
{
    public class FileService : IFileService
    {
        public string Archive(string filePath, string location, bool move = false, bool appendTimeStamp = false)
        {
            string fileName = GetName(filePath);

            if(appendTimeStamp)
            {
                fileName = AppendTimeStamp(fileName);
            }

            string archivePath = Path.Combine(location, fileName);
            
            if (!move)
            {
                Copy(filePath, archivePath, true);
            }
            else
            {
                Move(filePath, archivePath);
            }

            return archivePath;          
        }

        public string AppendTimeStamp(string fileName)
        {
            return string.Concat(Path.GetFileNameWithoutExtension(fileName),
                "_",
                DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                Path.GetExtension(fileName));            
        }

        public void Copy(string from, string to, bool overwrite = false)
        {
            File.Copy(from, to, overwrite);
        }

        public void Delete(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public string GetName(string filePath)
        {
            return new FileInfo(filePath).Name;            
        }

        public void Move(string from, string to)
        {
            File.Move(from, to);
        }
    }
}
