namespace DigitalLearningSolutions.Web.Controllers.Support
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Support.Faqs;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement;

    [SetDlsSubApplication]
    [SetSelectedTab(nameof(NavMenuTab.Support))]
    [Authorize(Policy = CustomPolicies.UserCentreAdminOrFrameworksAdmin)]
    public class FaqsController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IFeatureManager featureManager;
        private readonly IFaqsService faqsService;

        public FaqsController(IFeatureManager featureManager, IConfiguration configuration, IFaqsService faqsService)
        {
            this.featureManager = featureManager;
            this.configuration = configuration;
            this.faqsService = faqsService;
        }

        [Route("/{dlsSubApplication}/Support/FAQs")]
        public async Task<IActionResult> Index(
            DlsSubApplication dlsSubApplication,
            int page = 1,
            string? searchString = null
         )
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

            if (trackingSystemSupportEnabled && frameworksSupportEnabled)
            {
                return RedirectToAction("Index", "Home");
            }

            var targetGroup = FaqTargetGroup.FromDlsSubApplicationId(dlsSubApplication.Id);
            if (targetGroup == null)
            {
                return NotFound();
            }

            var faqs = faqsService.GetPublishedFaqsForTargetGroup(targetGroup.Id);

            var model = new FaqsViewModel(
                faqs,
                page,
                searchString
            );
            return View("Faqs", model);
        }
    }
}
