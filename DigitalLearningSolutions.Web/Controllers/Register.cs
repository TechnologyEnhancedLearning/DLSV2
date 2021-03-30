namespace DigitalLearningSolutions.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class Register : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}
