namespace DigitalLearningSolutions.Data.Models.UserFeedback
{
    public class UserFeedbackTempData
    {
        public int? UserID { get; set; }
        public string SourceUrl { get; set; }

        public bool? TaskAchieved { get; set; }

        // TODO: This probs belongs in the vm not here
        //[Required(ErrorMessage = "Please enter the task you were attempting to perform.")]
        public string? TaskAttempted { get; set; }

        //[Required(ErrorMessage = "Please enter your feedback text.")]
        public string? FeedbackText { get; set; }

        public int? TaskRating { get; set; }
    }
}

