namespace DigitalLearningSolutions.Web.ViewModels.Support.Faqs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class FaqsViewModel : BaseSearchablePageViewModel, ISupportViewModel
    {
        private const int MatchCutOffScore = 65;
        private const string FaqSortBy = "Weighting,FaqId";

        public FaqsViewModel(
            DlsSubApplication dlsSubApplication,
            SupportPage currentPage,
            string currentSystemBaseUrl,
            IEnumerable<FaqViewModel> faqs,
            int page,
            string? searchString
        ) : base(searchString, page, false, FaqSortBy, Descending, searchLabel: "Search faqs")
        {
            CurrentPage = currentPage;
            DlsSubApplication = dlsSubApplication;
            SupportSideNavViewModel = new SupportSideNavViewModel(currentSystemBaseUrl);

            var sortedItems = GenericSortingHelper.SortAllItems(
                faqs.AsQueryable(),
                SortBy,
                SortDirection
            );

            var searchedItems = GenericSearchHelper.SearchItemsUsingTokeniseScorer(sortedItems, SearchString, MatchCutOffScore).ToList();
            MatchingSearchResults = searchedItems.Count;
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(searchedItems);

            Faqs = paginatedItems.Select(f => new SearchableFaqViewModel(DlsSubApplication, f));
        }

        public IEnumerable<SearchableFaqViewModel> Faqs { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = Array.Empty<(string, string)>();

        public override bool NoDataFound => !Faqs.Any() && NoSearchOrFilter;

        public SupportPage CurrentPage { get; set; }

        public DlsSubApplication DlsSubApplication { get; set; }

        public SupportSideNavViewModel SupportSideNavViewModel { get; set; }
    }
}
