namespace DigitalLearningSolutions.Web.Helpers
{
    using System;

    public static class DisplayStringHelper
    {
        private const string Divider = " / ";
        private static readonly string[] Units = { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB" };

        public static string FormatNumberWithLimit(int number, int limit)
        {
            return limit == -1 ? number.ToString() : number + Divider + limit;
        }

        public static string FormatBytesWithLimit(long number, long limit)
        {
            return GenerateBytesDisplayString(number) + Divider + GenerateBytesDisplayString(limit);
        }

        private static string GenerateBytesDisplayString(long byteCount)
        {
            if (byteCount == 0)
            {
                return 0 + Units[0];
            }

            var bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var number = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * number) + Units[place];
        }
    }
}
