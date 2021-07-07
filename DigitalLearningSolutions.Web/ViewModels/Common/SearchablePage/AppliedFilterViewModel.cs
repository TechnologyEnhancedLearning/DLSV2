namespace DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage
{
    public class AppliedFilterViewModel
    {
        public AppliedFilterViewModel(string displayText, string filterCategory)
        {
            DisplayText = displayText;
            FilterCategory = filterCategory;
        }

        public string DisplayText { get; set; }

        public string FilterCategory { get; set; }
    }
}
