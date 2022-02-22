namespace DigitalLearningSolutions.Web.ViewModels.Common.ViewComponents
{
    public class TopLinkViewModel
    {
        public readonly string LinkText;
        public readonly string TopElementId;
        public TopLinkViewModel(string linkText, string topElementId)
        {
            LinkText = linkText;
            TopElementId = topElementId;
        }
    }
}
