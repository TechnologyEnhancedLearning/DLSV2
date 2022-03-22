namespace DigitalLearningSolutions.Web.ViewModels.Support.Faqs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.Faqs;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class FaqsPageViewModel : BaseSearchablePageViewModel<SearchableFaq>, ISupportViewModel
    {
        public FaqsPageViewModel(
            DlsSubApplication dlsSubApplication,
            SupportPage currentPage,
            string currentSystemBaseUrl,
            SearchSortFilterPaginationResult<SearchableFaq> result
        ) : base(result, false, searchLabel: "Search faqs")
        {
            CurrentPage = currentPage;
            DlsSubApplication = dlsSubApplication;
            CurrentSystemBaseUrl = currentSystemBaseUrl;

            Faqs = result.ItemsToDisplay.Select(f => new SearchableFaqViewModel(DlsSubApplication, f));
        }

        public IEnumerable<SearchableFaqViewModel> Faqs { get; set; }

        public override IEnumerable<(string, string)> SortOptions { get; } = Array.Empty<(string, string)>();

        public override bool NoDataFound => !Faqs.Any() && NoSearchOrFilter;

        public SupportPage CurrentPage { get; set; }

        public DlsSubApplication DlsSubApplication { get; set; }

        public string CurrentSystemBaseUrl { get; set; }
    }
}
