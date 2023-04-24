namespace DigitalLearningSolutions.Web.ViewModels.UserFeedback
{
    using System.ComponentModel.DataAnnotations;

    public class UserFeedbackViewModel
    {
        public int? UserId { get; set; }

        public string? SourceUrl { get; set; }

        public bool? TaskAchieved { get; set; }

        //[Required(ErrorMessage = "Please enter the task you were attempting to perform.")]
        public string? TaskAttempted { get; set; }

        [Required(ErrorMessage = "Please enter your feedback text.")]
        public string? FeedbackText { get; set; }

        public int? TaskRating { get; set; }
    }
}
