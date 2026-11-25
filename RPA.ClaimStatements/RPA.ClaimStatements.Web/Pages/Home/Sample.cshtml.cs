using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RPA.ClaimStatements.Generator.Services;

namespace RPA.ClaimStatements.Web.Pages.Home
{
    [IgnoreAntiforgeryToken]
    [Authorize(Roles = "Claim Statement Generator: View Claim Statements")]
    public class SampleModel : HomePageModel
    {
        private readonly ISampleService sampleService;
        private readonly ISampleXBService sampleXBService;

        public SampleModel(ISampleService sampleService, ISampleXBService sampleXBService)
        {
            this.sampleService = sampleService;
            this.sampleXBService = sampleXBService;
        }

        [BindProperty(SupportsGet = true)]
        public string statementType { get; set; }

        public IActionResult OnPost() => OnGet();

        public override IActionResult OnGet()
        {
            string path = statementType == "XB" ? sampleXBService.Sample() : sampleService.Sample();
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            System.IO.File.Delete(path);
            return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet,
                string.Format("Sample Claim Statement - {0}.docx", statementType));
        }
    }
}
