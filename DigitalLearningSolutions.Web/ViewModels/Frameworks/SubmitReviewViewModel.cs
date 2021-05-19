namespace DigitalLearningSolutions.Web.ViewModels.Frameworks
{
    using DigitalLearningSolutions.Data.Models.Frameworks;
    public class SubmitReviewViewModel
    {
        public int FrameworkId { get; set; }
        public string? FrameworkName { get; set; }
        public FrameworkReview FrameworkReview { get; set; }
    }
}
