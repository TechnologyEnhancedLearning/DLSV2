namespace DigitalLearningSolutions.Data.Models.PlatformReports
{
    using System;
    public class SelfAssessmentActivity
    {
        public DateTime ActivityDate { get; set; }
        public int Enrolled { get; set; }
        public int Completed { get; set; }
    }
}
