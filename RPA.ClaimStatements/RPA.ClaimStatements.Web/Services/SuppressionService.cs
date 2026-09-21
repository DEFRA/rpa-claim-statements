using OfficeOpenXml;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace RPA.ClaimStatements.Web.Services
{
    public class SuppressionService : ISuppressionService, IDisposable
    {
        IClaimStatementsContext db;
        IFolderService folderService;
        IFileService fileService;
        string root;        
        
        public SuppressionService(IClaimStatementsContext context, IFolderService folderService, IFileService fileService)
        {
            this.db = context;
            this.folderService = folderService;
            this.fileService = fileService;
            this.root = folderService.Root();
        }

        public void BulkUpdate(string direction, string schemeYear, HttpPostedFileBase bulkSource)
        {
            DataTable bulkData = BulkData(bulkSource);

            switch (direction)
            {
                case "Add":
                    BulkAdd(bulkData, schemeYear);
                    break;

                case "Remove":
                    BulkRemove(bulkData, schemeYear);
                    break;

                default:
                    break;
            }
        }

        public DataTable BulkData(HttpPostedFileBase bulkSource)
        {
            string filepath = Path.Combine(root, "Uploads",Path.GetFileName(bulkSource.FileName));
            bulkSource.SaveAs(filepath);

            DataTable bulkTable = new DataTable("BulkSource");

            bool firstRowIsHeader = false;

            FileInfo existingfile = new FileInfo(filepath);

            ExcelPackage ePack = new ExcelPackage(existingfile);
            ExcelWorksheet ws = ePack.Workbook.Worksheets.First();

            for (int xx = 1; xx <= ws.Dimension.End.Column; xx++)
            {
                string data = "";
                if (ws.Cells[1, xx].Value != null)
                {
                    data = ws.Cells[1, xx].Value.ToString();

                    string columnName = firstRowIsHeader ? data : "column" + xx.ToString();
                    bulkTable.Columns.Add(columnName);
                }
            }

            int first = firstRowIsHeader ? 2 : 1;

            for (int excelRow = first; excelRow <= ws.Dimension.End.Row; excelRow++)
            {
                DataRow rw = bulkTable.NewRow();
                bulkTable.Rows.Add(rw);

                for (int excelCol = 1; excelCol <= ws.Dimension.End.Column; excelCol++)
                {
                    string data = "";
                    if (ws.Cells[excelRow, excelCol].Value != null)
                    {
                        data = ws.Cells[excelRow, excelCol].Value.ToString();
                    }
                    rw[excelCol - 1] = data;
                }
            }

            fileService.Delete(filepath);

            return bulkTable;
        }

        public void BulkAdd(DataTable bulkData, string schemeYear)
        {
            List<Suppression> outstanding = db.Suppressions.AsNoTracking().Where(x => x.SuppressionEnd == null).ToList();

            foreach (DataRow row in bulkData.Rows)
            {
                Int64 frn;

                if (Int64.TryParse(row[0].ToString().Trim(), out frn))
                {
                    if (!outstanding.Exists(x => x.FRN == frn && x.SchemeYear.ToString() == schemeYear))
                    {
                        int sy = int.Parse(schemeYear);

                        Suppression suppression = new Suppression(frn, sy);
                        suppression.Start();
                        db.Suppressions.Add(suppression);                        
                    }
                }
            }

            db.SaveChanges();
        }        

        public void BulkRemove(DataTable bulkData, string schemeYear)
        {
            List<Suppression> outstanding = db.Suppressions.Where(x => x.SuppressionEnd == null).ToList();

            foreach (Suppression suppression in outstanding)
            {
                foreach (DataRow row in bulkData.Rows)
                {
                    if (suppression.FRN.ToString() == (row[0].ToString()) && suppression.SchemeYear.ToString() == schemeYear)
                    {
                        suppression.End();
                        db.SetModified(suppression);
                    }
                }
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