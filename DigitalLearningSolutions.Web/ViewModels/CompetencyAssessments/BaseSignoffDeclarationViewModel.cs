namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class BaseSignoffDeclarationViewModel
    {
        protected BaseSignoffDeclarationViewModel() { }
        protected BaseSignoffDeclarationViewModel(int id)
        {
            CompetencyAssessmentId = id;
        }

        public int CompetencyAssessmentId { get; set; }
        public string? CompetencyAssessmentName { get; set; }
        public int? DeclarationValue { get; set; }
        public string? ActionName { get; set; }
        public string? CustomText { get; set; }
        public string? DefaultText { get; set; }
        public string? Declaration => DeclarationValue == 1 ? "Custom" : "Default";
        public string? DeclarationText => DeclarationValue == 1 ? CustomText : DefaultText;
        protected void CopyProperties(BaseSignoffDeclarationViewModel model)
        {
            CompetencyAssessmentId = model.CompetencyAssessmentId;
            CompetencyAssessmentName = model.CompetencyAssessmentName;
            CustomText = model.CustomText;
            DefaultText = model.DefaultText.Replace("{{CompetencyAssessmentName}}", model.CompetencyAssessmentName);
            DeclarationValue = model.DeclarationValue;
            ActionName = model.ActionName;
        }
    }
}
