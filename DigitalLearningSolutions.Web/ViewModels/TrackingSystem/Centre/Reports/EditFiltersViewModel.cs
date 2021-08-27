namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports
{
    using System;
    using DigitalLearningSolutions.Data.Enums;

    public class EditFiltersViewModel
    {
        public int JobGroupId { get; set; }
        public int CourseCategoryId { get; set; }
        public int CustomisationId { get; set; }
        public int? StartDay { get; set; }
        public int? StartMonth { get; set; }
        public int? StartYear { get; set; }
        public int? EndDay { get; set; }
        public int? EndMonth { get; set; }
        public int? EndYear { get; set; }
        public ReportInterval ReportInterval { get; set; }
        public DateTime DataStart { get; set; }
        public bool CanFilterCourseCategories { get; set; }
    }
}
