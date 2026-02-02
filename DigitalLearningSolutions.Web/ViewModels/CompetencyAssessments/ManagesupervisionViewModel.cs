namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
     public class ManagesupervisionViewModel
        {
            public ManagesupervisionViewModel() { }
            public ManagesupervisionViewModel(SupervisedSelfAssessmentSignoffViewModel signoff)
            {
                Signoff = signoff;
                CompetencyAssessmentName = signoff.CompetencyAssessmentName;
            }
            public ManagesupervisionViewModel(SupervisorSignoffDeclarationViewModel supervisorDeclaration,
                SupervisedSelfAssessmentSignoffViewModel signoff)
            {
                SupervisorDeclaration = supervisorDeclaration;
                Signoff = signoff;
            }
            public ManagesupervisionViewModel(LearnerSignoffDeclarationViewModel learnerDeclaration,
                SupervisorSignoffDeclarationViewModel supervisorDeclaration,
                SupervisedSelfAssessmentSignoffViewModel signoff)
            {
                LearnerDeclaration = learnerDeclaration;
                SupervisorDeclaration = supervisorDeclaration;
                Signoff = signoff;
            }
            public ManagesupervisionViewModel(int competencyAssessmentId,
                ManagesupervisionViewModel model,
                string learnerDefaultText,
                 string supervisorDefaultText)
            {
                CompetencyAssessmentId = competencyAssessmentId;
                CompetencyAssessmentName = model.CompetencyAssessmentName;
                Signoff = model.Signoff;
                SupervisorDeclaration = model.SupervisorDeclaration;
                SupervisorDeclaration.DefaultText = supervisorDefaultText.Replace("{{CompetencyAssessmentName}}", CompetencyAssessmentName);
                LearnerDeclaration = model.LearnerDeclaration;
                LearnerDeclaration.DefaultText = learnerDefaultText.Replace("{{CompetencyAssessmentName}}", CompetencyAssessmentName);
            }
            public ManagesupervisionViewModel(int competencyAssessmentId, string competencyAssessmentName,
                bool supervisorResultsReview,
                bool SupervisorSelfAssessmentReview,
                string? signOffSupervisorStatement,
                string? signOffRequestorStatement,
                string learnerDefaultText,
                 string supervisorDefaultText)
            {
                CompetencyAssessmentId = competencyAssessmentId;
                CompetencyAssessmentName = competencyAssessmentName;
                Signoff.CompetencyAssessmentId = competencyAssessmentId;
                Signoff.Supervised = supervisorResultsReview == true ? 1 : 0;
                Signoff.Signoff = supervisorResultsReview == true ? 1 : 0;
                Signoff.Confirm = SupervisorSelfAssessmentReview == true ? 1 : 0;
                SupervisorDeclaration.CompetencyAssessmentName = competencyAssessmentName;
                SupervisorDeclaration.CompetencyAssessmentId = competencyAssessmentId;
                SupervisorDeclaration.DeclarationValue = signOffSupervisorStatement == null ? 0 : 1;
                SupervisorDeclaration.CustomText = signOffSupervisorStatement;
                SupervisorDeclaration.DefaultText = supervisorDefaultText.Replace("{{CompetencyAssessmentName}}", CompetencyAssessmentName);
                LearnerDeclaration.DeclarationValue = signOffRequestorStatement == null ? 0 : 1;
                LearnerDeclaration.CustomText = signOffRequestorStatement;
                LearnerDeclaration.DefaultText = learnerDefaultText.Replace("{{CompetencyAssessmentName}}", CompetencyAssessmentName);
                LearnerDeclaration.CompetencyAssessmentName = competencyAssessmentName;
                LearnerDeclaration.CompetencyAssessmentId = competencyAssessmentId;
            }
            public SupervisedSelfAssessmentSignoffViewModel Signoff { get; set; } = new SupervisedSelfAssessmentSignoffViewModel();
            public SupervisorSignoffDeclarationViewModel SupervisorDeclaration { get; set; } = new SupervisorSignoffDeclarationViewModel();
            public LearnerSignoffDeclarationViewModel LearnerDeclaration { get; set; } = new LearnerSignoffDeclarationViewModel();
            public bool? TaskCompleteChecked { get; set; }
            public int CompetencyAssessmentId { get; set; }
            public string CompetencyAssessmentName { get; set; } = string.Empty;
       
    }
}
