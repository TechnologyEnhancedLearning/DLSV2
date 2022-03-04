namespace DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface IBaseSearchablePageViewModel : IBasePaginatedViewModel
    {
        string? ExistingFilterString { get; }

        bool FilterEnabled { get; }

        string? SearchLabel { get; }

        string? SearchString { get; }

        string? SortBy { get; }

        string? SortDirection { get; }

        public IEnumerable<SelectListItem> SortBySelectListItems =>
            SelectListHelper.MapOptionsToSelectListItems(SortOptions);

        IEnumerable<(string, string)> SortOptions { get; }

        bool NoDataFound { get; }

        bool NoSearchOrFilter => SearchString == null && ExistingFilterString == null;

        IEnumerable<FilterModel> Filters { get; set; }

        Dictionary<string, string> RouteData { get; set; }
    }

    public abstract class BaseSearchablePageViewModel<T> : BasePaginatedViewModel<T>, IBaseSearchablePageViewModel
        where T : BaseSearchableItem
    {
        protected BaseSearchablePageViewModel(
            SearchSortFilterPaginateResult<T> searchSortFilterPaginateResult,
            bool filterEnabled,
            IEnumerable<FilterModel>? availableFilters = null,
            string? searchLabel = null,
            Dictionary<string, string>? routeData = null
        ) : base(searchSortFilterPaginateResult)
        {
            SortBy = searchSortFilterPaginateResult.SortBy;
            SortDirection = searchSortFilterPaginateResult.SortDirection;

            SearchString = searchSortFilterPaginateResult.SearchString;
            SearchLabel = searchLabel;

            ExistingFilterString = searchSortFilterPaginateResult.FilterString;
            FilterEnabled = filterEnabled;

            Filters = availableFilters ?? new List<FilterModel>();
            RouteData = routeData ?? new Dictionary<string, string>();
        }

        public string? ExistingFilterString { get; }
        public bool FilterEnabled { get; }
        public string? SearchLabel { get; }
        public string? SearchString { get; }
        public string? SortBy { get; }
        public string? SortDirection { get; }

        public IEnumerable<SelectListItem> SortBySelectListItems =>
            SelectListHelper.MapOptionsToSelectListItems(SortOptions);

        public abstract IEnumerable<(string, string)> SortOptions { get; }

        public abstract bool NoDataFound { get; }

        public bool NoSearchOrFilter => SearchString == null && ExistingFilterString == null;

        public IEnumerable<FilterModel> Filters { get; set; }

        public Dictionary<string, string> RouteData { get; set; }
    }
}
