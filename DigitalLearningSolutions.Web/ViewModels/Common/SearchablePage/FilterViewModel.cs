namespace DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage
{
    using System.Collections.Generic;

    public class FilterViewModel
    {
        public FilterViewModel(
            string filterProperty,
            string filterName,
            IEnumerable<FilterOptionViewModel> filterOptions
        )
        {
            FilterProperty = filterProperty;
            FilterName = filterName;
            FilterOptions = filterOptions;
        }

        public string FilterProperty { get; set; }

        public string FilterName { get; set; }

        public IEnumerable<FilterOptionViewModel> FilterOptions { get; set; }
    }
}
