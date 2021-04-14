namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    public class BackLinkViewModel
    {
        public readonly string AspAction;

        public readonly string AspController;

        public BackLinkViewModel(string aspController, string aspAction)
        {
            AspAction = aspAction;
            AspController = aspController;
        }
    }
}
