namespace DigitalLearningSolutions.Data.Models.UserFeedback
{
    public class UserFeedbackSessionData
    {
        public int? UserID { get; set; }
        public string SourceUrl { get; set; }


        
        public bool? TaskAchieved { get; set; }

        //[Required(ErrorMessage = "Please enter the task you were attempting to perform.")]
        public string? TaskAttempted { get; set; }

        //[Required(ErrorMessage = "Please enter your feedback text.")]
        public string? FeedbackText { get; set; }

        public string? TaskDifficulty { get; set; }
    }
}
}
