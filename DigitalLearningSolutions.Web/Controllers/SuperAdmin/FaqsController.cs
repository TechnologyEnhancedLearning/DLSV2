namespace DigitalLearningSolutions.Web.Controllers.SuperAdmin
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.Faqs;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Faqs;

    [FeatureGate(FeatureFlags.RefactoredSuperAdminInterface)]
    [Authorize(Policy = CustomPolicies.UserSuperAdmin)]
    [Route("SuperAdmin/Faqs")]
    [SetDlsSubApplication(nameof(DlsSubApplication.SuperAdmin))]
    public class FaqsController : Controller
    {
        private readonly IFaqsService faqsService;

        public FaqsController(IFaqsService faqsService)
        {
            this.faqsService = faqsService;
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
    }
}
