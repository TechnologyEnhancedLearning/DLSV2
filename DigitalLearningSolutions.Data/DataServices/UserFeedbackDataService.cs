namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Data;
    using Dapper;

    public interface IUserFeedbackDataService
    {
        public void SaveUserFeedback(
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
                userID = userID,
                sourcePageUrl = sourcePageUrl,
                taskAchieved = taskAchieved,
                taskAttempted = taskAttempted,
                feedbackText = feedbackText,
                taskRating = taskRating
            };

            connection.Execute(
               @"INSERT INTO UserFeedback
                        (UserID, SourcePageUrl, TaskAchieved, TaskAttempted, FeedbackText, TaskRating)
                        VALUES (
                        @userID, @sourcePageUrl, @taskAchieved, @taskAttempted, @feedbackText, @taskRating)",
               userFeedbackParams
           );
        }
    }
}
