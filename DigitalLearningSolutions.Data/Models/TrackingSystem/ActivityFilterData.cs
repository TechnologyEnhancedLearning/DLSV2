namespace DigitalLearningSolutions.Data.Models.TrackingSystem
{
    using System;
    using DigitalLearningSolutions.Data.Enums;

    public class ActivityFilterData
    {
        public int JobGroupId { get; set; }
        public int CourseCategoryId { get; set; }
        public int CustomisationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ReportInterval ReportInterval { get; set; }
    }
}
