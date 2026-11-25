using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Services;
using X.PagedList;

namespace RPA.ClaimStatements.Web.Pages.Log
{
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = "Claim Statement Generator: View Claim Statements")]
    public class IndexModel : OperatorPageModel
    {
        private readonly IClaimStatementsContext db;
        private readonly IConfigurationService configurationService;

        public IndexModel(IClaimStatementsContext db, IConfigurationService configurationService)
        {
            this.db = db;
            this.configurationService = configurationService;
        }

        [BindProperty(SupportsGet = true)]
        public string searchString { get; set; }
        [BindProperty(SupportsGet = true)]
        [ModelBinder(BinderType = typeof(LegacyPageNumberBinder))]
        public int page { get; set; } = 1;
        [BindProperty(SupportsGet = true)]
        public int pageSize { get; set; } = 15;
        public PagedList<Generator.Models.Entities.CS.Log> Items { get; private set; }

        public IActionResult OnPost() => OnGet();

        public override IActionResult OnGet()
        {
            var query = db.Log.AsNoTracking();
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(x => x.FRN.ToString() == searchString.Trim() || x.SBI.ToString() == searchString.Trim());
            }
            Items = new PagedList<Generator.Models.Entities.CS.Log>(
                query.OrderByDescending(x => x.ClaimProduced), page, pageSize);
            ViewData["searchData"] = searchString;
            ViewData["uploadActive"] = configurationService.IsActive("Manual Upload");
            return Page();
        }
    }
}
