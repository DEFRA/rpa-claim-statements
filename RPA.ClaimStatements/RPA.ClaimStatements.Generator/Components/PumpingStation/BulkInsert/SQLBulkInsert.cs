using FastMember;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Services;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace RPA.ClaimStatements.Generator.Components.PumpingStation.BulkInsert
{
    public class SQLBulkInsert<T> : IBulkInsert<T>, IDisposable where T: class
    {
        IClaimStatementsContext db;
        string connStr = null;
        
        public SQLBulkInsert(IClaimStatementsContext context)
        {
            this.db = context;
            this.connStr = ClaimStatementsConnectionString.Resolve();
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
