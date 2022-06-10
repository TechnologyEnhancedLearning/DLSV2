namespace DigitalLearningSolutions.Web.Helpers
{
    using System;

    public static class StringHelper
    {
        public static bool StringsMatchCaseInsensitive(string? firstString, string? secondString)
        {
            return string.Equals(firstString, secondString, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
