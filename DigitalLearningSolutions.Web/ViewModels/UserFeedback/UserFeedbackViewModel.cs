namespace DigitalLearningSolutions.Web.ViewModels.UserFeedback
{
    public class UserFeedbackViewModel
    {
        public UserFeedbackViewModel()
        {
            UserId = null;
            UserRoles = null;
            SourceUrl = null;
            SourcePageTitle = null;
            TaskAchieved = null;
            TaskAttempted = null;
            FeedbackText = null;
            TaskRating = null;
        }

        public int? UserId { get; set; }
        public string? UserRoles { get; set; }
        public string? SourceUrl { get; set; }
        public string? SourcePageTitle { get; set; }
        public bool? TaskAchieved { get; set; }
        public string? TaskAttempted { get; set; }
        public string? FeedbackText { get; set; }
        public int? TaskRating { get; set; }
    }
}
