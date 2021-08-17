namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Mvc;

    public class SetPasswordController : SetPasswordControllerBase
    {
        public SetPasswordController(IPasswordResetService passwordResetService, IPasswordService passwordService) :
            base(passwordResetService, passwordService) { }

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

            var hashIsValid = await IsSetPasswordLinkValid(email, code, true);

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

            var hashIsValid = await IsSetPasswordLinkValid(resetPasswordData, true);

            if (!hashIsValid)
            {
                TempData.Clear();
                return RedirectToAction("Error");
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            await ChangePassword(viewModel, resetPasswordData);

            return View("Success");
        }

        

        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }
    }
}
