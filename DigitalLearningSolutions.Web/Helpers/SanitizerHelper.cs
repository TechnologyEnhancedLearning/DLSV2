namespace DigitalLearningSolutions.Web.Helpers
{
    using Ganss.Xss;
    public static class SanitizerHelper
    {
        public static string SanitizeHtmlData(string htmlData)
        {
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedTags.Remove("iframe");
            sanitizer.AllowedTags.Add("svg");
            sanitizer.AllowedTags.Add("path");
            sanitizer.AllowedAttributes.Add("class");
            sanitizer.AllowedAttributes.Add("target");
            sanitizer.AllowedAttributes.Add("rel");
            sanitizer.AllowedAttributes.Add("xmlns");
            sanitizer.AllowedAttributes.Add("viewBox");
            sanitizer.AllowedAttributes.Add("aria-hidden");
            sanitizer.AllowedAttributes.Add("width");
            sanitizer.AllowedAttributes.Add("height");
            sanitizer.AllowedAttributes.Add("fill");
            sanitizer.AllowedSchemes.Add("https");
            sanitizer.AllowedAttributes.Add("d");
            sanitizer.AllowedAttributes.Add("style");

            var sanitized = sanitizer.Sanitize(htmlData);
            return sanitized;
        }
    }
}
