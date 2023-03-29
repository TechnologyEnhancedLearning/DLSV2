namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Exceptions;
    using Microsoft.Extensions.Logging;

    public interface IUserFeedbackDataService
    {
        public void SaveUserFeedback(
            int? userID,
            string sourceUrl,
            bool? taskAchieved,
            string? taskAttempted,
            string feedbackText,
            int? taskRating
        );
    }

    public class UserFeedbackDataService : IUserFeedbackDataService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<UserDataService.UserDataService> logger;

        public UserFeedbackDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public void SaveUserFeedback(
                int? userID,
                string sourceUrl,
                bool? taskAchieved,
                string? taskAttempted,
                string feedbackText,
                int? taskRating
            )
        {
            var userFeedbackParams = new
            {
                userID = userID,
                sourceUrl = sourceUrl,
                taskAchieved = taskAchieved,
                taskAttempted = taskAttempted,
                feedbackText = feedbackText,
                taskRating = taskRating
            };

            var numberOfAffectedRows = 0;
            
            try
            {
                numberOfAffectedRows = connection.Execute(
                    @"INSERT INTO UserFeedback
                        (UserID, SourcePageUrl, TaskAchieved, TaskAttempted, FeedbackText, TaskRating)
                        VALUES (
                        @userID, @sourceUrl, @taskAchieved, @taskAttempted, @feedbackText, @taskRating)",
                    userFeedbackParams
                );
            }
            catch (Exception e)
            {
                string message = $"User feedback db insert failed ('{e.Message}')";
                logger.LogWarning(message);
                throw new UserFeedbackFailedException(message);
            }
            finally
            {
                if (numberOfAffectedRows == 0)
                {
                    string message = $"User feedback db insert failed.";
                    logger.LogWarning(message);
                    throw new UserFeedbackFailedException(message);
                }

            }
        }
    }
}
