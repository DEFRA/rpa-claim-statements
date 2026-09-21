using RPA.ClaimStatements.Data.Entities.CS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using RPA.ClaimStatements.Data.Entities.DAX;
using System.Threading;
using RPA.ClaimStatements.Data.Context;
using RPA.ClaimStatements.Generator.Services;

namespace RPA.ClaimStatements.Generator.Imports
{
    public class ARImportService : IImportService, IDisposable
    {
        ClaimStatementsContext db;
        IConfigurationService configurationService;
        IFolderService folderService;
        IErrorService errorService;
        IFileService fileService;
        IMonitorService monitorService;

        const string arMutexId = "E1F813C0-9843-48C8-937D-E5F93BC8D64B";

        public ARImportService()
        {
            this.db = new ClaimStatementsContext();
            this.configurationService = new ConfigurationService(db);
            this.folderService = new FolderService(db);
            this.errorService = new ErrorService(db);
            this.fileService = new FileService();
            this.monitorService = new MonitorService(db);
        }

        public ARImportService(ClaimStatementsContext context)
        {
            this.db = context;
            this.configurationService = new ConfigurationService(context);
            this.folderService = new FolderService(context);
            this.errorService = new ErrorService(context);
            this.fileService = new FileService();
            this.monitorService = new MonitorService(context);
        }

        public ARImportService(ClaimStatementsContext context, IConfigurationService configurationService, IFolderService folderService, IErrorService errorService, IFileService fileService, IMonitorService monitorService)
        {
            this.db = context;
            this.configurationService = configurationService;
            this.folderService = folderService;
            this.errorService = errorService;
            this.fileService = fileService;
            this.monitorService = monitorService;
        }

        public void Import()
        {
            Mutex mutex = new Mutex(false, string.Format(@"Global\CSG_{0}", arMutexId));

            try
            {
                try
                {
                    mutex.WaitOne();
                }
                catch (AbandonedMutexException) { }
                
                ImportAR();
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        private void ImportAR()
        {
            if(configurationService.IsActive("AR Load"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Checking DAX AR Inbound");
                Console.ResetColor();

                try
                {
                    Folder folder = folderService.Get("DAX AR Inbound");

                    DataTable arTable = new DataTable("DAXAR");

                    List<string> bpsCodes = db.SchemeCodes.AsNoTracking().Select(x => x.SchemeCodeName).ToList();

                    bool firstRowIsHeader = true;

                    string[] files;

                    files = Directory.GetFiles(folder.Path, folder.Mask);

                    if(files.Length == 0)
                    {
                        Console.WriteLine("No files awaiting load");
                    }

                    foreach (string file in files)
                    {
                        FileInfo info = new FileInfo(file);
                                                
                        Console.WriteLine("Importing {0}", info.Name);
                        
                        Data.Entities.CS.Monitor monitor = new Data.Entities.CS.Monitor(monitorService.GetMonitorProcessId("Import DAX AR"), string.Format(info.Name));
                        monitorService.CreateMonitor(monitor);
                                                
                        ExcelPackage ePack = new ExcelPackage(info);

                        ExcelWorksheet sheet1 = ePack.Workbook.Worksheets.First();

                        for (int xx = 1; xx <= 20; xx++)
                        {
                            string data = "";
                            if (sheet1.Cells[1, xx].Value != null)
                            {
                                data = sheet1.Cells[1, xx].Value.ToString();

                                string columnName = "column" + xx.ToString();
                                arTable.Columns.Add(columnName);
                            }
                        }
                        
                        foreach (ExcelWorksheet sheet in ePack.Workbook.Worksheets)
                        {
                            int first = firstRowIsHeader ? 2 : 1;

                            for (int excelRow = first; excelRow <= sheet.Dimension.End.Row; excelRow++)
                            {
                                DataRow rw = arTable.NewRow();
                                arTable.Rows.Add(rw);

                                for (int excelCol = 1; excelCol <= 20; excelCol++)
                                {
                                    string data = "";
                                    if (sheet.Cells[excelRow, excelCol].Value != null)
                                    {
                                        data = sheet.Cells[excelRow, excelCol].Value.ToString();
                                    }
                                    rw[excelCol - 1] = data;
                                }
                            }
                        }

                        foreach (DataRow row in arTable.Rows)
                        {
                            if (bpsCodes.Contains(row[5]))
                            {
                                CreateAR(row);
                            }
                        }

                        db.SaveChanges();

                        fileService.Archive(file, folderService.GetPath("DAX AR Archive"), true, true);

                        monitorService.EndMonitor(monitor);
                    }
                }
                catch (Exception ex)
                {
                    errorService.Log(0, 0, string.Format("Unable to load DAX AR data - Confirm data file contains all data headers on every worksheet - {0}", ex.Message));
                }
            }
        }

        public void CreateAR(DataRow row)
        {
            Int64 supplier;
            DateTime invoiceDate;
            int lineNumber;            

            if (Int64.TryParse(row[1].ToString(), out supplier) && DateTime.TryParse(row[2].ToString(), out invoiceDate) && int.TryParse(row[8].ToString(), out lineNumber))
            {
                supplier = Int64.Parse(row[1].ToString());
                invoiceDate = DateTime.Parse(row[2].ToString());

                string invoice = row[0].ToString();
                                
                if (!db.AR.AsNoTracking().Any(x => x.H_InvoiceAccount == supplier && x.InvoiceNumber == invoice && x.L_LineNumber == lineNumber)) 
                {
                    AR newAR = new AR
                    {
                        ARID = Guid.NewGuid(),
                        H_InvoiceDate = invoiceDate,
                        InvoiceNumber = invoice,
                        H_InvoiceAccount = supplier,
                        L_LineNumber = lineNumber,
                        H_CurrencyCode = row[3].ToString(),
                        H_Fund = row[4].ToString(),
                        H_Scheme = row[5].ToString(),
                        H_DeliveryBody = row[7].ToString(),
                        L_LedgerAccount = row[9].ToString(),
                        L_Fund = row[10].ToString(),
                        L_Scheme = row[11].ToString(),
                        L_DeliveryBody = row[13].ToString(),
                        Irregularity = row[15].ToString(),
                        Admin = row[16].ToString(),
                        OriginalClaimReference = row[17].ToString()
                    };

                    int H_MarketingYearInt;

                    if (int.TryParse(row[6].ToString(), out H_MarketingYearInt))
                    {
                        H_MarketingYearInt = int.Parse(row[6].ToString());
                        newAR.H_MarketingYear = H_MarketingYearInt;
                    }                    

                    int L_MarketingYearInt;

                    if (int.TryParse(row[12].ToString(), out L_MarketingYearInt))
                    {
                        L_MarketingYearInt = int.Parse(row[12].ToString());
                        newAR.L_MarketingYear = L_MarketingYearInt;
                    }

                    decimal L_LineAmountDecimal;

                    if (decimal.TryParse(row[14].ToString(), out L_LineAmountDecimal))
                    {
                        L_LineAmountDecimal = decimal.Parse(row[14].ToString());
                        newAR.L_LineAmount = (L_LineAmountDecimal * -1);
                    }

                    DateTime OriginalClaimSettlementDateDate;

                    if (DateTime.TryParse(row[18].ToString(), out OriginalClaimSettlementDateDate))
                    {
                        OriginalClaimSettlementDateDate = DateTime.Parse(row[18].ToString());
                        newAR.OriginalClaimSettlementDate = OriginalClaimSettlementDateDate;
                    }
                    else
                    {
                        newAR.OriginalClaimSettlementDate = null;
                    }

                    int OpenBalanceInt;

                    if (int.TryParse(row[19].ToString(), out OpenBalanceInt))
                    {
                        OpenBalanceInt = int.Parse(row[19].ToString());
                        newAR.OpenBalance = OpenBalanceInt;
                    }

                    db.AR.Add(newAR);                    
                }
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

