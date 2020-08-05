namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using Microsoft.AspNetCore.Html;

    public class TermsViewModel
    {
        public HtmlString TermsText { get; }

        public TermsViewModel(string termsText)
        {
            TermsText = new HtmlString(termsText);
        }
    }
}
