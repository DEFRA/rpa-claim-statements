using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Services
{
    public class ConfigurationService : IConfigurationService, IDisposable
    {
        IClaimStatementsContext db;        

        public ConfigurationService(IClaimStatementsContext context)
        {
            this.db = context;
        }

        public string GetValue(string name)
        {
            return db.Configurations.AsNoTracking().Where(x => x.Setting == name).Select(x => x.Value).FirstOrDefault();
        }

        public bool IsActive(string name)
        {
            string value = db.Configurations.AsNoTracking().Where(x => x.Setting == name).Select(x => x.Value).FirstOrDefault();

            if(value == "Active")
            {
                return true;
            }

            return false;
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
