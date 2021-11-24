namespace DigitalLearningSolutions.Web.Controllers.Support
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Support;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement;

    [SetDlsSubApplication]
    [SetSelectedTab(nameof(NavMenuTab.Support))]
    public class SupportController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IFeatureManager featureManager;

        public SupportController(IFeatureManager featureManager, IConfiguration configuration)
        {
            this.featureManager = featureManager;
            this.configuration = configuration;
        }

        [Route("/{dlsSubApplication}/Support")]
        [Authorize(Policy = CustomPolicies.UserCentreAdminOrFrameworksAdmin)]
        public async Task<IActionResult> Index(DlsSubApplication dlsSubApplication)
        {
            if (!DlsSubApplication.TrackingSystem.Equals(dlsSubApplication) &&
                !DlsSubApplication.Frameworks.Equals(dlsSubApplication))
            {
                return NotFound();
            }

            var trackingSystemSupportEnabled =
                DlsSubApplication.TrackingSystem.Equals(dlsSubApplication) &&
                User.HasCentreAdminPermissions() &&
                await featureManager.IsEnabledAsync(FeatureFlags.RefactoredTrackingSystem);
            var frameworksSupportEnabled = DlsSubApplication.Frameworks.Equals(dlsSubApplication) &&
                                           User.HasFrameworksAdminPermissions();

            if (trackingSystemSupportEnabled || frameworksSupportEnabled)
            {
                var model = new SupportViewModel(
                    dlsSubApplication,
                    SupportPage.Support,
                    configuration.GetCurrentSystemBaseUrl()
                );
                return View("Support", model);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
