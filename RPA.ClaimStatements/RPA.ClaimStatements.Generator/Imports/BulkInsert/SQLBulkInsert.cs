using FastMember;
using RPA.ClaimStatements.Data.Context;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPA.ClaimStatements.Generator.Imports.BulkInsert
{
    public class SQLBulkInsert<T> : IBulkInsert<T>, IDisposable where T: class
    {
        ClaimStatementsContext db = null;
        string connStr = null;

        public SQLBulkInsert()
        {
            this.db = new ClaimStatementsContext();
            this.connStr = ConfigurationManager.ConnectionStrings["ClaimStatementsContext"].ConnectionString;
        }

        public SQLBulkInsert(ClaimStatementsContext context)
        {
            this.db = context;
            this.connStr = ConfigurationManager.ConnectionStrings["ClaimStatementsContext"].ConnectionString;
        }

        public SQLBulkInsert(ClaimStatementsContext uow, string connStr)
        {
            this.db = uow;
            this.connStr = connStr;
        }

        public void Insert(List<T> obj, string table, List<string> columnMappings = null)
        {
            using (var bcp = new SqlBulkCopy(connStr))
            using (var reader = ObjectReader.Create(obj))
            {
                if(columnMappings != null)
                {
                    foreach (var mapping in columnMappings)
                    {
                        var split = mapping.Split(new[] { ',' });
                        bcp.ColumnMappings.Add(split.First(), split.Last());
                    }
                }

                bcp.DestinationTableName = table;
                bcp.BatchSize = 10000;
                bcp.BulkCopyTimeout = 60;
                bcp.WriteToServer(reader);
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
