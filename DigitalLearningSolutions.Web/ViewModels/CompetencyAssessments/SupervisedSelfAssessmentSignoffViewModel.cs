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
        public SupervisedSelfAssessmentSignoffViewModel(SupervisedSelfAssessmentSignoffViewModel model )
        {
            CompetencyAssessmentId = model.CompetencyAssessmentId;
            Confirm = model.Confirm;
            Supervised = model.Supervised;
            Signoff = model.Signoff;
            ActionName = model.ActionName;
        }
        public int CompetencyAssessmentId { get; set; }
        public string CompetencyAssessmentName { get; set; } = string.Empty;
        public int? Supervised { get; set; }
        public int? Signoff { get; set; }
        public int? Confirm { get; set; }
        public string? ActionName { get; set; }
        public string SupervisedText => Supervised == 1 ? "Yes" : "No";
        public string SignoffText => Signoff == 1 ? "Yes" : "No";
        public string ConfirmText => Confirm == 1 ? "Yes" : "No";
    }
}
