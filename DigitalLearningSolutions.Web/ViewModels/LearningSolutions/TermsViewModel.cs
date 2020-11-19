namespace DigitalLearningSolutions.Web.ViewModels.LearningSolutions
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
