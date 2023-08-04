namespace DigitalLearningSolutions.Web.ViewModels.SuperAdmin.PlatformReports
{
    using DigitalLearningSolutions.Data.Models.PlatformReports;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using System;
    using System.Collections.Generic;

    public class SupervisedSelfAssessmentsReportViewModel
    {
        public SupervisedSelfAssessmentsReportViewModel(
            IEnumerable<SelfAssessmentActivityInPeriod> activity,
            NursingReportFilterModel filterModel,
             DateTime startDate,
            DateTime endDate,
            bool hasActivity,
            string category
            )
        {
            SelfAssessmentActivityTableViewModel = new SelfAssessmentActivityTableViewModel(activity, startDate, endDate);
            FilterModel = filterModel;
            HasActivity = hasActivity;
            Category = category;

        }
        public SuperAdminReportsPage CurrentPage => SuperAdminReportsPage.SupervisedSelfAssessments;
        public SelfAssessmentActivityTableViewModel SelfAssessmentActivityTableViewModel { get; set; }
        public NursingReportFilterModel FilterModel { get; set; }
        public bool HasActivity { get; set; }
        public string Category { get; set; }
    }
}
