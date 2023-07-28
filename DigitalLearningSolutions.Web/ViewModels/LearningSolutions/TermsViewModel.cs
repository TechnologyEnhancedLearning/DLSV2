namespace DigitalLearningSolutions.Web.ViewModels.LearningSolutions
{
    using Microsoft.AspNetCore.Html;
    using System;
    using DigitalLearningSolutions.Web.Models;
    public class TermsViewModel:PageReviewModel
    {
        public HtmlString TermsText { get; }
        public TermsViewModel(string termsText, DateTime lastReviewedDate, DateTime nextReviewDate)
        {
            TermsText = new HtmlString(termsText);
            LastReviewedDate = lastReviewedDate;
            NextReviewDate = nextReviewDate;
        }
    }
}
