using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
using DigitalLearningSolutions.Web.Helpers;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class CompetencyGroupDeleteViewModel
    {
        public CompetencyGroupDeleteViewModel(int competencyAssessmentId, int competencyGroupId, int competencyCount, string? Vocabulary)
        {
            CompetencyAssessmentId = competencyAssessmentId;
            CompetencyGroupId = competencyGroupId;
            CompetencyCount = competencyCount;
            VocabularySingular = FrameworkVocabularyHelper.VocabularySingular(Vocabulary);
            VocabularyPlural = FrameworkVocabularyHelper.VocabularyPlural(Vocabulary);
        }
        public int CompetencyAssessmentId { get; set; }
        public int CompetencyGroupId { get; set; }
        public int CompetencyCount { get; set; }
        public string? Vocabulary { get; set; }
        public string VocabularySingular { get; set; }
        public string VocabularyPlural { get; set; }
    }
}
