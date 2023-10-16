namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.PlatformReports
{
    using DigitalLearningSolutions.Data.Models.PlatformReports;
    using DigitalLearningSolutions.Web.Models.Enums;
    using System;
    using System.Collections.Generic;

    public class SelfAssessmentsReportViewModel
    {
        public SelfAssessmentsReportViewModel(
            IEnumerable<SelfAssessmentActivityInPeriod> activity,
            SelfAssessmentReportFilterModel filterModel,
             DateTime startDate,
            DateTime endDate,
            bool hasActivity,
            string category,
            bool supervised
            )
        {
            SelfAssessmentActivityTableViewModel = new SelfAssessmentActivityTableViewModel(activity, startDate, endDate);
            FilterModel = filterModel;
            HasActivity = hasActivity;
            Category = category;
            Supervised = supervised;
        }
        public SuperAdminReportsPage CurrentPage => Supervised ? SuperAdminReportsPage.SupervisedSelfAssessments : SuperAdminReportsPage.IndependentSelfAssessments;
        public SelfAssessmentActivityTableViewModel SelfAssessmentActivityTableViewModel { get; set; }
        public SelfAssessmentReportFilterModel FilterModel { get; set; }
        public bool HasActivity { get; set; }
        public string Category { get; set; }
        public bool Supervised { get; set; }
    }
}
