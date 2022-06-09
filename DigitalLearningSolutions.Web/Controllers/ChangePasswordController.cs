namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Route("/{dlsSubApplication}/ChangePassword", Order = 1)]
    [Route("/ChangePassword", Order = 2)]
    [TypeFilter(typeof(ValidateAllowedDlsSubApplication))]
    [SetDlsSubApplication]
    [SetSelectedTab(nameof(NavMenuTab.MyAccount))]
    [Authorize(Policy = CustomPolicies.BasicUser)]
    public class ChangePasswordController : Controller
    {
        private readonly IPasswordService passwordService;
        private readonly IUserVerificationService userVerificationService;

        public ChangePasswordController(
            IPasswordService passwordService,
            IUserVerificationService userVerificationService
        )
        {
            this.passwordService = passwordService;
            this.userVerificationService = userVerificationService;
        }

        [HttpGet]
        public IActionResult Index(DlsSubApplication dlsSubApplication)
        {
            var model = new ChangePasswordViewModel(dlsSubApplication);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(ChangePasswordFormData formData, DlsSubApplication dlsSubApplication)
        {
            var userId = User.GetUserId();
            var password = formData.CurrentPassword;

            if (!userVerificationService.IsPasswordValid(password, userId))
            {
                ModelState.AddModelError(
                    nameof(ChangePasswordFormData.CurrentPassword),
                    CommonValidationErrorMessages.IncorrectPassword
                );
            }

            if (!ModelState.IsValid)
            {
                var model = new ChangePasswordViewModel(formData, dlsSubApplication);
                return View(model);
            }

            var newPassword = formData.Password!;

            await passwordService.ChangePasswordAsync(userId!.Value, newPassword);

            return View("Success", dlsSubApplication);
        }
    }
}
