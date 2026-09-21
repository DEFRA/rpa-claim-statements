using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RPA.ClaimStatements.Web.Services
{
    public interface ISuppressionService
    {
        void BulkUpdate(string direction, string schemeYear, HttpPostedFileBase bulkSource);

        DataTable BulkData(HttpPostedFileBase bulkSource);

        void BulkAdd(DataTable bulkData, string schemeYear);

        void BulkRemove(DataTable bulkData, string schemeYear);
    }
}
