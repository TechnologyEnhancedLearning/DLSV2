namespace DigitalLearningSolutions.Web.ViewModels.Feedback
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class FeedbackViewModel
    {
        public FeedbackViewModel() { }

        public FeedbackViewModel(
            int? userId,
            string sourceUrl,
            bool taskAchieved,
            string taskAttempted,
            string feedbackText,
            string? taskRating
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

        public string SourceUrl { get; set; }

        public bool TaskAchieved { get; set; }

        [Required(ErrorMessage = "Please enter the task you were attempting to perform.")]
        public string TaskAttempted { get; set; }

        [Required(ErrorMessage = "Please enter your feedback text.")]
        public string FeedbackText { get; set; }

        public string? TaskRating { get; set; }
    }
}
