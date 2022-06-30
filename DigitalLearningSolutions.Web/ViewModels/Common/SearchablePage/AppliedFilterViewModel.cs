namespace DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage
{
    public class AppliedFilterViewModel
    {
        public AppliedFilterViewModel(string displayText, string filterCategory, string filterValue, string tagClass = "")
        {
            DisplayText = displayText;
            FilterCategory = filterCategory;
            FilterValue = filterValue;
            TagClass = tagClass;
        }
        public AppliedFilterViewModel()
        {

        }

        public string DisplayText { get; set; }

        public string FilterCategory { get; set; }

        public string FilterValue { get; set; }

        public string TagClass { get; set; }
    }
}
