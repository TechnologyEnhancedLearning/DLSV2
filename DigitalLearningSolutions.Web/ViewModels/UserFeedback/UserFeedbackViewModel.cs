namespace DigitalLearningSolutions.Web.ViewModels.UserFeedback
{
    public class UserFeedbackViewModel
    {
        public UserFeedbackViewModel() { }

        public UserFeedbackViewModel(
            int? userId,
            string? sourceUrl,
            bool? taskAchieved,
            string? taskAttempted,
            string? feedbackText,
            int? taskRating
            )
        {
            UserId = userId;
            SourceUrl = sourceUrl;
            TaskAchieved = taskAchieved;
            TaskAttempted = taskAttempted;
            FeedbackText = feedbackText;
            TaskRating = taskRating;
        }

        public int? UserId { get; set; }

        public string? SourceUrl { get; set; }

        public bool? TaskAchieved { get; set; }

        //[Required(ErrorMessage = "Please enter the task you were attempting to perform.")]
        public string? TaskAttempted { get; set; }

        //[Required(ErrorMessage = "Please enter your feedback text.")]
        public string? FeedbackText { get; set; }

        public int? TaskRating { get; set; }
    }
}
