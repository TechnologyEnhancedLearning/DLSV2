using DigitalLearningSolutions.Data.Models.CompetencyAssessments;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class SupervisedSelfAssessmentSignoffViewModel
    {
        public SupervisedSelfAssessmentSignoffViewModel()
        { }
        public SupervisedSelfAssessmentSignoffViewModel(CompetencyAssessmentBase competencyAssessmentBase, string? actionName)
        {
            CompetencyAssessmentId = competencyAssessmentBase.ID;
            CompetencyAssessmentName = competencyAssessmentBase.CompetencyAssessmentName;
            this.ActionName = actionName;
            UserRole = competencyAssessmentBase.UserRole;
            PublishStatusID = competencyAssessmentBase.PublishStatusID;
        }
        public SupervisedSelfAssessmentSignoffViewModel(SupervisedSelfAssessmentSignoffViewModel model)
        {
            CompetencyAssessmentId = model.CompetencyAssessmentId;
            CompetencyAssessmentName = model.CompetencyAssessmentName;
            Confirm = model.Confirm;
            Signoff = model.Signoff;
            ActionName = model.ActionName;
            UserRole = model.UserRole;
            PublishStatusID = model.PublishStatusID;
        }
        public int CompetencyAssessmentId { get; set; }
        public string CompetencyAssessmentName { get; set; } = string.Empty;
        public int Signoff { get; set; } = 0;
        public int Confirm { get; set; } = 0;
        public string? ActionName { get; set; }
        public string SignoffText => Signoff == 1 ? "Yes" : "No";
        public string ConfirmText => Confirm == 1 ? "Yes" : "No";
        public int UserRole { get; set; }
        public int PublishStatusID { get; set; }
    }
}
