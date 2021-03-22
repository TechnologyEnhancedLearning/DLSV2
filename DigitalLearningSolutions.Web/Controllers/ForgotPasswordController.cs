namespace DigitalLearningSolutions.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using DigitalLearningSolutions.Web.ViewModels.ForgotPassword;
    using DigitalLearningSolutions.Web.ViewModels.LearningSolutions;

    public class ForgotPasswordController : Controller
    {

        [Route("/ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            var model = new ForgotPasswordViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult SubmitEmail(string emailAddress)
        {
            //Do some validation
            ConfirmationViewModel confirmationModel = GetForgotPasswordConfirmationViewModel();

            return View("~/Views/LearningSolutions/Confirmation.cshtml", confirmationModel);
        }

        private ConfirmationViewModel GetForgotPasswordConfirmationViewModel()
        {
            string pageTitle = "Password Reset Email Sent";
            string[] pageDescription =
            {
                "An email has been sent to you giving details of how to reset your password.",
                "The link provided in that email will expire in two hours.",
                "If you have not received an email, please contact your centre administrator."
            };

            return new ConfirmationViewModel(pageTitle, pageDescription);
        }
    }
}
