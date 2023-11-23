namespace DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate
{
    using System.Collections.Generic;

    public class FilterModel
    {
        public FilterModel(
            string filterProperty,
            string filterName,
            IEnumerable<FilterOptionModel> filterOptions
        )
        {
            FilterProperty = filterProperty;
            FilterName = filterName;
            FilterOptions = filterOptions;
        }

        public string FilterProperty { get; set; }

        public string FilterName { get; set; }

        public IEnumerable<FilterOptionModel> FilterOptions { get; set; }
    }
}
