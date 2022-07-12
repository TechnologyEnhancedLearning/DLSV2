namespace DigitalLearningSolutions.Data.Utilities
{
    using System;

    public interface IClockUtility
    {
        public DateTime UtcNow { get; }
    }

    public class ClockUtility : IClockUtility
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
