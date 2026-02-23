using DigitalLearningSolutions.Data.Models.CompetencyAssessments;
using DigitalLearningSolutions.Web.Helpers;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class ManageCompetencyAssessmentViewModel
    {
        public ManageCompetencyAssessmentViewModel(
            CompetencyAssessmentBase competencyAssessmentBase,
            CompetencyAssessmentTaskStatus competencyAssessmentTaskStatus,
            bool hasCompetencies
            )
        {
            CompetencyAssessmentName = competencyAssessmentBase.CompetencyAssessmentName;
            PublishStatusID = competencyAssessmentBase.PublishStatusID;
            UserRole = competencyAssessmentBase.UserRole;
            CompetencyAssessmentTaskStatus = competencyAssessmentTaskStatus;
            HasCompetencies = hasCompetencies;
            VocabularySingular = FrameworkVocabularyHelper.VocabularySingular(competencyAssessmentBase.Vocabulary);
            VocabularyPlural = FrameworkVocabularyHelper.VocabularyPlural(competencyAssessmentBase.Vocabulary);
            SelfAssessmentReviewID = competencyAssessmentBase.SelfAssessmentReviewID;
            SelfAssessmentCommentID = competencyAssessmentBase.SelfAssessmentCommentID;
        }
        public string CompetencyAssessmentName { get; set; }
        public int PublishStatusID { get; set; }
        public int UserRole { get; set; }
        public string VocabularySingular { get; set; }
        public string VocabularyPlural { get; set; }
        public CompetencyAssessmentTaskStatus CompetencyAssessmentTaskStatus { get; set; }
        public int? SelfAssessmentReviewID { get; set; }
        public int? SelfAssessmentCommentID { get; set; }
        public bool HasCompetencies { get; set; }
    }
}
