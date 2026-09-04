using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
using DigitalLearningSolutions.Web.Helpers;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class CompetencyGroupDeleteViewModel
    {
        public CompetencyGroupDeleteViewModel(int competencyGroupId, int competencyCount, CompetencyAssessmentBase competencyAssessmentBase)
        {
            CompetencyAssessmentId = competencyAssessmentBase.ID;
            CompetencyGroupId = competencyGroupId;
            CompetencyCount = competencyCount;
            VocabularySingular = FrameworkVocabularyHelper.VocabularySingular(competencyAssessmentBase.Vocabulary);
            VocabularyPlural = FrameworkVocabularyHelper.VocabularyPlural(competencyAssessmentBase.Vocabulary);
            UserRole = competencyAssessmentBase.UserRole;
            PublishStatusID = competencyAssessmentBase.PublishStatusID;
        }
        public int CompetencyAssessmentId { get; set; }
        public int CompetencyGroupId { get; set; }
        public int CompetencyCount { get; set; }
        public string? Vocabulary { get; set; }
        public string VocabularySingular { get; set; }
        public string VocabularyPlural { get; set; }
        public int UserRole { get; set; }
        public int PublishStatusID { get; set; }
    }
}
