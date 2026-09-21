using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Models.Entities.XB;
using RPA.ClaimStatements.Generator.Extensions;
using RPA.ClaimStatements.Generator.Components.PumpingStation.BulkInsert;
using RPA.ClaimStatements.Generator.Components.PumpingStation.Maps;
using RPA.ClaimStatements.Generator.Components.PumpingStation.Serializer;
using RPA.ClaimStatements.Generator.Models.Enums;
using RPA.ClaimStatements.Generator.Services;
using RPA.ClaimStatements.Generator.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Components.PumpingStation
{
    public class XBImportService : IDisposable, IXBImportService
    {
        IClaimStatementsContext db;
        IConfigurationService configurationService;
        IFolderService folderService;
        IErrorService errorService;
        IFileService fileService;
        IMonitorService monitorService;
        ISerializer<XBData> serializer;
        IBulkInsert<XB> xbBulkInsert;
        IBulkInsert<XBData> xbDataBulkInsert;

        const string apMutexId = "FD4F74C2-C9F1-42E3-8642-021404584A1B";
        
        public XBImportService(IClaimStatementsContext context, IConfigurationService configurationService, IFolderService folderService,
            IErrorService errorService, IFileService fileService, IMonitorService monitorService, ISerializer<XBData> serializer, IBulkInsert<XB> xbBulkInsert, 
            IBulkInsert<XBData> xbDataBulkInsert)
        {
            this.db = context;
            this.configurationService = configurationService;
            this.folderService = folderService;
            this.errorService = errorService;
            this.fileService = fileService;
            this.monitorService = monitorService;
            this.serializer = serializer;
            this.xbBulkInsert = xbBulkInsert;
            this.xbDataBulkInsert = xbDataBulkInsert;
        }

        public void Import()
        {
            Mutex mutex = new Mutex(false, string.Format(@"Global\CSG_{0}", apMutexId));

            try
            {
                try
                {
                    mutex.WaitOne();
                }
                catch (AbandonedMutexException) { }

                ImportXB();
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        private void ImportXB()
        {
            if (configurationService.IsActive("XB Load"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Checking XB Inbound");
                Console.ResetColor();

                try
                {
                    Folder folder = folderService.Get("XB Inbound");
                    
                    string[] files;

                    files = Directory.GetFiles(folder.Path, folder.Mask);

                    if (files.Length == 0)
                    {
                        Console.WriteLine("No files awaiting load");
                    }

                    foreach (string file in files)
                    {
                        FileInfo info = new FileInfo(file);

                        Console.WriteLine("Importing {0}", info.Name);

                        Models.Entities.CS.Monitor monitor = new Models.Entities.CS.Monitor(monitorService.GetMonitorProcessId("Import XB"), string.Format(info.Name));
                        monitorService.CreateMonitor(monitor);

                        List<XBData> newXB = serializer.DeSerialize(file);

                        List<XBData> currentXB = db.XB.AsNoTracking().Where(x => x.Active).Select(x=>x.XBData).ToList();

                        InactivateDefunct(currentXB, newXB);

                        AddNew(currentXB, newXB);

                        fileService.Archive(file, folderService.GetPath("XB Archive"), true, true);

                        monitorService.EndMonitor(monitor);
                    }
                }
                catch (Exception ex)
                {
                    errorService.Log(0, 0, string.Format("Unable to load XB data - {0}", ex.Message));
                }
            }
        }        

        private void InactivateDefunct(List<XBData> currentXB, List<XBData> newXB)
        {
            var defunctXB = currentXB.Except(newXB, new XBDataComparer()).ToList();

            foreach(var xbData in defunctXB)
            {
                List<XB> xb = db.XB.Where(x => x.FRN == xbData.FRN && x.SchemeYear == xbData.SchemeYear && x.InvoiceNumber == xbData.InvoiceNumber && x.CalculationDate == xbData.CalculationDate && x.Active).ToList();

                foreach(var x in xb)
                {
                    x.Inactivate();
                }
            }

            db.SaveChanges();
        }

        private void AddNew(List<XBData> currentXB, List<XBData> newXB)
        {
            var xbData = newXB.Except(currentXB, new XBDataComparer()).ToList();
            List<XB> xb = new List<XB>();
            
            foreach (var xbD in xbData)
            {
                xb.Add(new XB(xbD.XBDataID, xbD.FRN, xbD.SchemeYear, xbD.InvoiceNumber, xbD.CalculationDate));                
            }

            if(xb.Count > 0)
            {
                xbBulkInsert.Insert(xb, "XB.XB", XBSQLMap.ColumnMapping());
                xbDataBulkInsert.Insert(xbData, "XB.XBData", XBDataSQLMap.ColumnMapping());
            }
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
