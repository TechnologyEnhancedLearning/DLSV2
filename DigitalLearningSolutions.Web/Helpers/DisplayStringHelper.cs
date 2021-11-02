﻿namespace DigitalLearningSolutions.Web.Helpers
{
    using System;

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
            return GenerateBytesDisplayString(number) + Divider + GenerateBytesDisplayString(limit);
        }

        public static string GenerateBytesDisplayString(long byteCount)
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
            return (Math.Sign(byteCount) * number) + Units[place];
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
    }
}
