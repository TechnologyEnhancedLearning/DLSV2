namespace DigitalLearningSolutions.Web.Controllers.SuperAdmin.Users
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Users;
    using DigitalLearningSolutions.Web.ViewModels.UserCentreAccounts;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using System.Linq;
    [FeatureGate(FeatureFlags.RefactoredSuperAdminInterface)]
    [Authorize(Policy = CustomPolicies.UserSuperAdmin)]

    [SetDlsSubApplication(nameof(DlsSubApplication.SuperAdmin))]
    [SetSelectedTab(nameof(NavMenuTab.Admins))]
    public class UsersController : Controller
    {
        private readonly IUserService userService;
        private readonly IUserCentreAccountsService userCentreAccountsService;
        public UsersController(IUserCentreAccountsService userCentreAccountsService, IUserService userService)
        {
            this.userService = userService;
            this.userCentreAccountsService = userCentreAccountsService;
        }
        [Route("SuperAdmin/Users")]
        public IActionResult Index()
        {
            var model = new UserAccountsViewModel();
            return View(model);
        }
        [Route("SuperAdmin/Users/Administrators")]
        public IActionResult Administrators()
        {
            var model = new AdministratorsViewModel();
            return View(model);
        }
        [Route("SuperAdmin/Users/{userId:int}/CentreAccounts")]
        public IActionResult CentreAccounts(int userId)
        {
            TempData["UserID"] = userId;
            var userEntity = userService.GetUserById(userId);
            var UserCentreAccountsRoleViewModel =
                userCentreAccountsService.GetUserCentreAccountsRoleViewModel(userEntity);
            var model = new UserCentreAccountRoleViewModel(
                     UserCentreAccountsRoleViewModel.OrderByDescending(account => account.IsActiveAdmin)
                         .ThenBy(account => account.CentreName).ToList(),
                     userEntity
                 );
            return View("UserCentreAccounts", model);
        }
    }
}
