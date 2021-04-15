namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Web.ViewModels.Register;
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

        [HttpPost]
        public IActionResult Index(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            return View("LearnerInformation", model);
        }

        public IActionResult LearnerInformation()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}
