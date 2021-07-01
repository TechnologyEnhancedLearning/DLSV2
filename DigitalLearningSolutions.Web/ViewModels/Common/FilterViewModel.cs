namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    using System.Collections.Generic;

    public class FilterViewModel
    {
        public FilterViewModel(
            string filterProperty,
            string filterName,
            IEnumerable<(string DisplayText, string Filter)> filterOptions
        )
        {
            FilterProperty = filterProperty;
            FilterName = filterName;
            FilterOptions = filterOptions;
        }

        public string FilterProperty { get; set; }

        public string FilterName { get; set; }

        public IEnumerable<(string DisplayText, string Filter)> FilterOptions { get; set; }
    }
}
