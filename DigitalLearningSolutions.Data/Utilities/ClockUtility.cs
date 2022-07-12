namespace DigitalLearningSolutions.Data.Utilities
{
    using System;

    public interface IClockService
    {
        public DateTime UtcNow { get; }
    }

    public class ClockService : IClockService
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
