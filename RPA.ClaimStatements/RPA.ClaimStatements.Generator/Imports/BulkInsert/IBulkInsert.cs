using System.Collections.Generic;

namespace RPA.ClaimStatements.Generator.Imports.BulkInsert
{
    public interface IBulkInsert<T> where T : class
    {
        void Insert(List<T> obj, string table, List<string> columnMappings = null);
    }
}