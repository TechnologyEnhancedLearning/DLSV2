namespace DigitalLearningSolutions.Web.Controllers.SuperAdmin
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Common.Faqs;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Faqs;
    using DigitalLearningSolutions.Data.Extensions;
    using Microsoft.Extensions.Configuration;

    [FeatureGate(FeatureFlags.RefactoredSuperAdminInterface)]
    [SetSelectedTab(nameof(NavMenuTab.System))]
    [Authorize(Policy = CustomPolicies.UserSuperAdmin)]
    [Route("SuperAdmin/System/Faqs")]
    [SetDlsSubApplication(nameof(DlsSubApplication.SuperAdmin))]
    public class SuperAdminFaqsController : Controller
    {
        private readonly IFaqsService faqsService;
        private readonly IConfiguration configuration;
        private readonly string legacyUrl;

        public SuperAdminFaqsController(IFaqsService faqsService, IConfiguration configuration)
        {
            this.faqsService = faqsService;
            this.configuration = configuration;
            legacyUrl = configuration.GetCurrentSystemBaseUrl();
        }

        public IActionResult Index()
        {
            var faqs = faqsService.GetAllFaqs()
                .OrderByDescending(f => f.CreatedDate)
                .Select(f => new SearchableFaq(f))
                .Take(10);

            var model = new FaqsPageViewModel(faqs);

            return View("SuperAdminFaqs", model);
        }
        public IActionResult ManageFaqs()
        {
            return RedirectToPage(legacyUrl + "tracking/admin-faqs");
        }
        public IActionResult Resources()
        {
            return RedirectToPage(legacyUrl + "tracking/admin-resources");
        }
        public IActionResult Notifications()
        {
            return RedirectToPage(legacyUrl + "tracking/admin-notifications");
        }
        public IActionResult Brands()
        {
            return RedirectToPage(legacyUrl + "tracking/admin-landing");
        }
    }
}
