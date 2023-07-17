namespace DigitalLearningSolutions.Web.ViewModels.LearningSolutions
{
    using Microsoft.AspNetCore.Html;
    using System;

    public class TermsViewModel
    {
        public HtmlString TermsText { get; }
        public DateTime UpdateLastDate { get; set; }

        public TermsViewModel(string termsText, DateTime UpdateLastDate)
        {
            TermsText = new HtmlString(termsText);
            this.UpdateLastDate = UpdateLastDate;
        }
    }
}
