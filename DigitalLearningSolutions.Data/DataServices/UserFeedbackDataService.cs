namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Email;

    public interface IUserFeedbackDataService
    {
        public void SaveUserFeedback(
            DateTime submitted,
            int? userID,
            string sourcePageUrl,
            bool? taskAchieved,
            string? taskAttempted,
            string feedbackText,
            int? taskRating
        );
    }

    public class UserFeedbackDataService : IUserFeedbackDataService
    {
        private readonly IDbConnection connection;

        public UserFeedbackDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public void SaveUserFeedback(
                DateTime submitted,
                int? userID,
                string sourcePageUrl,
                bool? taskAchieved,
                string? taskAttempted,
                string feedbackText,
                int? taskRating
            )
        {
            var userFeedbackParams = new
            {
                submitted = submitted,
                userID = userID,
                sourcePageUrl = sourcePageUrl,
                taskAchieved = taskAchieved,
                taskAttempted = taskAttempted,
                feedbackText = feedbackText,
                taskRating = taskRating
            };

            connection.Execute(
                @"INSERT INTO UserFeedback
                        (Submitted, UserID, SourcePageUrl, TaskAchieved, TaskAttempted, FeedbackText, TaskRating)
                        VALUES (
                        @submitted, @userID, @sourcePageUrl, @taskAchieved, @taskAttempted, @feedbackText, @taskRating)",
                userFeedbackParams
            );
        }
    }
}
