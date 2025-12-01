namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class LearnerSignoffDeclarationViewModel: BaseSignoffDeclarationViewModel
    {
        public LearnerSignoffDeclarationViewModel() : base() { }
        public LearnerSignoffDeclarationViewModel(int id) : base(id) { }
        public LearnerSignoffDeclarationViewModel(LearnerSignoffDeclarationViewModel model)
        {
            CopyProperties(model);
        }
    }
}
