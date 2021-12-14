﻿namespace DigitalLearningSolutions.Web.Controllers.Support
{
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
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
    [Route("/{dlsSubApplication}/Support/FAQs")]
    public class FaqsController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IFaqsService faqsService;
        private readonly IFeatureManager featureManager;

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
            string? searchString = null
        )
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

            if (!trackingSystemSupportEnabled && !frameworksSupportEnabled)
            {
                return RedirectToAction("Index", "Home");
            }

            var faqs = faqsService.GetPublishedFaqsForTargetGroup(dlsSubApplication.FaqTargetGroupId!.Value)
                .Select(f => new FaqViewModel(f));

            var model = new FaqsViewModel(
                dlsSubApplication,
                SupportPage.Faqs,
                configuration.GetCurrentSystemBaseUrl(),
                faqs,
                page,
                searchString
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

            // TODO HEEDLS-608 If the user is centre admin but tracking system is off we need to show a 404
            // TODO HEEDLS-608 name these something appropriate
            var trackingSystemSupportEnabled =
                DlsSubApplication.TrackingSystem.Equals(dlsSubApplication) &&
                User.HasCentreAdminPermissions() &&
                await featureManager.IsEnabledAsync(FeatureFlags.RefactoredTrackingSystem);
            var frameworksSupportEnabled = DlsSubApplication.Frameworks.Equals(dlsSubApplication) &&
                                           User.HasFrameworksAdminPermissions();

            if (!trackingSystemSupportEnabled && !frameworksSupportEnabled)
            {
                return RedirectToAction("Index", "Home");
            }

            var faq = faqsService.GetPublishedFaqByIdForTargetGroup(faqId, dlsSubApplication.FaqTargetGroupId!.Value);

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

            // TODO HEEDLS-608 If the user is centre admin but tracking system is off we need to show a 404
            // TODO HEEDLS-608 name these something appropriate
            var trackingSystemSupportEnabled =
                DlsSubApplication.TrackingSystem.Equals(dlsSubApplication) &&
                User.HasCentreAdminPermissions() &&
                await featureManager.IsEnabledAsync(FeatureFlags.RefactoredTrackingSystem);
            var frameworksSupportEnabled = DlsSubApplication.Frameworks.Equals(dlsSubApplication) &&
                                           User.HasFrameworksAdminPermissions();

            if (!trackingSystemSupportEnabled && !frameworksSupportEnabled)
            {
                return RedirectToAction("Index", "Home");
            }

            var faqs = faqsService.GetPublishedFaqsForTargetGroup(dlsSubApplication.FaqTargetGroupId!.Value)
                .Select(f => new FaqViewModel(f));

            var model = new FaqItemsViewModel(dlsSubApplication, faqs);

            return View(model);
        }
    }
}
