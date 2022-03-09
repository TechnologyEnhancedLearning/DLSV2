namespace DigitalLearningSolutions.Web.Controllers.Support
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
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
    [TypeFilter(
        typeof(ValidateAllowedDlsSubApplication),
        Arguments = new object[]
            { new[] { nameof(DlsSubApplication.TrackingSystem), nameof(DlsSubApplication.Frameworks) } }
    )]
    [Route("/{dlsSubApplication}/Support/FAQs")]
    public class FaqsController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IFaqsService faqsService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;

        public FaqsController(
            IConfiguration configuration,
            IFaqsService faqsService,
            ISearchSortFilterPaginateService searchSortFilterPaginateService
        )
        {
            this.configuration = configuration;
            this.faqsService = faqsService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
        }

        [Route("{page=1:int}")]
        public IActionResult Index(
            DlsSubApplication dlsSubApplication,
            int page = 1,
            string? searchString = null
        )
        {
            // A MatchCutOffScore of 65 is being used here rather than the default 80.
            // The default Fuzzy Search configuration does not reliably bring back expected FAQs.
            // Through trial and error a combination of the PartialTokenSetScorer ratio scorer
            // and this cut off score bring back reliable results comparable to the JS search.
            const int matchCutOffScore = 65;
            const string faqSortBy = "Weighting,FaqId";

            var faqs = faqsService.GetPublishedFaqsForTargetGroup(dlsSubApplication.FaqTargetGroupId!.Value)
                .Select(f => new SearchableFaq(f));

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                new SearchOptions(searchString, matchCutOffScore),
                new SortOptions(faqSortBy, GenericSortingHelper.Descending),
                null,
                new PaginationOptions(page)
            );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                faqs,
                searchSortPaginationOptions
            );

            var model = new FaqsPageViewModel(
                dlsSubApplication,
                SupportPage.Faqs,
                configuration.GetCurrentSystemBaseUrl(),
                result
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
