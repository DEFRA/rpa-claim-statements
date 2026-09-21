using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WinSCP;

namespace RPA.ClaimStatements.Generator.Imports
{
    public class FTPService : IFTPService
    {
        SessionOptions sessionOptions;

        public FTPService(string hostName, string userName, string password)
        {
            sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Ftp,
                HostName = hostName,
                UserName = userName,
                Password = password
            };
        }

        public void Download(string from, string to, string mask, string controlPrefix)
        {
            if (HttpRuntime.AppDomainAppId == null)
            {
                using (Session session = new Session())
                {
                    session.Open(sessionOptions);

                    TransferOptions transferOptions = new TransferOptions();
                    transferOptions.TransferMode = TransferMode.Binary;

                    TransferOperationResult controlTransfer = session.GetFiles(string.Format("{0}{1}{2}", from, controlPrefix, mask), to, true, transferOptions);
                    controlTransfer.Check();

                    foreach (TransferEventArgs transfer in controlTransfer.Transfers)
                    {
                        string dataFile = transfer.FileName.Replace(controlPrefix, "");

                        TransferOperationResult dataTransfer = session.GetFiles(dataFile, to, true, transferOptions);
                        dataTransfer.Check();

                        Console.WriteLine("Downloaded {0}", dataFile);
                    }
                }
            }
        }
    }
}
