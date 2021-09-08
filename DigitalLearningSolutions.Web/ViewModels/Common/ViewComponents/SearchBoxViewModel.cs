namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;

    public class SearchBoxViewModel
    {
        
        public readonly BaseSearchablePageViewModel SearchablePageViewModel;
        public readonly string Label;

        public SearchBoxViewModel(
            BaseSearchablePageViewModel searchablePageViewModel,
            string label
        )
        {
            SearchablePageViewModel = searchablePageViewModel;
            Label = label;
        }
    }
}
