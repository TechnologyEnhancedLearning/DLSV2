namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

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
            var user = userVerificationService.GetUserAccountById((int)userId);
            RegistrationPasswordValidator.ValidatePassword(formData.Password, user.FirstName, user.LastName, ModelState);

            if (!ModelState.IsValid)
            {
                var model = new ChangePasswordViewModel(formData, dlsSubApplication);
                return View(model);
            }

            if (!userVerificationService.IsPasswordValid(formData.CurrentPassword, userId))
            {
                ModelState.AddModelError(
                    nameof(ChangePasswordFormData.CurrentPassword),
                    CommonValidationErrorMessages.IncorrectPassword
                );
                return View(new ChangePasswordViewModel(formData, dlsSubApplication));
            }


            var newPassword = formData.Password!;

            await passwordService.ChangePasswordAsync(userId!.Value, newPassword);

            return View("Success", dlsSubApplication);
        }
    }
}
