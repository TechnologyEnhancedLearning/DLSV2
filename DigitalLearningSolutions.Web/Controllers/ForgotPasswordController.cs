namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.ForgotPassword;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    [SetSelectedTab(nameof(NavMenuTab.LogIn))]
    public class ForgotPasswordController : Controller
    {
        private readonly IPasswordResetService passwordResetService;
        private readonly IConfiguration config;

        public ForgotPasswordController(IPasswordResetService passwordResetService, IConfiguration config)
        {
            this.passwordResetService = passwordResetService;
            this.config = config;
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
        public async Task<IActionResult> Index(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var baseUrl = config.GetAppRootPath();

            try
            {
                await passwordResetService.GenerateAndSendPasswordResetLink(
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
