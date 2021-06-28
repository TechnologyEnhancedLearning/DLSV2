namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    public class SearchBoxViewModel
    {
        public readonly string AspAction;
        public readonly string AspController;
        public readonly BaseSearchablePageViewModel SearchablePageViewModel;
        public readonly string Label;
        public readonly string? Class;

        public SearchBoxViewModel(
            string aspController,
            string aspAction,
            BaseSearchablePageViewModel searchablePageViewModel,
            string label,
            string? cssClass
        )
        {
            AspAction = aspAction;
            AspController = aspController;
            SearchablePageViewModel = searchablePageViewModel;
            Label = label;
            Class = cssClass;
        }
    }
}
