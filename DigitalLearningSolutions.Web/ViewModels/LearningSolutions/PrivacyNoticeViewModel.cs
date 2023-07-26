namespace DigitalLearningSolutions.Web.ViewModels.LearningSolutions
{
    using DigitalLearningSolutions.Web.Models;
    using Microsoft.AspNetCore.Html;
    using System;
    public class PrivacyNoticeViewModel : PageReviewModel
    {
        public HtmlString TermsText { get; }

        public PrivacyNoticeViewModel(string termsText, DateTime lastReviewedDate, DateTime nextReviewDate)
        {
            TermsText = new HtmlString(termsText);
            LastReviewedDate = lastReviewedDate;
            NextReviewDate = nextReviewDate;
        }
    }
}
