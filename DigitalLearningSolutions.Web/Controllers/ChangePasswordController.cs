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
    [Authorize]
    public class ChangePasswordController : Controller
    {
        private readonly IPasswordService passwordService;
        private readonly IUserService userService;

        public ChangePasswordController(IPasswordService passwordService, IUserService userService)
        {
            this.passwordService = passwordService;
            this.userService = userService;
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
            var adminId = User.GetAdminId();
            var delegateId = User.GetCandidateId();

            if (!ModelState.IsValid)
            {
                var model = new ChangePasswordViewModel(formData, dlsSubApplication);
                return View(model);
            }

            var verifiedLinkedUsersAccounts = string.IsNullOrEmpty(formData.CurrentPassword)
                ? new UserAccountSet()
                : userService.GetVerifiedLinkedUsersAccounts(adminId, delegateId, formData.CurrentPassword!);

            if (!verifiedLinkedUsersAccounts.Any())
            {
                ModelState.AddModelError(
                    nameof(ChangePasswordFormData.CurrentPassword),
                    CommonValidationErrorMessages.IncorrectPassword
                );
            }

            //if (!ModelState.IsValid)
            //{
            //    var model = new ChangePasswordViewModel(formData, dlsSubApplication);
            //    return View(model);
            //}

            var newPassword = formData.Password!;

            await passwordService.ChangePasswordAsync(verifiedLinkedUsersAccounts.GetUserRefs(), newPassword);

            return View("Success", dlsSubApplication);
        }
    }
}
