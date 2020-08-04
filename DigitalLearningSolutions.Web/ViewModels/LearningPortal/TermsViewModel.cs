namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using Microsoft.AspNetCore.Html;

    public class TermsViewModel
    {
        public HtmlString TermsText { get; }

        public TermsViewModel(string termsText)
        {
            this.TermsText = new HtmlString(termsText);
        }
    }
}
