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

            string baseUrl = ConfigHelper.GetAppConfig().GetAppRootPath();

            try
            {
                passwordResetService.GenerateAndSendPasswordResetLink
                (
                    model.EmailAddress.Trim(),
                    baseUrl
                );
            }
            catch (UserAccountNotFoundException)
            {
                ModelState.AddModelError("EmailAddress", "A user with this email address could not be found");

                return View(model);
            }
            catch (ResetPasswordInsertException)
            {
                return new StatusCodeResult(500);
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
