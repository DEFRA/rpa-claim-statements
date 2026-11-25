using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RPA.ClaimStatements.Generator.Context;
using RPA.ClaimStatements.Generator.Services;
using X.PagedList;

namespace RPA.ClaimStatements.Web.Pages.Error
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
        public int pageSize { get; set; } = 10;
        public PagedList<Generator.Models.Entities.CS.Error> Items { get; private set; }

        public IActionResult OnPost() => OnGet();

        public override IActionResult OnGet()
        {
            var query = db.Errors.AsNoTracking().Where(x => !x.Resolved);
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(x => x.FRN.ToString() == searchString.Trim());
            }
            Items = new PagedList<Generator.Models.Entities.CS.Error>(
                query.OrderByDescending(x => x.ErrorDate), page, pageSize);
            ViewData["ForceGeneration"] = configurationService.IsActive("Force Generation");
            ViewData["searchData"] = searchString;
            return Page();
        }
    }
}
