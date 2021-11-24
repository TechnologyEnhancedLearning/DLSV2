namespace DigitalLearningSolutions.Web.Controllers.Support
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Support.Resources;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement;

    [Route("/{dlsSubApplication}/Resources")]
    [Authorize(Policy = CustomPolicies.UserCentreAdminOrFrameworksAdmin)]
    [SetDlsSubApplication]
    [SetSelectedTab(nameof(NavMenuTab.Support))]
    public class ResourcesController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IFeatureManager featureManager;
        private readonly IResourcesService resourcesService;

        public ResourcesController(
            IFeatureManager featureManager,
            IConfiguration configuration,
            IResourcesService resourcesService
        )
        {
            this.featureManager = featureManager;
            this.configuration = configuration;
            this.resourcesService = resourcesService;
        }

        public async Task<IActionResult> Index(DlsSubApplication dlsSubApplication)
        {
            if (!DlsSubApplication.TrackingSystem.Equals(dlsSubApplication) &&
                !DlsSubApplication.Frameworks.Equals(dlsSubApplication))
            {
                return NotFound();
            }

            // TODO HEEDLS-608 If the user is centre admin but tracking system is off we need to show a 404
            // TODO HEEDLS-608 name these something appropriate
            var trackingSystemSupportEnabled =
                DlsSubApplication.TrackingSystem.Equals(dlsSubApplication) &&
                User.HasCentreAdminPermissions() &&
                await featureManager.IsEnabledAsync(FeatureFlags.RefactoredTrackingSystem);
            var frameworksSupportEnabled = DlsSubApplication.Frameworks.Equals(dlsSubApplication) &&
                                           User.HasFrameworksAdminPermissions();

            if (trackingSystemSupportEnabled || frameworksSupportEnabled)
            {
                var model = new ResourcesViewModel(
                    dlsSubApplication,
                    SupportPage.Resources,
                    configuration.GetCurrentSystemBaseUrl(),
                    resourcesService.GetAllResources()
                );
                return View("Index", model);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
