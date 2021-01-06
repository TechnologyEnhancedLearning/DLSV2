namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.PostLearningAssessment;
    using Microsoft.Extensions.Logging;

    public interface IPostLearningAssessmentService
    {
        PostLearningAssessment? GetPostLearningAssessment(int customisationId, int candidateId, int sectionId);
    }

    public class PostLearningAssessmentService : IPostLearningAssessmentService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<PostLearningAssessmentService> logger;

        public PostLearningAssessmentService(IDbConnection connection, ILogger<PostLearningAssessmentService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public PostLearningAssessment? GetPostLearningAssessment(int customisationId, int candidateId, int sectionId)
        {
            return connection.QueryFirstOrDefault<PostLearningAssessment>(
                @"
                    SELECT
                        Applications.ApplicationName,
                        Customisations.CustomisationName,
                        Sections.SectionName,
                        Sections.PLAssessPath,
                        COALESCE (Attempts.BestScore, 0) AS BestScore,
                        COALESCE (Attempts.AttemptsPL, 0) AS AttemptsPL,
                        COALESCE (Attempts.PLPasses, 0) AS PLPasses,
                        CAST (COALESCE (Progress.PLLocked, 0) AS bit) AS PLLocked
                    FROM Sections
                        INNER JOIN Customisations
                            ON Customisations.ApplicationID = Sections.ApplicationID
                        INNER JOIN Applications
                            ON Applications.ApplicationID = Sections.ApplicationID
                        LEFT JOIN Progress
                            ON Progress.CustomisationID = Customisations.CustomisationID
                            AND Progress.CandidateID = @candidateId
                            AND Progress.RemovedDate IS NULL
                            AND Progress.SystemRefreshed = 0
                        LEFT JOIN (
                            SELECT
                                COUNT(AssessAttemptID) AS AttemptsPL,
                                AssessAttempts.ProgressID,
                                AssessAttempts.SectionNumber,
                                MAX(AssessAttempts.Score) AS BestScore,
                                SUM(CAST(AssessAttempts.Status AS Integer)) AS PLPasses
                            FROM AssessAttempts
                                GROUP BY
                                    AssessAttempts.ProgressID,
                                    AssessAttempts.SectionNumber
                            ) AS Attempts ON (Progress.ProgressID = Attempts.ProgressID) AND (Attempts.SectionNumber = Sections.SectionNumber)
                    WHERE
                        Customisations.CustomisationID = @customisationId
                        AND Customisations.IsAssessed = 1
                        AND Sections.SectionID = @sectionId
                        AND Sections.ArchivedDate IS NULL
                        AND Sections.PLAssessPath IS NOT NULL",
                new { customisationId, candidateId, sectionId }
            );
        }
    }
}
