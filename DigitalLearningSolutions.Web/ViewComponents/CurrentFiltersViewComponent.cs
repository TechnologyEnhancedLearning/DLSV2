namespace DigitalLearningSolutions.Web.ViewComponents
{
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
                currentFilter => new AppliedFilterViewModel(
                    searchablePageViewModel.Filters
                        .Single(filter => filter.FilterOptions.Any(filterValue => filterValue.Filter == currentFilter))
                        .FilterOptions.Single(filterValue => filterValue.Filter == currentFilter).DisplayText,
                    searchablePageViewModel.Filters.Single(
                        filter => filter.FilterOptions.Any(filterValue => filterValue.Filter == currentFilter)
                    ).Filter.FilterName
                )
            );

            var model = new CurrentFiltersViewModel(appliedFilters, searchablePageViewModel.SearchString);

            return View(model);
        }
    }
}
