using DigitalLearningSolutions.Data.Models.CompetencyAssessments;

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
            Vocabulary = competencyAssessmentBase.Vocabulary;
        }
        public string CompetencyAssessmentName { get; set; }
        public int PublishStatusID { get; set; }
        public int UserRole { get; set; }
        public string? Vocabulary { get; set; }
        public CompetencyAssessmentTaskStatus CompetencyAssessmentTaskStatus { get; set; }
    }
}
