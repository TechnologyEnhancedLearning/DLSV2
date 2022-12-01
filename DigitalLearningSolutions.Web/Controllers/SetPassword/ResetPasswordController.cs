namespace DigitalLearningSolutions.Web.Controllers.SetPassword
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Mvc;

    public class ResetPasswordController : Controller
    {
        private readonly IPasswordResetService passwordResetService;

        public ResetPasswordController(
            IPasswordResetService passwordResetService
        )
        {
            this.passwordResetService = passwordResetService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string email, string code)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
            {
                return RedirectToAction("Index", "Login");
            }

            var hashIsValid = await passwordResetService.EmailAndResetPasswordHashAreValidAsync(
                email,
                code,
                ResetPasswordHelpers.ResetPasswordHashExpiryTime
            );

            TempData.Set(new ResetPasswordData(email, code));

            if (!hashIsValid)
            {
                return RedirectToAction("Error");
            }

            return View(new ConfirmPasswordViewModel());
        }

        [HttpPost]
        [ServiceFilter(typeof(RedirectEmptySessionData<ResetPasswordData>))]
        public async Task<IActionResult> Index(ConfirmPasswordViewModel viewModel)
        {
            var resetPasswordData = TempData.Peek<ResetPasswordData>()!;

            var passwordReset = await passwordResetService.GetValidPasswordResetEntityAsync(
                resetPasswordData.Email,
                resetPasswordData.ResetPasswordHash,
                ResetPasswordHelpers.ResetPasswordHashExpiryTime
            );

            if (passwordReset == null)
            {
                TempData.Clear();
                return RedirectToAction("Error");
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            await passwordResetService.ResetPasswordAsync(passwordReset, viewModel.Password!);

            TempData.Clear();

            return View("Success");
        }

        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }
    }
}
