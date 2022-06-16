namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using DigitalLearningSolutions.Web.Helpers;
    using System.Collections.Generic;

    public class SelfAssessmentCompetencyViewModel
    {
        public readonly SelfAssessment Assessment;
        public readonly Competency Competency;
        public readonly int CompetencyNumber;
        public readonly int TotalNumberOfCompetencies;

        public SelfAssessmentCompetencyViewModel(
            CurrentSelfAssessment assessment,
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
        public string VocabPlural()
        {
            return FrameworkVocabularyHelper.VocabularyPlural(Competency.Vocabulary);
        }
    }
}
