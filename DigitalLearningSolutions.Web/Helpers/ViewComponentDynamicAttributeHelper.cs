namespace DigitalLearningSolutions.Web.Helpers
{
    public static class ViewComponentDynamicAttributeHelper
    {
        public static string? GetAriaDescribedByAttribute(this string name, bool hasError, string? hintText)
        {
            string? describedBy = hasError ? name + "-error" : null;
            if (hintText != null)
            {
                describedBy = describedBy == null ? "" : describedBy += " ";
                describedBy += name + "-hint";
            }
            return describedBy;
        }
    }
}
