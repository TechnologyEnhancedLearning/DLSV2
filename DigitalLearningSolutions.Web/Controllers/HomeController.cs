using Microsoft.AspNetCore.Mvc;

namespace DigitalLearningSolutions.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
