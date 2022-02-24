namespace DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage
{
    public class AppliedFilterViewModel
    {
        public AppliedFilterViewModel(string displayText, string filterCategory, string filterValue)
        {
            DisplayText = displayText;
            FilterCategory = filterCategory;
            FilterValue = filterValue;
        }
        public AppliedFilterViewModel()
        {

        }

        public string DisplayText { get; set; }

        public string FilterCategory { get; set; }

        public string FilterValue { get; set; }
    }
}
