using DigitalLearningSolutions.Data.Models.CompetencyAssessments;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class EditDescriptionViewModel
    {
        public EditDescriptionViewModel() { }
        public EditDescriptionViewModel(CompetencyAssessmentBase competencyAssessmentBase, bool? taskStatus)
        {
            ID = competencyAssessmentBase.ID;
            CompetencyAssessmentName = competencyAssessmentBase.CompetencyAssessmentName;
            Description = competencyAssessmentBase.Description;
            UserRole = competencyAssessmentBase.UserRole;
            TaskStatus = taskStatus;
        }
        public int ID { get; set; }
        public string CompetencyAssessmentName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int UserRole { get; set; }
        public bool? TaskStatus { get; set; }
    }
}
