namespace DigitalLearningSolutions.Web.Controllers.SuperAdmin
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.SuperAdmin.Faqs;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

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

        [Route("All")]
        public IActionResult AllFaqItems()
        {
            var faqs = faqsService.GetAllFaqs()
                .OrderByDescending(f => f.CreatedDate)
                .Select(f => new SearchableFaqModel(f))
                .Take(10);

            var model = new FaqsPageViewModel(faqs);

            return View("SuperAdminFaqs", model);
        }
    }
}
