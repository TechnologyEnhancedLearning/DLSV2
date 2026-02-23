using DigitalLearningSolutions.Data.Models.CompetencyAssessments;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class SubmitReviewViewModel
    {
        public SubmitReviewViewModel()
        { }
        public SubmitReviewViewModel(int competencyAssessmentID, string competencyAssessmentName, SelfAssessmentReview review)
        {
            CompetencyAssessmentID = competencyAssessmentID;
            CompetencyAssessmentName = competencyAssessmentName;
            SelfAssessmentReview = review;
        }
        public int CompetencyAssessmentID { get; set; }
        public string? CompetencyAssessmentName { get; set; }
        public SelfAssessmentReview SelfAssessmentReview { get; set; } = new();
    }
}
