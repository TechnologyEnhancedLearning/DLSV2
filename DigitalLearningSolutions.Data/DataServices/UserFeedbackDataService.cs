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
            int? userId,
            string? userRoles,
            string? sourceUrl,
            bool? taskAchieved,
            string? taskAttempted,
            string? feedbackText,
            int? taskRating
        );
    }

    public class UserFeedbackDataService : IUserFeedbackDataService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<UserDataService.UserDataService> logger;

        public UserFeedbackDataService(IDbConnection connection, ILogger<UserDataService.UserDataService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public void SaveUserFeedback(
                int? userId,
                string userRoles,
                string? sourceUrl,
                bool? taskAchieved,
                string? taskAttempted,
                string feedbackText,
                int? taskRating
            )
        {
            var userFeedbackParams = new
            {
                userId,
                userRoles,
                sourceUrl,
                taskAchieved,
                taskAttempted,
                feedbackText,
                taskRating
            };

            var numberOfAffectedRows = 0;
            
            try
            {
                numberOfAffectedRows = connection.Execute(
                    @"INSERT INTO UserFeedback
                        (UserID, SourcePageUrl, TaskAchieved, TaskAttempted, FeedbackText, TaskRating, UserRoles)
                        VALUES (
                        @userId, @sourceUrl, @taskAchieved, @taskAttempted, @feedbackText, @taskRating, @userRoles)",
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
