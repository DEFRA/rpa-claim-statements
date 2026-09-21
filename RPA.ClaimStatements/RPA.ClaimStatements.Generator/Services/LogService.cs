using RPA.ClaimStatements.Data.Context;
using RPA.ClaimStatements.Data.Entities.CS;
using RPA.ClaimStatements.Data.Entities.DAX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Services
{
    public class LogService : ILogService, IDisposable
    {
        ClaimStatementsContext db;

        public LogService()
        {
            this.db = new ClaimStatementsContext();
        }

        public LogService(ClaimStatementsContext context)
        {
            this.db = context;
        }

        public void Log(long frn, int sbi, int schemeYear, string filePath, Guid claimId)
        {
            Log log = new Log(frn, sbi, schemeYear, filePath, claimId);
            db.Log.Add(log);

            var apList = db.AP.Where(x => x.Supplier == frn && x.MarketingYear == schemeYear && x.LogID == null && x.Active).ToList();
            
            foreach(var ap in apList)
            {
                ap.LogID = log.LogID;
            }

            var arList = db.AR.Where(x => x.H_InvoiceAccount == frn && x.H_MarketingYear == schemeYear && x.LogID == null).ToList();
            
            foreach(var ar in arList)
            {
                ar.LogID = log.LogID;
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
