namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Text.RegularExpressions;

    public static class ContentViewerHelper
    {
        private static readonly Regex ScormRegex = new Regex(@".*imsmanifest\.xml$");
        public static bool IsScormPath(string path) => ScormRegex.IsMatch(path);
    }
}
