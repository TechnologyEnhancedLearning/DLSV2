namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.SelfAssessmentReports
{
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using System.Collections;
    using System.Collections.Generic;

    public class SelfAssessmentReportsViewModel
    {
        public SelfAssessmentReportsViewModel(
            IEnumerable<SelfAssessmentSelect> selfAssessmentSelects
            )
        {
            SelfAssessmentSelects = selfAssessmentSelects;
        }

        public IEnumerable<SelfAssessmentSelect> SelfAssessmentSelects { get; set; }
    }
}
