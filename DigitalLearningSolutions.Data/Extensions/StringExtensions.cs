using System.Collections.Generic;

namespace DigitalLearningSolutions.Data.Extensions
{
    public static class StringExtensions
    {
        public static bool ContainsAllStartWith(this string data, List<string> searchList)
        {
            foreach (string word in searchList)
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(data, $@"\b{word.Trim()}", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
