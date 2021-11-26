namespace DigitalLearningSolutions.Web.Controllers.Support
{
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.Support.Faqs;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement;

    [SetDlsSubApplication]
    [SetSelectedTab(nameof(NavMenuTab.Support))]
    [Authorize(Policy = CustomPolicies.UserCentreAdminOrFrameworksAdmin)]
    [Route("/{dlsSubApplication}/Support/FAQs")]
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

        [Route("{page=1:int}")]
        public async Task<IActionResult> Index(
            DlsSubApplication dlsSubApplication,
            int page = 1,
            string? searchString = null,
            string sortBy = "Weighting,FaqId",
            string sortDirection = BaseSearchablePageViewModel.Descending
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

            var targetGroup = dlsSubApplication.FaqTargetGroupId;
            if (targetGroup == null)
            {
                return NotFound();
            }

            var faqs = faqsService.GetPublishedFaqsForTargetGroup(targetGroup.Value).Select(f => new FaqViewModel(f));

            var model = new FaqsViewModel(
                dlsSubApplication,
                SupportPage.Faqs,
                configuration.GetCurrentSystemBaseUrl(),
                faqs,
                page,
                searchString,
                sortBy,
                sortDirection
            );
            return View("Faqs", model);
        }

        [Route("View/{faqId:int}")]
        public async Task<IActionResult> ViewFaq(DlsSubApplication dlsSubApplication, int faqId)
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

            var targetGroup = dlsSubApplication.FaqTargetGroupId;
            if (targetGroup == null)
            {
                return NotFound();
            }

            var faq = faqsService.GetPublishedFaqByIdForTargetGroup(faqId, targetGroup.Value);

            if (faq == null)
            {
                return NotFound();
            }

            var model = new SearchableFaqViewModel(dlsSubApplication, new FaqViewModel(faq));

            return View(model);
        }

        [Route("AllItems")]
        public async Task<IActionResult> AllFaqItems(DlsSubApplication dlsSubApplication)
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

            var targetGroup = dlsSubApplication.FaqTargetGroupId;
            if (targetGroup == null)
            {
                return NotFound();
            }

            var faqs = faqsService.GetPublishedFaqsForTargetGroup(targetGroup.Value).Select(f => new FaqViewModel(f));

            var model = new FaqItemsViewModel(dlsSubApplication, faqs);

            return View(model);
        }
    }
}
