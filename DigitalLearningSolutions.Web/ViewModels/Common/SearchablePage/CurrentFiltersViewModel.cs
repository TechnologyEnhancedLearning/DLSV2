namespace DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage
{
    using System.Collections.Generic;

    public class CurrentFiltersViewModel
    {
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

        public IEnumerable<AppliedFilterViewModel> AppliedFilters { get; set; }

        public string? SearchString { get; set; }

        public Dictionary<string, string> RouteData { get; set; }
    }
}
