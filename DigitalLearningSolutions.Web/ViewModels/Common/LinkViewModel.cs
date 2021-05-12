namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    public class LinkViewModel
    {
        public readonly string AspAction;

        public readonly string AspController;

        public LinkViewModel(string aspController, string aspAction)
        {
            AspAction = aspAction;
            AspController = aspController;
        }
    }
}
