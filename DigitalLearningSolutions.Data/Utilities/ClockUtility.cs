namespace DigitalLearningSolutions.Data.Utilities
{
    using System;

    public interface IClockUtility
    {
        public DateTime UtcNow { get; }
        public DateTime UtcToday { get; }
    }

    public class ClockUtility : IClockUtility
    {
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTime UtcToday => UtcNow.Date;
    }
}
