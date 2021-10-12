namespace DigitalLearningSolutions.Web.Controllers.SuperAdmin.Admins
{
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredSuperAdminInterface)]
    [Authorize(Policy = CustomPolicies.UserSuperAdmin)]
    [Route("SuperAdmin/Admins")]
    public class AdminsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
