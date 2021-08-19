namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports
{
    using System;
    using DigitalLearningSolutions.Data.Enums;

    public class EditFiltersViewModel
    {
        public int JobGroupId { get; set; }
        public int CourseCategoryId { get; set; }
        public int CustomisationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ReportInterval ReportInterval { get; set; }
    }
}
