namespace DigitalLearningSolutions.Web.Controllers
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc;

    public class LogoutController : Controller
    {
        [HttpPost]
        public IActionResult Index()
        {
            HttpContext.SignOutAsync();
            HttpContext.Response.Cookies.Delete("ASP.NET_SessionId");
            return RedirectToAction("Index", "Home");
        }
    }
}
