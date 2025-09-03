using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TravelExpenseTracker.Controllers
{
    [AllowAnonymous]
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LoginGoogle()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = Url.Action("Success")
            }, "Google");
        }

        public IActionResult LoginMicrosoft()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = Url.Action("Success")
            }, "OpenIdConnect");
        }

        public IActionResult Success()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return Content($"Success! Logged in as: {User.Identity.Name} via {User.FindFirst("iss")?.Value}");
            }
            return Content("Authentication failed");
        }

    }
}
