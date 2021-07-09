namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using DigitalLearningSolutions.Data.Models.SelfAssessments;

    public class SelfAssessmentCompetencyViewModel
    {
        public readonly SelfAssessment Assessment;
        public readonly ReviewedCompetency ReviewedCompetency;
        public readonly int CompetencyNumber;
        public readonly int TotalNumberOfCompetencies;

        public SelfAssessmentCompetencyViewModel(
            CurrentSelfAssessment assessment,
            ReviewedCompetency reviewedCompetency,
            int competencyNumber,
            int totalNumberOfCompetencies
        )
        {
            Assessment = assessment;
            ReviewedCompetency = reviewedCompetency;
            CompetencyNumber = competencyNumber;
            TotalNumberOfCompetencies = totalNumberOfCompetencies;
        }
    }
}
