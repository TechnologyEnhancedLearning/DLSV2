namespace DigitalLearningSolutions.Web.ViewModels.Common
{
    public class ActionLinkViewModel : LinkViewModel
    {
        public readonly string LinkText;

        public ActionLinkViewModel(string aspController, string aspAction, string linkText)
            : base(aspController, aspAction)
        {
            LinkText = linkText;
        }
    }
}
