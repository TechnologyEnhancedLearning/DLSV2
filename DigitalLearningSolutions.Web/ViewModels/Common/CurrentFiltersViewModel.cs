namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using System.Collections.Generic;

    public class CurrentFiltersViewModel
    {
        public CurrentFiltersViewModel(IEnumerable<AppliedFilterViewModel> filters)
        {
            AppliedFilters = filters;
        }

        public IEnumerable<AppliedFilterViewModel> AppliedFilters { get; set; }
    }
}
