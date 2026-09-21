using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Services
{
    public class ErrorService : IErrorService, IDisposable
    {
        IClaimStatementsContext db;
        
        public ErrorService(IClaimStatementsContext context)
        {
            this.db = context;
        }

        public void Log(long frn, int schemeYear, string message, string filePath = null)
        {
            Error error = new Error(frn, schemeYear, message, filePath);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();

            db.Errors.Add(error);
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
