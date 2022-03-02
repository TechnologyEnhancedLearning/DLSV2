namespace DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage
{
    using DigitalLearningSolutions.Web.Models.Enums;

    public class FilterOptionViewModel
    {
        public FilterOptionViewModel(string displayText, string newFilterToAdd, FilterStatus tagStatus)
        {
            DisplayText = displayText;
            NewFilterToAdd = newFilterToAdd;
            TagStatus = tagStatus;
        }

        public string DisplayText { get; set; }

        public string NewFilterToAdd { get; set; }

        public FilterStatus TagStatus { get; set; }
    }
}
