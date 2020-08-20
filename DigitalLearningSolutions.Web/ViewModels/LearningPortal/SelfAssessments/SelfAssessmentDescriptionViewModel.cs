namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using DigitalLearningSolutions.Data.Models;

    public class SelfAssessmentDescriptionViewModel
    {
        public readonly string Name;
        public readonly string Description;

        public SelfAssessmentDescriptionViewModel(SelfAssessment selfAssessment)
        {
            Name = selfAssessment.Name;
            Description = selfAssessment.Description;
        }
    }
}
