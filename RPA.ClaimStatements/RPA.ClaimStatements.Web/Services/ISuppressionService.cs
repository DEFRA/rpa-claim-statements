using System.Data;
using Microsoft.AspNetCore.Http;

namespace RPA.ClaimStatements.Web.Services
{
    public interface ISuppressionService
    {
        void BulkUpdate(string direction, string schemeYear, IFormFile bulkSource);

        DataTable BulkData(IFormFile bulkSource);

        void BulkAdd(DataTable bulkData, string schemeYear);

        void BulkRemove(DataTable bulkData, string schemeYear);
    }
}
