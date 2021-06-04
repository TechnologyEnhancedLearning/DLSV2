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

        public ChangePasswordController(IPasswordService passwordService)
        {
            this.passwordService = passwordService;
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

            var email = User.GetEmail();
            var newPassword = model.Password!;
            if (email != null)
            {
                await passwordService.ChangePasswordAsync(email, newPassword);
            }
            else
            {
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
