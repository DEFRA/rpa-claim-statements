using RPA.ClaimStatements.Data.Context;
using RPA.ClaimStatements.Data.Entities.CS;
using RPA.ClaimStatements.Data.Entities.SITI;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Entity;
using RPA.ClaimStatements.Generator.Imports.Maps;
using RPA.ClaimStatements.Generator.Utilities;
using RPA.ClaimStatements.Generator.Imports.Serializer;
using RPA.ClaimStatements.Generator.Imports.BulkInsert;
using RPA.ClaimStatements.Generator.Models.Enums;
using RPA.ClaimStatements.Generator.Extensions;

namespace RPA.ClaimStatements.Generator.Imports
{
    public class SITIImportService : IImportService, IDisposable
    {
        ClaimStatementsContext db;
        IFTPService ftpService;
        IConfigurationService configurationService;
        IFolderService folderService;
        IErrorService errorService;
        IFileService fileService;
        IMonitorService monitorService;
        ISerializer<Claim> serializer;
        IBulkInsert<Claim> claimBulkInsert;
        IBulkInsert<SUM> sumBulkInsert;
        IBulkInsert<SUM2> sum2BulkInsert;
        IBulkInsert<BPS> bpsBulkInsert;
        IBulkInsert<BPSPEN> bpspenBulkInsert;
        IBulkInsert<GR> grBulkInsert;
        IBulkInsert<GRPEN> grpenBulkInsert;
        IBulkInsert<YF> yfBulkInsert;
        IBulkInsert<CLD> cldBulkInsert;

        string controlPrefix = "CTL_";

        const string sitiMutexId = "5C45109B-C89C-4B25-AF6A-98A2325EE4B0";

        public SITIImportService()
        {
            this.db = new ClaimStatementsContext();
            this.configurationService = new ConfigurationService(db);
            this.folderService = new FolderService(db);
            this.errorService = new ErrorService(db);
            this.fileService = new FileService();
            this.monitorService = new MonitorService(db);
            this.serializer = new SITISerializer();
            this.claimBulkInsert = new SQLBulkInsert<Claim>();
            this.sumBulkInsert = new SQLBulkInsert<SUM>();
            this.sum2BulkInsert = new SQLBulkInsert<SUM2>();
            this.bpsBulkInsert = new SQLBulkInsert<BPS>();
            this.bpspenBulkInsert = new SQLBulkInsert<BPSPEN>();
            this.grBulkInsert = new SQLBulkInsert<GR>();
            this.grpenBulkInsert = new SQLBulkInsert<GRPEN>();
            this.yfBulkInsert = new SQLBulkInsert<YF>();
            this.cldBulkInsert = new SQLBulkInsert<CLD>();
        }

        public SITIImportService(ClaimStatementsContext context)
        {
            this.db = context;
            this.configurationService = new ConfigurationService(context);
            this.folderService = new FolderService(context);
            this.errorService = new ErrorService(context);
            this.fileService = new FileService();
            this.monitorService = new MonitorService(context);
            this.serializer = new SITISerializer();
            this.claimBulkInsert = new SQLBulkInsert<Claim>();
            this.sumBulkInsert = new SQLBulkInsert<SUM>();
            this.sum2BulkInsert = new SQLBulkInsert<SUM2>();
            this.bpsBulkInsert = new SQLBulkInsert<BPS>();
            this.bpspenBulkInsert = new SQLBulkInsert<BPSPEN>();
            this.grBulkInsert = new SQLBulkInsert<GR>();
            this.grpenBulkInsert = new SQLBulkInsert<GRPEN>();
            this.yfBulkInsert = new SQLBulkInsert<YF>();
            this.cldBulkInsert = new SQLBulkInsert<CLD>();
        }

        public SITIImportService(ClaimStatementsContext uow, IFTPService ftpService, IConfigurationService configurationService, IFolderService folderService,
            IErrorService errorService, IFileService fileService, IMonitorService monitorService, ISerializer<Claim> serializer, IBulkInsert<Claim> claimBulkInsert, IBulkInsert<SUM> sumBulkInsert,
            IBulkInsert<SUM2> sum2BulkInsert, IBulkInsert<BPS> bpsBulkInsert, IBulkInsert<BPSPEN> bpspenBulkInsert, IBulkInsert<GR> grBulkInsert, IBulkInsert<GRPEN> grpenBulkInsert,
            IBulkInsert<YF> yfBulkInsert, IBulkInsert<CLD> cldBulkInsert)
        {
            this.db = uow;
            this.ftpService = ftpService;
            this.configurationService = configurationService;
            this.folderService = folderService;
            this.errorService = errorService;
            this.fileService = fileService;
            this.monitorService = monitorService;
            this.serializer = serializer;
            this.claimBulkInsert = claimBulkInsert;
            this.sumBulkInsert = sumBulkInsert;
            this.sum2BulkInsert = sum2BulkInsert;
            this.bpsBulkInsert = bpsBulkInsert;
            this.bpspenBulkInsert = bpspenBulkInsert;
            this.grBulkInsert = grBulkInsert;
            this.grpenBulkInsert = grpenBulkInsert;
            this.yfBulkInsert = yfBulkInsert;
            this.cldBulkInsert = cldBulkInsert;
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

                    if (ftpService == null)
                    {
                        ftpService = new FTPService(ftpFolder.Host, ftpFolder.UserName, ftpFolder.Password);
                    }

                    ftpService.Download(ftpFolder.Path, workingFolder.Path, ftpFolder.Mask, controlPrefix);

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

                        Data.Entities.CS.Monitor monitor = new Data.Entities.CS.Monitor(monitorService.GetMonitorProcessId("Import SITI"), string.Format(info.Name));
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
                }

                claimBulkInsert.Insert(claims, "SITI.Claims", ClaimSQLMap.ColumnMapping());
                sumBulkInsert.Insert(sum, "SITI.SUM", SUMSQLMap.ColumnMapping());
                sum2BulkInsert.Insert(sum2, "SITI.SUM2", SUM2SQLMap.ColumnMapping());
                bpsBulkInsert.Insert(bps, "SITI.BPS", BPSSQLMap.ColumnMapping());
                bpspenBulkInsert.Insert(bpspen, "SITI.BPSPEN", BPSPENSQLMap.ColumnMapping());
                grBulkInsert.Insert(gr, "SITI.GR", GRSQLMap.ColumnMapping());
                grpenBulkInsert.Insert(grpen, "SITI.GRPEN", GRPENSQLMap.ColumnMapping());
                yfBulkInsert.Insert(yf, "SITI.YF", YFSQLMap.ColumnMapping());
                cldBulkInsert.Insert(cld, "SITI.CLD", CLDSQLMap.ColumnMapping());

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
