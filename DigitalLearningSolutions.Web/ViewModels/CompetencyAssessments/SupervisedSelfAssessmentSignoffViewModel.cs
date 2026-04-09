namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class SupervisedSelfAssessmentSignoffViewModel
    {
        public SupervisedSelfAssessmentSignoffViewModel()
        { }
        public SupervisedSelfAssessmentSignoffViewModel(int id, string competencyAssessmentName, string? actionName)
        {
            CompetencyAssessmentId = id;
            CompetencyAssessmentName = competencyAssessmentName;
            this.ActionName = actionName;
        }
        public SupervisedSelfAssessmentSignoffViewModel(SupervisedSelfAssessmentSignoffViewModel model)
        {
            CompetencyAssessmentId = model.CompetencyAssessmentId;
            CompetencyAssessmentName = model.CompetencyAssessmentName;
            Confirm = model.Confirm;
            Signoff = model.Signoff;
            ActionName = model.ActionName;
        }
        public int CompetencyAssessmentId { get; set; }
        public string CompetencyAssessmentName { get; set; } = string.Empty;
        public int Signoff { get; set; } = 0;
        public int Confirm { get; set; } = 0;
        public string? ActionName { get; set; }
        public string SignoffText => Signoff == 1 ? "Yes" : "No";
        public string ConfirmText => Confirm == 1 ? "Yes" : "No";
    }
}
