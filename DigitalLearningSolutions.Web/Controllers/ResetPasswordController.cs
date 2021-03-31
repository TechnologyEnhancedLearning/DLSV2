namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Services;
    using Microsoft.AspNetCore.Mvc;

    public class ResetPasswordController : Controller
    {
        private readonly IPasswordResetService passwordResetService;
        public ResetPasswordController(IPasswordResetService passwordResetService)
        {
            this.passwordResetService = passwordResetService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string email, string resetHash)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(resetHash))
            {
                return RedirectToAction("Index", "Login");
            }

            var requestIsValid = await passwordResetService.PasswordResetHashIsValidAsync(email, resetHash);

            if (!requestIsValid)
            {
                return RedirectToAction("Error");
            }

            return View();
        }

        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }
    }
}
