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
        public IActionResult Index(ForgotPasswordViewModel? model)
        {
            model ??= new ForgotPasswordViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Index(string? emailAddress)
        {
            if (emailAddress?.Contains('@') != true)
            {
                ForgotPasswordViewModel model = GetEmailInvalidErrorModel(emailAddress ?? string.Empty);
                return Index(model);
            }

            string baseUrl = ConfigHelper.GetAppConfig()["AppRootPath"];

            try
            {
                passwordResetService.GenerateAndSendPasswordResetLink
                (
                    emailAddress,
                    baseUrl
                );
            }
            catch (EmailAddressNotFoundException)
            {
                ForgotPasswordViewModel model = GetEmailNotFoundErrorModel(emailAddress);
                return Index(model);
            }
            catch (ResetPasswordInsertException)
            {
                return RedirectToAction("Error", "LearningSolutions");
            }

            return RedirectToAction("Confirm");
        }

        public IActionResult Confirm()
        {
            return View();
        }

        private ForgotPasswordViewModel GetEmailInvalidErrorModel(string emailAddress)
        {
            return new ForgotPasswordViewModel
            (
                emailAddress,
                "The email address needs to be a valid email address, such as example@domain.com"
            );
        }

        private ForgotPasswordViewModel GetEmailNotFoundErrorModel(string emailAddress)
        {
            return new ForgotPasswordViewModel
            (
                emailAddress,
                "User with this email address does not exist. Please try again"
            );
        }
    }
}
