using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RPA.ClaimStatements.Web.Pages.Account
{
    [Authorize]
    public class LogoutModel : OperatorPageModel
    {
        public IActionResult OnPost()
        {
            return SignOut(new AuthenticationProperties { RedirectUri = Url.Page("/Home/Index") },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
