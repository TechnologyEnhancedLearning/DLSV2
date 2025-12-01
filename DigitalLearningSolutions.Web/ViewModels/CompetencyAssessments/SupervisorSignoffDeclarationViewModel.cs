namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class SupervisorSignoffDeclarationViewModel: BaseSignoffDeclarationViewModel
    {
        public SupervisorSignoffDeclarationViewModel() : base() { }

        public SupervisorSignoffDeclarationViewModel(int id) : base(id) { }
        public SupervisorSignoffDeclarationViewModel(SupervisorSignoffDeclarationViewModel model)
        {
            CopyProperties(model);
        }
    }
}
