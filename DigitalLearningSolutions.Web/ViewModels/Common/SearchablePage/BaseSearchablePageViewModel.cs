namespace DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public abstract class BaseSearchablePageViewModel : BasePaginatedViewModel
    {
        public const string DefaultSortOption = "SearchableName";
        public const string Descending = "Descending";
        public const string Ascending = "Ascending";

        public readonly string? FilterBy;

        public readonly bool FilterEnabled;

        public readonly string? SearchLabel;

        public readonly string? SearchString;

        protected BaseSearchablePageViewModel(
            string? searchString,
            int page,
            bool filterEnabled,
            string sortBy = DefaultSortOption,
            string sortDirection = Ascending,
            string? filterBy = null,
            int itemsPerPage = DefaultItemsPerPage,
            string? searchLabel = null,
            Dictionary<string, string>? routeData = null
        ) : base(page, itemsPerPage)
        {
            SortBy = sortBy;
            SortDirection = sortDirection;
            SearchString = searchString;
            SearchLabel = searchLabel;
            FilterBy = filterBy;
            FilterEnabled = filterEnabled;
            Filters = new List<FilterViewModel>();
            RouteData = routeData ?? new Dictionary<string, string>();
        }

        public string SortDirection { get; set; }

        public string SortBy { get; set; }

        public IEnumerable<SelectListItem> SortBySelectListItems =>
            SelectListHelper.MapOptionsToSelectListItems(SortOptions);

        public abstract IEnumerable<(string, string)> SortOptions { get; }

        public abstract bool NoDataFound { get; }

        public bool NoSearchOrFilter => SearchString == null && FilterBy == null;

        public IEnumerable<FilterViewModel> Filters { get; set; }

        public Dictionary<string, string> RouteData { get; set; }
    }
}
