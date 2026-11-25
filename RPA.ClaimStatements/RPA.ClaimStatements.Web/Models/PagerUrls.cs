using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace RPA.ClaimStatements.Web.Models
{
    public static class PagerUrls
    {
        public static string Page(IUrlHelper url, string pageName, object values, int page)
        {
            // Url.Page reserves "page" for its route target, so append the legacy numeric
            // query parameter only after generating the target URL.
            return QueryHelpers.AddQueryString(url.Page(pageName, values), "page",
                page.ToString(CultureInfo.InvariantCulture));
        }
    }
}
