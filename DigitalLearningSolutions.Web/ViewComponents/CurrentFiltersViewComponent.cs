﻿namespace DigitalLearningSolutions.Web.ViewComponents
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using Microsoft.AspNetCore.Mvc;

    public class CurrentFiltersViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(
            BaseSearchablePageViewModel searchablePageViewModel
        )
        {
            var currentFilters =
                NewlineSeparatedStringListHelper.SplitNewlineSeparatedList(searchablePageViewModel.FilterString);

            var appliedFilters = currentFilters.Select(
                currentFilter => PopulateAppliedFilterViewModel(searchablePageViewModel, currentFilter)
            );

            var model = new CurrentFiltersViewModel(appliedFilters, searchablePageViewModel.SearchString);

            return View(model);
        }

        private static AppliedFilterViewModel PopulateAppliedFilterViewModel(
            BaseSearchablePageViewModel searchablePageViewModel,
            string currentFilter
        )
        {
            var appliedFilter = searchablePageViewModel.Filters.Single(filter => FilterOptionsContainsFilter(currentFilter, filter.FilterOptions));

            return new AppliedFilterViewModel(
                GetFilterDisplayText(currentFilter, appliedFilter.FilterOptions),
                appliedFilter.FilterName,
                GetFilterValue(currentFilter, appliedFilter.FilterOptions)
            );
        }

        private static string GetFilterDisplayText(
            string currentFilter,
            IEnumerable<FilterOptionViewModel> filterOptions
        )
        {
            return filterOptions.Single(filterOption => filterOption.FilterValue == currentFilter).DisplayText;
        }

        private static string GetFilterValue(
            string currentFilter,
            IEnumerable<FilterOptionViewModel> filterOptions
        )
        {
            return filterOptions.Single(filterOption => filterOption.FilterValue == currentFilter).FilterValue;
        }

        private static bool FilterOptionsContainsFilter(
            string currentFilter,
            IEnumerable<FilterOptionViewModel> filterOptions
        )
        {
            return filterOptions.Any(filterOption => filterOption.FilterValue == currentFilter);
        }
    }
}
