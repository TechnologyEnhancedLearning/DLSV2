using DigitalLearningSolutions.Data.Models.SelfAssessments;
using DigitalLearningSolutions.Web.Helpers;

namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    public class AddOptionalCompetenciesViewModel
    {
        public CurrentSelfAssessment SelfAssessment { get; set; }
        public string VocabPlural()
        {
            return FrameworkVocabularyHelper.VocabularyPlural(SelfAssessment.Vocabulary);
        }
    }
}
