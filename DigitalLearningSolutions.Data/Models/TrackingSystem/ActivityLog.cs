namespace DigitalLearningSolutions.Data.Models.TrackingSystem
{
    using System;

    public class ActivityLog
    {
        public DateTime LogDate { get; set; }
        public int LogYear { get; set; }
        public int LogQuarter { get; set; }
        public int LogMonth { get; set; }
        public bool Registered { get; set; }
        public bool Completed { get; set; }
        public bool Evaluated { get; set; }
    }
}
