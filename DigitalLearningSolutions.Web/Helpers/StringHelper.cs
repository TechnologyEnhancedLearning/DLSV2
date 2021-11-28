namespace DigitalLearningSolutions.Web.Helpers
{
    using System.Text.RegularExpressions;

    public class StringHelper
    {
        public static string? ReplaceNoneAlphaNumericSpaceChars(string? input, string replacement)
        {
            if (input == null)
            {
                return input;
            }

            return Regex.Replace(input, "[^ a-zA-Z0-9]", replacement);
        }
    }
}
