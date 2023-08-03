namespace DigitalLearningSolutions.Data.Models.PlatformReports
{
    using System;
    public class SelfAssessmentActivity
    {
        public DateTime ActivityDate { get; set; }
        public bool Enrolled { get; set; }
        public bool Completed { get; set; }
    }
}
