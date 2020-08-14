namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal
{
    using DigitalLearningSolutions.Data.Models;

    public class SelfAssessmentCompetencyViewModel
    {
        public readonly SelfAssessment Assessment;
        public readonly Competency Competency;
        public readonly int CompetencyNumber;

        public SelfAssessmentCompetencyViewModel(SelfAssessment assessment, Competency competency, int competencyNumber)
        {
            Assessment = assessment;
            Competency = competency;
            CompetencyNumber = competencyNumber;
        }
    }
}
