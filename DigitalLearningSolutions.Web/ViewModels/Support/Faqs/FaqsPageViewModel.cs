namespace DigitalLearningSolutions.Web.ViewModels.Support.Faqs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.Faqs;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class FaqsPageViewModel : BaseSearchablePageViewModel, ISupportViewModel
    {
        // A MatchCutOffScore of 60 is being used here rather than the default 80.
        // The default Fuzzy Search configuration does not reliably bring back expected FAQs.
        // Through trial and error a combination of the PartialTokenSetScorer ratio scorer
        // and this cut off score bring back reliable results comparable to the JS search.
        private const int MatchCutOffScore = 60;
        private const string FaqSortBy = "Weighting,FaqId";

        public FaqsPageViewModel(
            DlsSubApplication dlsSubApplication,
            SupportPage currentPage,
            string currentSystemBaseUrl,
            IEnumerable<SearchableFaq> faqs,
            int page,
            string? searchString
        ) : base(searchString, page, false, FaqSortBy, Descending, searchLabel: "Search faqs")
        {
            CurrentPage = currentPage;
            DlsSubApplication = dlsSubApplication;
            CurrentSystemBaseUrl = currentSystemBaseUrl;

            var searchedItems = GenericSearchHelper.SearchItemsUsingTokeniseScorer(faqs, SearchString, MatchCutOffScore, true).ToList();
            var faqsToShow = SortFilterAndPaginate(searchedItems);
            Faqs = faqsToShow.Select(f => new SearchableFaqViewModel(DlsSubApplication, f));
        }

        public IEnumerable<SearchableFaqViewModel> Faqs { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = Array.Empty<(string, string)>();

        public override bool NoDataFound => !Faqs.Any() && NoSearchOrFilter;

        public SupportPage CurrentPage { get; set; }

        public DlsSubApplication DlsSubApplication { get; set; }

        public string CurrentSystemBaseUrl { get; set; }
    }
}
