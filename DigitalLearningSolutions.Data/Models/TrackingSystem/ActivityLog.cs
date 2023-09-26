namespace DigitalLearningSolutions.Data.Models.TrackingSystem
{
    using System;

    public class ActivityLog
    {
        public DateTime LogDate { get; set; }
        public int LogYear { get; set; }
        public int LogQuarter { get; set; }
        public int LogMonth { get; set; }
        public int Registered { get; set; }
        public int Completed { get; set; }
        public int Evaluated { get; set; }
    }
}
