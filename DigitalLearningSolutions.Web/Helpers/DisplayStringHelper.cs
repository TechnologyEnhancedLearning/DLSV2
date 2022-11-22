namespace DigitalLearningSolutions.Web.Helpers
{
    using System;
    using System.Text.RegularExpressions;

    public static class DisplayStringHelper
    {
        private const string Divider = " / ";
        private static readonly string[] Units = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };

        private static readonly Regex PascalRegex =
            new Regex(@"(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z\s])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z\s])");

        public static string FormatNumberWithLimit(int number, int limit)
        {
            return limit == -1 ? number.ToString() : number + Divider + limit;
        }

        public static string FormatBytesWithLimit(long number, long limit)
        {
            return GenerateSizeDisplayString(number) + Divider + GenerateSizeDisplayString(limit);
        }

        public static string GenerateSizeDisplayString(long byteCount)
        {
            if (byteCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(byteCount), $"Byte count cannot be negative: {byteCount}");
            }

            if (byteCount == 0)
            {
                return 0 + Units[0];
            }

            var place = Convert.ToInt32(Math.Floor(Math.Log(byteCount, 1024)));
            // Do not include decimal place below GB
            var decimalPlaces = place <= 2 ? 0 : 1;
            var number = Math.Round(byteCount / Math.Pow(1024, place), decimalPlaces);
            return Math.Sign(byteCount) * number + Units[place];
        }

        public static string? ConvertNumberToMonthsString(int numberOfMonths)
        {
            return numberOfMonths == 0 ? null : $"{numberOfMonths} month{GetPluralitySuffix(numberOfMonths)}";
        }

        public static string GetNonSortableFullNameForDisplayOnly(string? firstName, string lastName)
        {
            return (string.IsNullOrEmpty(firstName) ? "" : $"{firstName} ") + lastName;
        }

        public static string GetNameWithEmailForDisplay(string name, string? email)
        {
            return name + (string.IsNullOrWhiteSpace(email) ? "" : $" ({email})");
        }

        public static string? GetPotentiallyInactiveAdminName(string? firstName, string? lastName, bool? active)
        {
            return !string.IsNullOrEmpty(lastName)
                ? GetNonSortableFullNameForDisplayOnly(firstName, lastName) + (active == true ? "" : " (inactive)")
                : null;
        }

        public static string GetEmailDisplayString(string? email)
        {
            return !string.IsNullOrEmpty(email) ? $" ({email})" : "";
        }

        public static string GetPluralitySuffix(int number)
        {
            return number == 1 ? string.Empty : "s";
        }

        public static string? ReplaceNonAlphaNumericSpaceChars(string? input, string replacement)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            return Regex.Replace(input, "[^ a-zA-Z0-9]", replacement);
        }

        public static string GetTimeStringFromMinutes(int minutes)
        {
            return minutes < 60 ? $"{minutes}m" : $"{minutes / 60}h {minutes % 60}m";
        }

        public static string GetTimeStringForScreenReaderFromMinutes(int minutes)
        {
            return minutes < 60 ? $"{minutes} minutes" : $"{minutes / 60} hours {minutes % 60} minutes";
        }

        public static string AddSpacesToPascalCaseString(string pascalCaseString)
        {
            return PascalRegex.Replace(pascalCaseString, " ");
        }

        public static string Ellipsis(string text, int length)
        {
            if ((text ?? "").Trim().Length < length)
                return text;

            return string.Format("{0}...", text.Trim().Substring(0, length));
        }

        public static string RemoveMarkup(string input)
        {
            return Regex.Replace(input ?? String.Empty, $"<.*?>|&nbsp;", String.Empty);
        }
        public static bool IsGuid(string value)
        {
            Guid x;
            return Guid.TryParse(value, out x);
        }
        
    }
}
