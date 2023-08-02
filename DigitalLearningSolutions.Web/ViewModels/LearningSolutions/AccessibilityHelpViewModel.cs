namespace DigitalLearningSolutions.Web.ViewModels.LearningSolutions
{
    using DigitalLearningSolutions.Web.Models;
    using Microsoft.AspNetCore.Html;
    using System;

    public class AccessibilityHelpViewModel : PageReviewModel
    {
        public HtmlString AccessibilityHelpText { get; }

        public AccessibilityHelpViewModel(string accessibilityText, DateTime lastReviewedDate, DateTime nextReviewDate)
        {
            AccessibilityHelpText = new HtmlString(accessibilityText);
            LastReviewedDate = lastReviewedDate;
            NextReviewDate = nextReviewDate;
        }

    }
}
