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
        public FaqsViewModel(
            DlsSubApplication dlsSubApplication,
            SupportPage currentPage,
            string currentSystemBaseUrl,
            IEnumerable<FaqViewModel> faqs,
            int page,
            string? searchString,
            string sortBy,
            string sortDirection
        ) : base(searchString, page, false, sortBy, sortDirection, searchLabel: "Search faqs")
        {
            CurrentPage = currentPage;
            DlsSubApplication = dlsSubApplication;
            SupportSideNavViewModel = new SupportSideNavViewModel(currentSystemBaseUrl);

            var sortedItems = GenericSortingHelper.SortAllItems(
                faqs.AsQueryable(),
                sortBy,
                sortDirection
            );

            var searchedItems = GenericSearchHelper.SearchItemsUsingTokeniseScorer(sortedItems, SearchString, 65).ToList();
            MatchingSearchResults = searchedItems.Count();
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
