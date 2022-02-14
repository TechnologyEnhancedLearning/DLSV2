namespace DigitalLearningSolutions.Web.Helpers
{
    using System;
    using System.Text.RegularExpressions;

    public static class DisplayStringHelper
    {
        private const string Divider = " / ";
        private static readonly string[] Units = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };

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

        public static string GetDelegateNameString(string? firstName, string lastName)
        {
            return (string.IsNullOrEmpty(firstName) ? "" : $"{firstName} ") + lastName;
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

        public static string GetFullNameDisplayString(string lastName, string? firstName)
        {
            return string.IsNullOrWhiteSpace(firstName) ? lastName : $"{lastName}, {firstName}";
        }
    }
}
