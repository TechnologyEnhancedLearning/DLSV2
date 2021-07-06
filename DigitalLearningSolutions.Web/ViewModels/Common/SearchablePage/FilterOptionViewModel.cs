namespace DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage
{
    using DigitalLearningSolutions.Web.Models.Enums;

    public class FilterOptionViewModel
    {
        public FilterOptionViewModel(string displayText, string filterValue, FilterStatus tagStatus)
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
