namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    public class SearchBoxViewModel
    {
        public readonly string AspAction;
        public readonly string AspController;
        public readonly BaseSearchablePageViewModel SearchablePageViewModel;
        public readonly string Label;

        public SearchBoxViewModel(
            string aspController,
            string aspAction,
            BaseSearchablePageViewModel searchablePageViewModel,
            string label
        )
        {
            AspAction = aspAction;
            AspController = aspController;
            SearchablePageViewModel = searchablePageViewModel;
            Label = label;
        }
    }
}
