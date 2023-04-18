namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Centre.Reports
{
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
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
