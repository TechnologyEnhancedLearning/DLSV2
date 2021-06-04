namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Enums;
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
        private readonly ILoginService loginService;
        private readonly IUserService userService;

        public ChangePasswordController(
            IPasswordService passwordService,
            ILoginService loginService,
            IUserService userService
        )
        {
            this.passwordService = passwordService;
            this.loginService = loginService;
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentPassword = model.CurrentPassword!;
            var newPassword = model.Password!;

            var email = User.GetEmailIfAny();

            if (email != null)
            {
                var passwordIsValid = userService.GetVerifiedLinkedUsersAccounts(email, currentPassword).Any();

                if (!passwordIsValid)
                {
                    ModelState.AddModelError(
                        nameof(model.CurrentPassword),
                        "The password you have entered is incorrect."
                    );
                    return View(model);
                }

                await passwordService.ChangePasswordAsync(email, newPassword);
            }
            else
            {
                var passwordIsValid = userService.GetVerifiedLinkedUsersAccounts(
                    User.GetAdminId(),
                    User.GetCandidateId(),
                    currentPassword
                ).Any();

                if (!passwordIsValid)
                {
                    ModelState.AddModelError(
                        nameof(model.CurrentPassword),
                        "The password you have entered is incorrect."
                    );
                    return View(model);
                }

                await ChangePasswordForLoggedInUser(newPassword);
            }

            return View("Success");
        }

        private async Task ChangePasswordForLoggedInUser(string newPassword)
        {
            var adminId = User.GetAdminId();

            if (adminId.HasValue)
            {
                await passwordService.ChangePasswordAsync(
                    new UserReference(adminId.Value, UserType.AdminUser),
                    newPassword
                );
            }

            var candidateId = User.GetCandidateId();

            if (candidateId.HasValue)
            {
                await passwordService.ChangePasswordAsync(
                    new UserReference(candidateId.Value, UserType.DelegateUser),
                    newPassword
                );
            }
        }
    }
}
