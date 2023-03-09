namespace DigitalLearningSolutions.Web.Controllers.SuperAdmin.Users
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Users;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using System.Threading.Tasks;

    [FeatureGate(FeatureFlags.RefactoredSuperAdminInterface)]
    [Authorize(Policy = CustomPolicies.UserSuperAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.SuperAdmin))]
    [SetSelectedTab(nameof(NavMenuTab.Admins))]
    [Route("SuperAdmin/Users/AdminUserSetPassword/{userId:int}")]
    public class AdminUserSetPasswordController : Controller
    {
        private readonly IPasswordService passwordService;
        private readonly IUserService userService;
        public AdminUserSetPasswordController(IPasswordService passwordService, IUserService userService)
        {
            this.passwordService = passwordService;
            this.userService = userService;
        }
        [HttpGet]
        public IActionResult Index(int userId, DlsSubApplication dlsSubApplication)
        {
            var userEntity = userService.GetUserById(userId);
            TempData["UserID"] = userId;
            TempData["UserName"]= userEntity.UserAccount.FirstName+" "+ userEntity.UserAccount.LastName+" ("+userEntity.UserAccount.PrimaryEmail+")";
            var model = new SetAdminUserPasswordViewModel(dlsSubApplication);
            return View("AdminUserSetPassword", model);
        }
        
        [HttpPost]
        public async Task<IActionResult> Index(SetPasswordFormData formData, DlsSubApplication dlsSubApplication)
        {
            TempData.Peek("UserID");
            var userId = TempData["UserID"];
            

            if (!ModelState.IsValid)
            {
                var model = new SetAdminUserPasswordViewModel(formData, dlsSubApplication);
                return View("AdminUserSetPassword", model);
            }

            var newPassword = formData.Password!;

            await passwordService.ChangePasswordAsync((int)userId, newPassword);

            //Reload user account page here.Waiting for TD-992 to completed
            var model1 = new UserAccountsViewModel();
            return View("Index",model1);
        }
    }
}
