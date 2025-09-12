using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelExpenseTracker.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action("LoginCallback", "Account", new { returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> LoginCallback(string returnUrl = null)
        {
            var result = await HttpContext.AuthenticateAsync();

            if (result?.Succeeded != true)
            {
                return RedirectToAction("Login");
            }

            return LocalRedirect(returnUrl ?? "/");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            var scheme = User.FindFirst("iss")?.Value?.Contains("microsoft") == true
                ? OpenIdConnectDefaults.AuthenticationScheme
                : CookieAuthenticationDefaults.AuthenticationScheme;

            await HttpContext.SignOutAsync(scheme);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
        public override SignOutResult SignOut(AuthenticationProperties properties)
        {
            properties.RedirectUri = Url.Content("~/"); // after the IdP calls back to /signout-callback-oidc
            return base.SignOut(properties);
        }
     

    }
}
