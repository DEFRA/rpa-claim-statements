using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FluentFTP;

namespace RPA.ClaimStatements.Generator.Components.PumpingStation
{
    public class FTPService : IFTPService
    {
        string hostName;
        string userName;
        string password;

        public void SetOptions(string hostName, string userName, string password)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(hostName);
            ArgumentException.ThrowIfNullOrWhiteSpace(userName);
            this.hostName = hostName;
            this.userName = userName;
            this.password = password;
        }

        protected virtual IFtpClient CreateClient()
        {
            return new FtpClient(hostName, userName, password);
        }

        public void Download(string from, string to, string mask, string controlPrefix)
        {
            string directory = string.IsNullOrEmpty(from) ? "." : from.TrimEnd('/');
            if (directory.Length == 0)
            {
                directory = "/";
            }

            // FluentFTP has no get-by-mask, so list the folder and match * and ? here.
            var pattern = new Regex("\\A" + Regex.Escape(controlPrefix + mask).Replace("\\*", ".*").Replace("\\?", ".") + "\\z",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline);
            using (var client = CreateClient())
            {
                client.Config.EncryptionMode = FtpEncryptionMode.None;
                client.Config.DataConnectionType = FtpDataConnectionType.AutoPassive;
                client.Config.DownloadDataType = FtpDataType.Binary;
                client.Connect();

                var controls = client.GetListing(directory, FtpListOption.Auto)
                    .Where(item => item.Type == FtpObjectType.File && pattern.IsMatch(item.Name))
                    .Select(item => RemotePath(directory, item.Name)).ToArray();

                // The legacy batch completes every control transfer before starting any data transfer.
                foreach (string control in controls)
                {
                    DownloadAndRemove(client, control, to);
                }

                foreach (string control in controls)
                {
                    string dataFile = control.Replace(controlPrefix, "");
                    DownloadAndRemove(client, dataFile, to);

                    Console.WriteLine("Downloaded {0}", dataFile);
                }
            }
        }

        private static string RemotePath(string directory, string name)
        {
            if (string.IsNullOrEmpty(name) || name == "." || name == ".."
                || name.IndexOfAny(new[] { '/', '\\', ':', '*', '?', '\r', '\n' }) >= 0)
            {
                throw new IOException("The FTP listing contains an unsafe SITI filename.");
            }
            return directory.TrimEnd('/') + "/" + name;
        }

        private static void DownloadAndRemove(IFtpClient client, string remotePath, string destination)
        {
            string name = remotePath.Substring(remotePath.LastIndexOf('/') + 1);
            string temporary = Path.Combine(destination, ".csg-download-" + Guid.NewGuid().ToString("N") + ".part");
            try
            {
                var status = client.DownloadFile(temporary, remotePath, FtpLocalExists.Overwrite, FtpVerify.None, null);
                if (status != FtpStatus.Success)
                {
                    throw new IOException("SITI FTP download did not complete: " + remotePath + " (" + status + ").");
                }
                File.Move(temporary, Path.Combine(destination, name), true);
                client.DeleteFile(remotePath);
            }
            finally
            {
                File.Delete(temporary);
            }
        }
    }
}
