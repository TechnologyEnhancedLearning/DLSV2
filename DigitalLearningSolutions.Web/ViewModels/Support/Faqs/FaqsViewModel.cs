namespace DigitalLearningSolutions.Web.ViewModels.Support.Faqs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.Support;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class FaqsViewModel : BaseSearchablePageViewModel
    {
        public FaqsViewModel(
            IEnumerable<Faq> faqs,
            int page,
            string? searchString
        ) : base(searchString, page, true, searchLabel: "Search faqs")
        {
            var searchedItems = GenericSearchHelper.SearchItems(faqs, SearchString).ToList();
            MatchingSearchResults = searchedItems.Count();
            SetTotalPages();
            var paginatedItems = GetItemsOnCurrentPage(searchedItems);

            Faqs = paginatedItems.Select(f => new SearchableFaqViewModel(f));
        }

        public IEnumerable<SearchableFaqViewModel> Faqs { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = Array.Empty<(string, string)>();

        public override bool NoDataFound => !Faqs.Any() && NoSearchOrFilter;
    }
}
