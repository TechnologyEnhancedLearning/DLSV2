namespace DigitalLearningSolutions.Web.ViewModels.LearningSolutions
{
    using DigitalLearningSolutions.Web.Models;
    using Microsoft.AspNetCore.Html;
    using System;

    public class AcceptableUsePolicyViewModel: PageReviewModel
    {
        public HtmlString AcceptableUsePolicyText { get; }

        public AcceptableUsePolicyViewModel(string acceptableUsePolicyText, DateTime lastReviewedDate, DateTime nextReviewDate)
        {
            AcceptableUsePolicyText = new HtmlString(acceptableUsePolicyText);
            LastReviewedDate = lastReviewedDate;
            NextReviewDate = nextReviewDate;
        }
    }
}
