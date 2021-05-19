namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    public class LinkViewModel
    {
        public readonly string AspAction;

        public readonly string AspController;

        public readonly string LinkText;

        public LinkViewModel(string aspController, string aspAction, string linkText)
        {
            AspAction = aspAction;
            AspController = aspController;
            LinkText = linkText;
        }
    }
}
