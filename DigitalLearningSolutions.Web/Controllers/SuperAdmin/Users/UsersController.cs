namespace DigitalLearningSolutions.Web.Controllers.SuperAdmin.Users
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Users;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredSuperAdminInterface)]
    [Authorize(Policy = CustomPolicies.UserSuperAdmin)]

    [SetDlsSubApplication(nameof(DlsSubApplication.SuperAdmin))]
    [SetSelectedTab(nameof(NavMenuTab.Admins))]
    public class UsersController : Controller
    {
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
    }
}
