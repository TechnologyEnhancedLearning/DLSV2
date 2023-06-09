﻿namespace DigitalLearningSolutions.Data.Models.UserFeedback
{
    using System.ComponentModel.DataAnnotations;

    public class UserFeedbackTempData
    {
        public UserFeedbackTempData()
        {
            UserId = null;
            UserRoles = string.Empty;
            SourceUrl = string.Empty;
            SourcePageTitle = string.Empty;
            TaskAchieved = null;
            TaskAttempted = "TaskAttempted";
            FeedbackText = "FeedbackText";
            TaskRating = null;
        }
        
        public int? UserId { get; set; }
        public string? UserRoles { get; set; }
        public string? SourceUrl { get; set; }
        public string? SourcePageTitle { get; set; }
        public bool? TaskAchieved { get; set; }

        [Required(ErrorMessage = "[UserFeedbackTempData] Please enter the task you were attempting to perform.")]
        public string TaskAttempted { get; set; }

        [Required(ErrorMessage = "[UserFeedbackTempData] Please enter your feedback text.")]
        public string FeedbackText { get; set; }

        public int? TaskRating { get; set; }
    }
}
