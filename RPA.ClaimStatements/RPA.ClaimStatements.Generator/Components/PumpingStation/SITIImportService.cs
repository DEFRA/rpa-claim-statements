using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Models.Entities.SITI;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Entity;
using RPA.ClaimStatements.Generator.Components.PumpingStation.Maps;
using RPA.ClaimStatements.Generator.Utilities;
using RPA.ClaimStatements.Generator.Components.PumpingStation.Serializer;
using RPA.ClaimStatements.Generator.Components.PumpingStation.BulkInsert;
using RPA.ClaimStatements.Generator.Models.Enums;
using RPA.ClaimStatements.Generator.Extensions;

namespace RPA.ClaimStatements.Generator.Components.PumpingStation
{
    public class SITIImportService : IDisposable, ISITIImportService
    {
        IClaimStatementsContext db;
        IFTPService ftpService;
        IConfigurationService configurationService;
        IFolderService folderService;
        IErrorService errorService;
        IFileService fileService;
        IMonitorService monitorService;
        ISerializer<Claim> serializer;
        ISITIBulkInsertContainer sitiBulkInsertContainer;


        string controlPrefix = "CTL_";

        const string sitiMutexId = "5C45109B-C89C-4B25-AF6A-98A2325EE4B0";

        public SITIImportService(IClaimStatementsContext context, IFTPService ftpService, IConfigurationService configurationService, IFolderService folderService,
            IErrorService errorService, IFileService fileService, IMonitorService monitorService, ISerializer<Claim> serializer, ISITIBulkInsertContainer sitiBulkInsertContainer)
        {
            this.db = context;
            this.ftpService = ftpService;
            this.configurationService = configurationService;
            this.folderService = folderService;
            this.errorService = errorService;
            this.fileService = fileService;
            this.monitorService = monitorService;
            this.serializer = serializer;
            this.sitiBulkInsertContainer = sitiBulkInsertContainer;
        }

        public void Import()
        {
            Mutex mutex = new Mutex(false, string.Format(@"Global\CSG_{0}", sitiMutexId));

            try
            {
                try
                {
                    mutex.WaitOne();
                }
                catch (AbandonedMutexException) { }

                ImportSITI();
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        private void ImportSITI()
        {
            if (configurationService.IsActive("SITI Load"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Checking SITI Inbound");
                Console.ResetColor();

                try
                {
                    Folder ftpFolder = folderService.Get("SITI FTP");
                    Folder workingFolder = folderService.Get("SITI Inbound");

                    try
                    {
                        ftpService.SetOptions(ftpFolder.Host, ftpFolder.UserName, ftpFolder.Password);
                        ftpService.Download(ftpFolder.Path, workingFolder.Path, ftpFolder.Mask, controlPrefix);
                    }
                    catch(Exception ex)
                    {
                        errorService.Log(0, 0, string.Format("Unable to download SITI data - {0}", ex.Message));
                    }

                    string[] files;

                    files = Directory.GetFiles(workingFolder.Path, workingFolder.Mask);

                    if (files.Length == 0)
                    {
                        Console.WriteLine("No files awaiting load");
                    }

                    foreach (string file in files)
                    {
                        FileInfo info = new FileInfo(file);

                        Console.WriteLine("Importing {0}", info.Name);

                        Models.Entities.CS.Monitor monitor = new Models.Entities.CS.Monitor(monitorService.GetMonitorProcessId("Import SITI"), string.Format(info.Name));
                        monitorService.CreateMonitor(monitor);

                        List<Claim> newClaims = serializer.DeSerialize(file, null, false);

                        AddNew(newClaims);

                        fileService.Archive(file, folderService.GetPath("SITI Archive"), true);
                        fileService.Delete(Path.Combine(workingFolder.Path, string.Format("{0}{1}", controlPrefix, info.Name)));

                        monitorService.EndMonitor(monitor);

                    }
                }
                catch (Exception ex)
                {
                    errorService.Log(0, 0, string.Format("Unable to load SITI data - {0}", ex.Message));
                }
            }
        }

        private void AddNew(List<Claim> newClaims)
        {
            List<Claim> claims = new List<Claim>();
            List<SUM> sum = new List<SUM>();
            List<SUM2> sum2 = new List<SUM2>();
            List<BPS> bps = new List<BPS>();
            List<BPSPEN> bpspen = new List<BPSPEN>();
            List<GR> gr = new List<GR>();
            List<GRPEN> grpen = new List<GRPEN>();
            List<YF> yf = new List<YF>();
            List<CLD> cld = new List<CLD>();
            List<PR> pr = new List<PR>();

            HashSet<string> currentInvoiceNumbers = new HashSet<string>(db.SUM.Select(x => x.InvoiceNumber));
            claims = newClaims.Where(x => !currentInvoiceNumbers.Contains(x.SUM.InvoiceNumber)).ToList();

            if (claims.Count > 0)
            {
                foreach (var c in claims)
                {
                    sum.Add(c.SUM);
                    sum2.Add(c.SUM2);
                    bps.Add(c.BPS);
                    bpspen.Add(c.BPSPEN);
                    gr.Add(c.GR);
                    grpen.Add(c.GRPEN);
                    yf.Add(c.YF);
                    cld.Add(c.CLD);
                    pr.Add(c.PR);
                }

                sitiBulkInsertContainer.ClaimBulkInsert.Insert(claims, "SITI.Claims", ClaimSQLMap.ColumnMapping());
                sitiBulkInsertContainer.SUMBulkInsert.Insert(sum, "SITI.SUM", SUMSQLMap.ColumnMapping());
                sitiBulkInsertContainer.SUM2BulkInsert.Insert(sum2, "SITI.SUM2", SUM2SQLMap.ColumnMapping());
                sitiBulkInsertContainer.BPSBulkInsert.Insert(bps, "SITI.BPS", BPSSQLMap.ColumnMapping());
                sitiBulkInsertContainer.BPSPENBulkInsert.Insert(bpspen, "SITI.BPSPEN", BPSPENSQLMap.ColumnMapping());
                sitiBulkInsertContainer.GRBulkInsert.Insert(gr, "SITI.GR", GRSQLMap.ColumnMapping());
                sitiBulkInsertContainer.GRPENBulkInsert.Insert(grpen, "SITI.GRPEN", GRPENSQLMap.ColumnMapping());
                sitiBulkInsertContainer.YFBulkInsert.Insert(yf, "SITI.YF", YFSQLMap.ColumnMapping());
                sitiBulkInsertContainer.CLDBulkInsert.Insert(cld, "SITI.CLD", CLDSQLMap.ColumnMapping());
                sitiBulkInsertContainer.PRBulkInsert.Insert(pr, "SITI.PR", PRSQLMap.ColumnMapping());

                ClearErrors(sum);
            }
        }

        private void ClearErrors(List<SUM> sums)
        {
            var invoiceNumbers = sums.Select(x => string.Format("No SITI data for invoice {0}", x.InvoiceNumber)).ToList();

            var sitiErrors = db.Errors.Where(x => !x.Resolved).ToList();
            sitiErrors = sitiErrors.Where(x => invoiceNumbers.Exists(i => i == x.ErrorMessage)).ToList();

            foreach (var error in sitiErrors)
            {
                error.Resolved = true;
            }

            db.SaveChanges();
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    db.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
