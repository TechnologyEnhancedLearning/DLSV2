namespace DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage
{
    using DigitalLearningSolutions.Web.Models.Enums;

    public class FilterOptionViewModel
    {
        public FilterOptionViewModel(string displayText, string filter, FilterStatus tagStatus)
        {
            DisplayText = displayText;
            Filter = filter;
            TagStatus = tagStatus;
        }

        public string DisplayText { get; set; }

        public string Filter { get; set; }

        public FilterStatus TagStatus { get; set; }
    }
}
