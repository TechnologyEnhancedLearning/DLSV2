namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.RecommendedLearning
{
    using DigitalLearningSolutions.Data.Models.SelfAssessments;

    public class ResourceRemovedViewModel
    {
        public ResourceRemovedViewModel(SelfAssessment selfAssessment)
        {
            SelfAssessment = selfAssessment;
        }

        public SelfAssessment SelfAssessment { get; set; }
    }
}
