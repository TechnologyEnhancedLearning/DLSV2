namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Threading.Tasks;
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentPassword = model.CurrentPassword!;
            var newPassword = model.Password!;

            var adminId = User.GetAdminId();
            var candidateId = User.GetCandidateId();

            var passwordIsValid =
                userService.GetVerifiedLinkedUsersAccounts(adminId, candidateId, currentPassword).Any();

            if (!passwordIsValid)
            {
                ModelState.AddModelError(nameof(model.CurrentPassword), "The password you have entered is incorrect.");
                return View(model);
            }

            var (admin, candidate) = userService.GetUsersById(adminId, candidateId);
            await passwordService.ChangePasswordForLinkedUserAccounts(admin, candidate, newPassword);

            return View("Success");
        }
    }
}
