using CsvHelper;
using RPA.ClaimStatements.Data.Context;
using RPA.ClaimStatements.Data.Entities.CS;
using RPA.ClaimStatements.Data.Entities.DAX;
using RPA.ClaimStatements.Generator.Utilities;
using RPA.ClaimStatements.Generator.Imports.Maps;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RPA.ClaimStatements.Generator.Imports.BulkInsert;
using RPA.ClaimStatements.Generator.Imports.Serializer;

namespace RPA.ClaimStatements.Generator.Imports
{
    public class APImportService : IImportService, IDisposable
    {
        ClaimStatementsContext db;
        IConfigurationService configurationService;
        IFolderService folderService;
        IErrorService errorService;
        IFileService fileService;
        ISerializer<AP> serializer;
        IBulkInsert<AP> bulkInsert;
        IMonitorService monitorService;

        const string apMutexId = "676DFF3F-C7F4-4E0F-BE5D-1CC7CE3373CD";

        public APImportService()
        {
            this.db = new ClaimStatementsContext();
            this.configurationService = new ConfigurationService(db);
            this.folderService = new FolderService(db);
            this.errorService = new ErrorService(db);
            this.fileService = new FileService();
            this.monitorService = new MonitorService(db);
            this.serializer = new Serializer<AP>();
            this.bulkInsert = new SQLBulkInsert<AP>();
        }

        public APImportService(ClaimStatementsContext context)
        {
            this.db = context;
            this.configurationService = new ConfigurationService(context);
            this.folderService = new FolderService(context);
            this.errorService = new ErrorService(context);
            this.fileService = new FileService();
            this.monitorService = new MonitorService(context);
            this.serializer = new Serializer<AP>();
            this.bulkInsert = new SQLBulkInsert<AP>();
        }

        public APImportService(ClaimStatementsContext context, IConfigurationService configurationService, IFolderService folderService,
            IErrorService errorService, IFileService fileService, IMonitorService monitorService, ISerializer<AP> serializer, IBulkInsert<AP> bulkInsert)
        {
            this.db = context;
            this.configurationService = configurationService;
            this.folderService = folderService;
            this.errorService = errorService;
            this.fileService = fileService;
            this.monitorService = monitorService;
            this.serializer = serializer;
            this.bulkInsert = bulkInsert;
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

                ImportAP();
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        private void ImportAP()
        {
            if (configurationService.IsActive("AP Load"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Checking DAX AP Inbound");
                Console.ResetColor();

                try
                {
                    Folder folder = folderService.Get("DAX AP Inbound");

                    List<string> bpsCodes = db.SchemeCodes.AsNoTracking().Select(x => x.SchemeCodeName).ToList();

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
                        
                        Data.Entities.CS.Monitor monitor = new Data.Entities.CS.Monitor(monitorService.GetMonitorProcessId("Import DAX AP"), string.Format(info.Name));
                        monitorService.CreateMonitor(monitor);

                        List<AP> newAP = serializer.DeSerialize(file, new APFileMap());
                        newAP = newAP.Where(x => bpsCodes.Exists(b => b == x.Scheme)).ToList();

                        List<AP> currentAP = db.AP.Where(x => x.Active).ToList();

                        InactivateDefunct(currentAP, newAP);

                        AddNew(currentAP, newAP);                       

                        fileService.Archive(file, folderService.GetPath("DAX AP Archive"), true, true);

                        monitorService.EndMonitor(monitor);
                    }
                }
                catch (Exception ex)
                {
                    errorService.Log(0, 0, string.Format("Unable to load DAX AP data - {0}", ex.Message));
                }
            }
        }

        private void InactivateDefunct(List<AP> currentAP, List<AP> newAP)
        {
            var defunctAP = currentAP.Except(newAP, new APComparer()).ToList();
              
            foreach (var ap in defunctAP)
            {
                ap.Inactivate();
            }

            db.SaveChanges();
        }

        private void AddNew(List<AP> currentAP, List<AP> newAP)
        {
            var ap = newAP.Except(currentAP, new APComparer()).ToList();            

            bulkInsert.Insert(ap, "DAX.AP", APSQLMap.ColumnMapping());
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
