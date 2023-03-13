namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Web.Extensions;
    using Microsoft.AspNetCore.Mvc;

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
