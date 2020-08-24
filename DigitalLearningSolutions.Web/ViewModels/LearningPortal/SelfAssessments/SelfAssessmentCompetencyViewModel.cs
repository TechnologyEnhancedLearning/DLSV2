namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using DigitalLearningSolutions.Data.Models;

    public class SelfAssessmentCompetencyViewModel
    {
        public readonly SelfAssessment Assessment;
        public readonly Competency Competency;
        public readonly int CompetencyNumber;
        public readonly int TotalNumberOfCompetencies;

        public SelfAssessmentCompetencyViewModel(
            SelfAssessment assessment,
            Competency competency,
            int competencyNumber,
            int totalNumberOfCompetencies
        )
        {
            Assessment = assessment;
            Competency = competency;
            CompetencyNumber = competencyNumber;
            TotalNumberOfCompetencies = totalNumberOfCompetencies;
        }
    }
}
