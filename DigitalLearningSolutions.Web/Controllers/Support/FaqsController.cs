namespace DigitalLearningSolutions.Web.Controllers.Support
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.Common.Faqs;
    using DigitalLearningSolutions.Web.ViewModels.Support.Faqs;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [SetDlsSubApplication]
    [SetSelectedTab(nameof(NavMenuTab.Support))]
    [TypeFilter(typeof(ValidateAllowedDlsSubApplication), Arguments = new object[] { new[] { nameof(DlsSubApplication.TrackingSystem), nameof(DlsSubApplication.Frameworks) } })]
    [Route("/{dlsSubApplication}/Support/FAQs")]
    public class FaqsController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IFaqsService faqsService;

        public FaqsController(IConfiguration configuration, IFaqsService faqsService)
        {
            this.configuration = configuration;
            this.faqsService = faqsService;
        }

        [Route("{page=1:int}")]
        public IActionResult Index(
            DlsSubApplication dlsSubApplication,
            int page = 1,
            string? searchString = null
        )
        {
            var faqs = faqsService.GetPublishedFaqsForTargetGroup(dlsSubApplication.FaqTargetGroupId!.Value)
                .Select(f => new SearchableFaq(f));

            var model = new FaqsPageViewModel(
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
        public IActionResult ViewFaq(DlsSubApplication dlsSubApplication, int faqId)
        {
            var faq = faqsService.GetPublishedFaqByIdForTargetGroup(faqId, dlsSubApplication.FaqTargetGroupId!.Value);

            if (faq == null)
            {
                return NotFound();
            }

            var model = new SearchableFaqViewModel(dlsSubApplication, new SearchableFaq(faq));

            return View(model);
        }

        [Route("AllItems")]
        public IActionResult AllFaqItems(DlsSubApplication dlsSubApplication)
        {
            var faqs = faqsService.GetPublishedFaqsForTargetGroup(dlsSubApplication.FaqTargetGroupId!.Value)
                .Select(f => new SearchableFaq(f));

            var model = new FaqItemsViewModel(dlsSubApplication, faqs);

            return View(model);
        }
    }
}
