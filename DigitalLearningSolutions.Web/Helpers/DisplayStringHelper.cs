namespace DigitalLearningSolutions.Web.Helpers
{
    using System;

    public static class DisplayStringHelper
    {
        private const string Divider = " / ";

        public static string GenerateNumberWithLimitDisplayString(int number, int limit)
        {
            return limit == -1 ? number.ToString() : number + Divider + limit;
        }

        public static string GenerateBytesLimitDisplayString(long number, long limit)
        {
            return GenerateBytesDisplayString(number) + Divider + GenerateBytesDisplayString(limit);
        }

        private static string GenerateBytesDisplayString(long byteCount)
        {
            var units = new[] { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB" };
            if (byteCount == 0)
            {
                return 0 + units[0];
            }

            var bytes = Math.Abs(byteCount);
            var place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            var number = Math.Round(bytes / Math.Pow(1024, place), 1);
            return (Math.Sign(byteCount) * number) + units[place];
        }
    }
}
