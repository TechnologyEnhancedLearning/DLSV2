namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.SelfAssessments
{
    using DigitalLearningSolutions.Data.Models;

    public class SelfAssessmentDescriptionViewModel
    {
        public readonly int Id;
        public readonly string Name;
        public readonly string Description;

        public SelfAssessmentDescriptionViewModel(SelfAssessment selfAssessment)
        {
            Id = selfAssessment.Id;
            Name = selfAssessment.Name;
            Description = selfAssessment.Description;
        }
    }
}
