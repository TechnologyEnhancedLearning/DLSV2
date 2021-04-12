namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.ForgotPassword;
    using Microsoft.AspNetCore.Mvc;

    public class ForgotPasswordController : Controller
    {
        private readonly IPasswordResetService passwordResetService;

        public ForgotPasswordController(IPasswordResetService passwordResetService)
        {
            this.passwordResetService = passwordResetService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Index(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string baseUrl = ConfigHelper.GetAppConfig()["AppRootPath"];

            try
            {
                passwordResetService.GenerateAndSendPasswordResetLink
                (
                    model.EmailAddress,
                    baseUrl
                );
            }
            catch (UserAccountNotFoundException)
            {
                ModelState.AddModelError("EmailAddress", "User with this email address does not exist. Please try again.");

                return View(model);
            }
            catch (ResetPasswordInsertException)
            {
                return RedirectToAction("Error", "LearningSolutions");
            }

            return RedirectToAction("Confirm");
        }

        public IActionResult Confirm()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}
