using DigitalLearningSolutions.Web.Attributes;

namespace DigitalLearningSolutions.Web.ViewModels.CompetencyAssessments
{
    public class PublishWithoutReviewViewModel
    {
        public PublishWithoutReviewViewModel()
        { }
        public PublishWithoutReviewViewModel(int competencyAssessmentID, string competencyAssessmentName)
        {
            CompetencyAssessmentID = competencyAssessmentID;
            CompetencyAssessmentName = competencyAssessmentName;
        }
        public int CompetencyAssessmentID { get; set; }
        public string CompetencyAssessmentName { get; set; } = string.Empty;
        public int AdminID { get; set; }
        [BooleanMustBeTrue(ErrorMessage = "You must confirm that you wish to publish without review")]
        public bool Confirm { get; set; }
    }
}
