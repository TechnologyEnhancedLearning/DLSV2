namespace DigitalLearningSolutions.Web.Helpers
{
    using Ganss.XSS;
    public static class SanitizerHelper
    {
        public static string SanitizeHtmlData(string htmlData)
        {
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedTags.Remove("iframe");
            sanitizer.AllowedTags.Remove("img");
            var sanitized = sanitizer.Sanitize(htmlData);
            return sanitized;
        }
    }
}
