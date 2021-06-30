namespace DigitalLearningSolutions.Web.ViewComponents
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
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
            var (filterProperty, filterOptions) =
                searchablePageViewModel.Filters.Single(filter => FilterOptionsContainsFilter(currentFilter, filter));

            return new AppliedFilterViewModel(
                GetFilterDisplayText(currentFilter, filterOptions),
                filterProperty.FilterName
            );
        }

        private static string GetFilterDisplayText(
            string currentFilter,
            IEnumerable<(string DisplayText, string Filter)> filterOptions
        )
        {
            return filterOptions.Single(filterValue => filterValue.Filter == currentFilter).DisplayText;
        }

        private static bool FilterOptionsContainsFilter(
            string currentFilter,
            ((string FilterProperty, string FilterName) Filter, IEnumerable<(string DisplayText, string Filter)>
                FilterOptions) filter
        )
        {
            return filter.FilterOptions.Any(filterValue => filterValue.Filter == currentFilter);
        }
    }
}
