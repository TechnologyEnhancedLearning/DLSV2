namespace DigitalLearningSolutions.Web.ViewModels.Feedback
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class FeedbackViewModel
    {
        public FeedbackViewModel() { }

        public FeedbackViewModel(
            int? userId,
            string sourcePageUrl,
            bool? taskAchieved,
            string taskAttempted,
            string feedbackText,
            string? taskRating
            )
        {
            UserId = userId;
            SourcePageUrl = sourcePageUrl;
            TaskAchieved = taskAchieved;
            TaskAttempted = taskAttempted;
            FeedbackText = feedbackText;
            TaskRating = taskRating;
        }

        public int? UserId { get; set; }

        [Required(ErrorMessage = "Please enter SourcePageUrl")]
        public string SourcePageUrl { get; set; }

        public bool? TaskAchieved { get; set; }

        [Required(ErrorMessage = "Please enter TaskAttempted")]
        public string TaskAttempted { get; set; }

        [Required(ErrorMessage = "Please enter FeedbackText")]
        public string FeedbackText { get; set; }

        public string? TaskRating { get; set; }
    }
}
