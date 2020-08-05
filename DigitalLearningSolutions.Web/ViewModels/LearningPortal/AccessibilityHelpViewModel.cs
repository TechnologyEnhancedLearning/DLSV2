namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using Microsoft.AspNetCore.Html;

    public class AccessibilityHelpViewModel
    {
        public HtmlString AccessibilityHelpText { get; }

        public AccessibilityHelpViewModel(string accessibilityText)
        {
            AccessibilityHelpText = new HtmlString(accessibilityText);
        }

    }
}
