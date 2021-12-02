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

    [Route("/{dlsSubApplication}/Support/Tickets")]
    [Authorize(Policy = CustomPolicies.UserCentreAdminOrFrameworksAdmin)]
    [SetDlsSubApplication]
    [SetSelectedTab(nameof(NavMenuTab.Support))]
    public class SupportTicketsController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IFeatureManager featureManager;

        public SupportTicketsController(IFeatureManager featureManager, IConfiguration configuration)
        {
            this.featureManager = featureManager;
            this.configuration = configuration;
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
                var model = new SupportTicketsViewModel(
                    dlsSubApplication,
                    SupportPage.SupportTickets,
                    configuration.GetCurrentSystemBaseUrl()
                );
                return View("Index", model);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
