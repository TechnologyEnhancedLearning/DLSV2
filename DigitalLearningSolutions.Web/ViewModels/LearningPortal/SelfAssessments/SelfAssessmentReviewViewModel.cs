namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;

    public class SelfAssessmentReviewViewModel
    {
        public CurrentSelfAssessment SelfAssessment { get; set; }
        public IEnumerable<IGrouping<string, ReviewedCompetency>> CompetencyGroups { get; set; }
        public int PreviousCompetencyNumber { get; set; }
    }
}
