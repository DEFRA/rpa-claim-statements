using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPA.ClaimStatements.Generator.Models.Entities.CS;
using System.Reflection;
using RPA.ClaimStatements.Generator.Context;
using System.Web;

namespace RPA.ClaimStatements.Generator.Services
{
    public class FolderService : IFolderService, IDisposable
    {
        IClaimStatementsContext db;
        
        public FolderService(IClaimStatementsContext context)
        {
            this.db = context;
        }

        public void CheckCreateWorking()
        {
            Directory.CreateDirectory(Path.Combine(Root(), "XML"));
            Directory.CreateDirectory(Path.Combine(Root(), "DOCX"));
            Directory.CreateDirectory(Path.Combine(Root(), "HTML"));
            Directory.CreateDirectory(Path.Combine(Root(), "PDF"));
            Directory.CreateDirectory(Path.Combine(Root(), "XSLT"));
            Directory.CreateDirectory(Path.Combine(Root(), "Templates"));
        }

        public Folder Get(string name)
        {
            return db.Folders.AsNoTracking().Where(x => x.Description == name).FirstOrDefault();
        }

        public string GetPath(string name)
        {
            return db.Folders.AsNoTracking().Where(x => x.Description == name).Select(x => x.Path).FirstOrDefault();            
        }

        public string Root()
        {            
            return HttpRuntime.AppDomainAppId != null ? AppDomain.CurrentDomain.BaseDirectory : Directory.GetParent(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).Parent.FullName;           
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
