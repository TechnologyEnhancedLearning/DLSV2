namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using Microsoft.AspNetCore.Mvc;

    public class ResetPasswordController : Controller
    {
        private readonly IPasswordResetService passwordResetService;

        public ResetPasswordController(IPasswordResetService passwordResetService)
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

            var matchingUserRefs = await passwordResetService.GetValidMatchingUserReferencesAsync(email, code);

            if (!matchingUserRefs.Any())
            {
                return RedirectToAction("Error");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Index(PasswordViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }
    }
}
