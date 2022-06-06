namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using DigitalLearningSolutions.Web.Helpers;

    public class LogoutController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Index()
        {
            await HttpContext.Logout();
            return RedirectToAction("Index", "Home");
        }
    }
}
