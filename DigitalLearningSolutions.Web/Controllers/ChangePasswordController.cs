namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Index()
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(ChangePasswordViewModel model)
        {
            var adminId = User.GetAdminId();
            var delegateId = User.GetCandidateId();

            var verifiedLinkedUsersAccounts = string.IsNullOrEmpty(model.CurrentPassword)
                ? new UserAccountSet(null, null)
                : userService.GetVerifiedLinkedUsersAccounts(adminId, delegateId, model.CurrentPassword!);

            if (!verifiedLinkedUsersAccounts.Any())
            {
                ModelState.AddModelError(
                    nameof(ChangePasswordViewModel.CurrentPassword),
                    CommonValidationErrorMessages.IncorrectPassword
                );
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var newPassword = model.Password!;

            await passwordService.ChangePasswordAsync(verifiedLinkedUsersAccounts.GetUserRefs(), newPassword);

            return View("Success");
        }
    }
}
