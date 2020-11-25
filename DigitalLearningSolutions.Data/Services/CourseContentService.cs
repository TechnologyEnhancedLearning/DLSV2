namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.CourseContent;
    using Microsoft.Extensions.Logging;

    public interface ICourseContentService
    {
        CourseContent? GetCourseContent(int customisationId);
        int GetProgressId(int candidateId, int customisationId);
        void UpdateLoginCountAndDuration(int progressId);
    }

    public class CourseContentService : ICourseContentService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<CourseContentService> logger;

        public CourseContentService(IDbConnection connection, ILogger<CourseContentService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public CourseContent? GetCourseContent(int customisationId)
        {
            return connection.QueryFirstOrDefault<CourseContent>(
                @"SELECT C.CustomisationID AS Id, C.CustomisationName, A.ApplicationName 
                      FROM Customisations as C
                      JOIN Applications AS A ON C.ApplicationID = A.ApplicationID
                      WHERE C.CustomisationID = @customisationId",
                new { customisationId }
            );
        }

        public int GetProgressId(int candidateId, int customisationId)
        {
            // TODO HEEDLS-202: change QueryFirstOrDefault to creating progress record if not found
            return connection.QueryFirstOrDefault<int>(
                @"SELECT ProgressId
                        FROM Progress
                        WHERE CandidateID = @candidateId
                          AND CustomisationID = @customisationId",
                new { candidateId, customisationId }
            );
        }

        public void UpdateLoginCountAndDuration(int progressId)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE Progress
	                    SET LoginCount = (SELECT COALESCE(COUNT(*), 0)
		                    FROM Sessions AS S
		                    WHERE S.CandidateID = Progress.CandidateID
		                      AND S.CustomisationID = Progress.CustomisationID
		                      AND (S.LoginTime BETWEEN Progress.FirstSubmittedTime AND Progress.SubmittedTime)),
                            Duration = (SELECT COALESCE(SUM(S1.Duration), 0)
		                    FROM Sessions AS S1
		                    WHERE S1.CandidateID = Progress.CandidateID
		                      AND S1.CustomisationID = Progress.CustomisationID
		                      AND (S1.LoginTime BETWEEN Progress.FirstSubmittedTime AND Progress.SubmittedTime))
	                    WHERE Progress.ProgressID = @progressId",
                new { progressId }
            );
            
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating login count and duration as db update failed. " +
                    $"Progress id: {progressId}"
                );
            }
        }
    }
}
