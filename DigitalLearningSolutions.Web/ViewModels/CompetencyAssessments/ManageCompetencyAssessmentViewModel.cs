using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
using DigitalLearningSolutions.Web.Helpers;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class ManageCompetencyAssessmentViewModel
    {
        public ManageCompetencyAssessmentViewModel(
            CompetencyAssessmentBase competencyAssessmentBase,
            CompetencyAssessmentTaskStatus competencyAssessmentTaskStatus
            )
        {
            CompetencyAssessmentName = competencyAssessmentBase.CompetencyAssessmentName;
            PublishStatusID = competencyAssessmentBase.PublishStatusID;
            UserRole = competencyAssessmentBase.UserRole;
            CompetencyAssessmentTaskStatus = competencyAssessmentTaskStatus;
            VocabularySingular = FrameworkVocabularyHelper.VocabularySingular(competencyAssessmentBase.Vocabulary);
            VocabularyPlural = FrameworkVocabularyHelper.VocabularyPlural(competencyAssessmentBase.Vocabulary);
        }
        public string CompetencyAssessmentName { get; set; }
        public int PublishStatusID { get; set; }
        public int UserRole { get; set; }
        public string VocabularySingular { get; set; }
        public string VocabularyPlural { get; set; }
        public CompetencyAssessmentTaskStatus CompetencyAssessmentTaskStatus { get; set; }
    }
}
