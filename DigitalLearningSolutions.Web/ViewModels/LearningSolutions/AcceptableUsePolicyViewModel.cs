namespace DigitalLearningSolutions.Web.ViewModels.LearningSolutions
{
    using DigitalLearningSolutions.Web.Models;
    using Microsoft.AspNetCore.Html;
    using System;

    public class AcceptableUsePolicyViewModel: PageReviewModel
    {
        public HtmlString TermsText { get; }

        public AcceptableUsePolicyViewModel(string termsText, DateTime lastReviewedDate, DateTime nextReviewDate)
        {
            TermsText = new HtmlString(termsText);
            LastReviewedDate = lastReviewedDate;
            NextReviewDate = nextReviewDate;
        }
    }
}
