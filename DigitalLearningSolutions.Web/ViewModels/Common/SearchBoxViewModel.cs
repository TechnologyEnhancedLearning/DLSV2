namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    public class SearchBoxViewModel
    {
        public readonly string AspAction;
        public readonly string AspController;
        public readonly BaseSearchablePageViewModel ContainingPageViewModel;
        public readonly string Label;

        public SearchBoxViewModel(
            string aspController,
            string aspAction,
            BaseSearchablePageViewModel containingPageViewModel,
            string label
        )
        {
            AspAction = aspAction;
            AspController = aspController;
            ContainingPageViewModel = containingPageViewModel;
            Label = label;
        }
    }
}
