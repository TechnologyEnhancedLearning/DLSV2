namespace DigitalLearningSolutions.Web.ViewComponents
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using Microsoft.AspNetCore.Mvc;

    public class CurrentFiltersViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            IBaseSearchablePageViewModel searchablePageViewModel
        )
        {
            var currentFilters = searchablePageViewModel.ExistingFilterString?.Split(FilteringHelper.FilterSeparator).ToList() ??
                                 new List<string>();

            var appliedFilters = currentFilters.Select(
                currentFilter => PopulateAppliedFilterViewModel(searchablePageViewModel, currentFilter)
            );

            var model = new CurrentFiltersViewModel(
                appliedFilters,
                searchablePageViewModel.SearchString,
                searchablePageViewModel.SortBy,
                searchablePageViewModel.SortDirection,
                searchablePageViewModel.ItemsPerPage,
                searchablePageViewModel.RouteData
            );

            return View(model);
        }

        private static AppliedFilterViewModel PopulateAppliedFilterViewModel(
            IBaseSearchablePageViewModel searchablePageViewModel,
            string currentFilter
        )
        {
            var appliedFilter = searchablePageViewModel.Filters.Single(
                filter => FilterOptionsContainsFilter(currentFilter, filter.FilterOptions)
            );

            return new AppliedFilterViewModel(
                GetFilterDisplayText(currentFilter, appliedFilter.FilterOptions),
                appliedFilter.FilterName,
                GetFilterValue(currentFilter, appliedFilter.FilterOptions)
            );
        }

        private static string GetFilterDisplayText(
            string currentFilter,
            IEnumerable<FilterOptionModel> filterOptions
        )
        {
            return filterOptions.First(filterOption => filterOption.FilterValue == currentFilter).DisplayText;
        }

        private static string GetFilterValue(
            string currentFilter,
            IEnumerable<FilterOptionModel> filterOptions
        )
        {
            return filterOptions.First(filterOption => filterOption.FilterValue == currentFilter).FilterValue;
        }

        private static bool FilterOptionsContainsFilter(
            string currentFilter,
            IEnumerable<FilterOptionModel> filterOptions
        )
        {
            return filterOptions.Any(filterOption => filterOption.FilterValue == currentFilter);
        }
    }
}
