using X.PagedList.Mvc.Core;

namespace RPA.ClaimStatements.Web.Models
{
    public static class PagerOptions
    {
        // Same paging link labels as the PagedList.Mvc package used before.
        public static PagedListRenderOptions Legacy => new PagedListRenderOptions
        {
            LinkToFirstPageFormat = "««",
            LinkToPreviousPageFormat = "«",
            LinkToNextPageFormat = "»",
            LinkToLastPageFormat = "»»"
        };
    }
}
