namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.ApplicationSelector;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class ApplicationSelectorController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            var learningPortalAccess = User.GetCustomClaimAsBool(CustomClaimTypes.LearnUserAuthenticated) ?? false;
            var trackingSystemAccess = User.GetCustomClaimAsBool(CustomClaimTypes.UserCentreAdmin) ?? false;
            var contentManagementSystemAccess =
                User.GetCustomClaimAsBool(CustomClaimTypes.UserAuthenticatedCm) ?? false;
            var superviseAccess = User.GetCustomClaimAsBool(CustomClaimTypes.IsSupervisor) ?? false;
            var contentCreatorAccess = User.GetCustomClaimAsBool(CustomClaimTypes.UserCentreManager) ?? false;
            var frameworksAccess = User.GetCustomClaimAsBool(CustomClaimTypes.IsFrameworkDeveloper) ?? false;

            var model = new ApplicationSelectorViewModel(
                learningPortalAccess,
                trackingSystemAccess,
                contentManagementSystemAccess,
                superviseAccess,
                contentCreatorAccess,
                frameworksAccess);

            return View(model);
        }
    }
}
