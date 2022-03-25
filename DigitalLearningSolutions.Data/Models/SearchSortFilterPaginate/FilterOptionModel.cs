namespace DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate
{
    using DigitalLearningSolutions.Data.Enums;

    public class FilterOptionModel
    {
        public FilterOptionModel(string displayText, string filterValue, FilterStatus tagStatus)
        {
            DisplayText = displayText;
            FilterValue = filterValue;
            TagStatus = tagStatus;
        }

        public string DisplayText { get; set; }

        public string FilterValue { get; set; }

        public FilterStatus TagStatus { get; set; }
    }
}
