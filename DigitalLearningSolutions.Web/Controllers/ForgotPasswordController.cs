namespace DigitalLearningSolutions.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using DigitalLearningSolutions.Web.ViewModels.ForgotPassword;
    using DigitalLearningSolutions.Web.ViewModels.LearningSolutions;

    public class ForgotPasswordController : Controller
    {
        public IActionResult Index()
        {
            var model = new ForgotPasswordViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult RecoverEmail(string emailAddress)
        {
            // TODO: HEEDLS-354 - form submission logic

            return RedirectToAction("Confirm");
        }

        public IActionResult Confirm()
        {
            return View();
        }
    }
}
