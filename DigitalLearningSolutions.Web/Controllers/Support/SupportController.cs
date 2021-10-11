namespace DigitalLearningSolutions.Web.Controllers.Support
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Support;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement;

    public class SupportController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IFeatureManager featureManager;

        public SupportController(IFeatureManager featureManager, IConfiguration configuration)
        {
            this.featureManager = featureManager;
            this.configuration = configuration;
        }

        [Route("/{application}/Support")]
        [Authorize(Policy = CustomPolicies.UserCentreAdminOrFrameworksAdmin)]
        public async Task<IActionResult> Index(ApplicationType application)
        {
            if (!ApplicationType.TrackingSystem.Equals(application) &&
                !ApplicationType.Frameworks.Equals(application))
            {
                return NotFound();
            }

            var trackingSystemSupportEnabled =
                ApplicationType.TrackingSystem.Equals(application) &&
                User.HasCentreAdminPermissions() &&
                await featureManager.IsEnabledAsync(FeatureFlags.RefactoredTrackingSystem);
            var frameworksSupportEnabled = ApplicationType.Frameworks.Equals(application) &&
                                           User.HasFrameworksAdminPermissions();

            if (trackingSystemSupportEnabled || frameworksSupportEnabled)
            {
                var model = new SupportViewModel(
                    application,
                    SupportPage.Support,
                    configuration.GetCurrentSystemBaseUrl()
                );
                return View("Support", model);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
