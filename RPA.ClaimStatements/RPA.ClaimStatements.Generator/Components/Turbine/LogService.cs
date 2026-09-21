using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using RPA.ClaimStatements.Generator.Models.Entities.DAX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Components.Turbine
{
    public class LogService : ILogService, IDisposable
    {
        IClaimStatementsContext db;
                
        public LogService(ClaimStatementsContext context)
        {
            this.db = context;
        }

        public void Log(long frn, int sbi, int schemeYear, string filePath, Guid claimId)
        {
            Log log = new Log(frn, sbi, schemeYear, filePath, claimId);
            db.Log.Add(log);

            db.AP.Where(x => x.Supplier == frn && x.MarketingYear == schemeYear && x.LogID == null && x.Active).ToList().ForEach(x => x.LogID = log.LogID);
            db.AR.Where(x => x.H_InvoiceAccount == frn && x.H_MarketingYear == schemeYear && x.LogID == null).ToList().ForEach(x => x.LogID = log.LogID);
            
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
