namespace DigitalLearningSolutions.Data.Services
{
    using System;

    public interface IClockService
    {
        public DateTime UtcNow { get; }
        public DateTime UtcToday { get; }
    }

    public class ClockService : IClockService
    {
        public DateTime UtcNow => DateTime.UtcNow;
        public DateTime UtcToday => UtcNow.Date;
    }
}
