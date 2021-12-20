namespace DigitalLearningSolutions.Web.ViewModels.LearningPortal.RecommendedLearning
{
    using DigitalLearningSolutions.Data.Models.SelfAssessments;

    public class RecommendedLearningViewModel
    {
        public RecommendedLearningViewModel(SelfAssessment selfAssessment)
        {
            SelfAssessment = selfAssessment;
        }

        public SelfAssessment SelfAssessment { get; set; }
    }
}
