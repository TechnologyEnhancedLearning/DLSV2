namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.PlatformReports
{
    using DigitalLearningSolutions.Data.Models.PlatformReports;
    using DigitalLearningSolutions.Data.Models.TrackingSystem;
    using DigitalLearningSolutions.Web.Models.Enums;
    using System;
    using System.Collections.Generic;

    public class CourseUsageReportViewModel
    {
        public CourseUsageReportViewModel(
            IEnumerable<PeriodOfActivity> activity,
            CourseUsageReportFilterModel filterModel,
             DateTime startDate,
            DateTime endDate,
            bool hasActivity,
            string category
            )
        {
            CourseActivityTableViewModel = new CourseActivityTableViewModel(activity, startDate, endDate);
            FilterModel = filterModel;
            HasActivity = hasActivity;
            Category = category;
        }
        public SuperAdminReportsPage CurrentPage = SuperAdminReportsPage.CourseUsage;
        public CourseActivityTableViewModel CourseActivityTableViewModel { get; set; }
        public CourseUsageReportFilterModel FilterModel { get; set; }
        public bool HasActivity { get; set; }
        public string Category { get; set; }
    }
}
