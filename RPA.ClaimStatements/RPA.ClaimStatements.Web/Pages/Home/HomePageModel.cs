using Microsoft.AspNetCore.Authorization;

namespace RPA.ClaimStatements.Web.Pages.Home
{
    [Authorize(Roles = "Claim Statement Generator: Trigger Generation, Claim Statement Generator: Manage Exceptions, Claim Statement Generator: Manage Suppressions, Claim Statement Generator: View Claim Statements, Claim Statement Generator: Manual Upload")]
    public abstract class HomePageModel : OperatorPageModel
    {
    }
}
