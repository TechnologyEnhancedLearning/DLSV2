namespace DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage
{
    using System.Collections.Generic;

    public class CurrentFiltersViewModel
    {
        public CurrentFiltersViewModel(IEnumerable<AppliedFilterViewModel> filters, string? searchString)
        {
            AppliedFilters = filters;
            SearchString = searchString;
        }

        public IEnumerable<AppliedFilterViewModel> AppliedFilters { get; set; }

        public string? SearchString { get; set; }
    }
}
