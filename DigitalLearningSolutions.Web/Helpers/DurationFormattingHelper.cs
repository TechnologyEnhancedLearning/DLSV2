namespace DigitalLearningSolutions.Web.Helpers
{
    public class DurationFormattingHelper
    {
        public static string FormatDuration(int duration)
        {
            if (duration < 60)
            {
                return FormatTimeUnits(duration, "minute");
            }

            var durationMinutes = duration % 60;
            var formattedHours = FormatTimeUnits(duration / 60, "hour");
            var formattedMinutes = FormatTimeUnits(durationMinutes, "minute");
            return durationMinutes == 0 ? formattedHours : $"{formattedHours} {formattedMinutes}";

        }

        public static string? FormatNullableDuration(int? duration) =>
            duration == null ? null : FormatDuration(duration.Value);

        private static string FormatTimeUnits(int duration, string unit) =>
            duration == 1 ? $"{duration} {unit}" : $"{duration} {unit}s";
    }
}
