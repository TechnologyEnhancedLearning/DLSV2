namespace DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage
{
    using System;
    using System.Collections.Generic;

    public class CurrentFiltersViewModel
    {
        [Obsolete("This is currently only used in SearchSelfAssessmentOvervieviewViewModel.cs, " +
                  "but this version has been superseded by the version with sortBy etc. " +
                  "parameters to fix a bug with Clear Filters resetting Sort and Items per page")]
        public CurrentFiltersViewModel(
            IEnumerable<AppliedFilterViewModel> filters,
            string? searchString,
            Dictionary<string, string> routeData
        )
        {
            AppliedFilters = filters;
            SearchString = searchString;
            RouteData = routeData;
        }

        public CurrentFiltersViewModel(
            IEnumerable<AppliedFilterViewModel> appliedFilters,
            string? searchString,
            string? sortBy,
            string? sortDirection,
            int itemsPerPage,
            Dictionary<string, string> routeData
        )
        {
            AppliedFilters = appliedFilters;
            SearchString = searchString;
            SortBy = sortBy;
            SortDirection = sortDirection;
            ItemsPerPage = itemsPerPage;
            RouteData = routeData;
        }

        public IEnumerable<AppliedFilterViewModel> AppliedFilters { get; set; }

        public string? SearchString { get; set; }

        public string? SortBy { get; set; }

        public string? SortDirection { get; set; }

        public int ItemsPerPage { get; set; }

        public Dictionary<string, string> RouteData { get; set; }
    }
}
