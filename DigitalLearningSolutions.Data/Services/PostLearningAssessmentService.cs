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
        PostLearningContent? GetPostLearningContent(int customisationId, int sectionId);
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
                    WITH NextSectionIdTable AS (
                        SELECT TOP(1)
                            CurrentSection.SectionID,
                            NextSections.SectionID AS NextSectionID
                        FROM Sections AS CurrentSection
                                LEFT JOIN CustomisationTutorials AS NextCustomisationTutorials
                                    ON NextCustomisationTutorials.CustomisationID = @customisationId
                                LEFT JOIN Customisations
                                    ON NextCustomisationTutorials.CustomisationID = Customisations.CustomisationID
                                LEFT JOIN Tutorials AS NextSectionsTutorials
                                    ON NextCustomisationTutorials.TutorialID = NextSectionsTutorials.TutorialID
                                    AND NextSectionsTutorials.ArchivedDate IS NULL
                                LEFT JOIN Sections AS NextSections
                                    ON NextSectionsTutorials.SectionID = NextSections.SectionID
                                    AND CurrentSection.SectionNumber <= NextSections.SectionNumber
                                    AND (CurrentSection.SectionNumber < NextSections.SectionNumber
                                        OR CurrentSection.SectionID < NextSections.SectionID)
                        WHERE CurrentSection.SectionId = @sectionId
                            AND NextSections.SectionID IS NOT NULL
                            AND NextSections.SectionNumber IS NOT NULL
                            AND Customisations.Active = 1
                            AND NextSections.ArchivedDate IS NULL
                            AND (
                                 NextCustomisationTutorials.Status = 1
                                 OR NextCustomisationTutorials.DiagStatus = 1
                                 OR (Customisations.IsAssessed = 1 AND NextSections.PLAssessPath IS NOT NULL)
                            )
                        GROUP BY
                            CurrentSection.SectionID,
                            NextSections.SectionID,
                            NextSections.SectionNumber
                        ORDER BY NextSections.SectionNumber, NextSections.SectionID
                    )
                    SELECT
                        Applications.ApplicationName,
                        Customisations.CustomisationName,
                        Sections.SectionName,
                        COALESCE (Attempts.BestScore, 0) AS BestScore,
                        COALESCE (Attempts.AttemptsPL, 0) AS AttemptsPL,
                        COALESCE (Attempts.PLPasses, 0) AS PLPasses,
                        CAST (COALESCE (Progress.PLLocked, 0) AS bit) AS PLLocked,
                        NextSectionIdTable.NextSectionId
                    FROM Sections
                        INNER JOIN Customisations
                            ON Customisations.ApplicationID = Sections.ApplicationID
                        INNER JOIN Applications
                            ON Applications.ApplicationID = Sections.ApplicationID
                        LEFT JOIN NextSectionIdTable
                            ON Sections.SectionID = NextSectionIdTable.SectionID
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

        public PostLearningContent? GetPostLearningContent(int customisationId, int sectionId)
        {
            PostLearningContent? postLearningContent = null;
            return connection.Query<PostLearningContent, int, PostLearningContent>(
                @"
                    SELECT
                        Applications.ApplicationName,
                        Customisations.CustomisationName,
                        Sections.SectionName,
                        Sections.PLAssessPath,
                        Applications.PLAPassThreshold,
                        Customisations.CurrentVersion,
                        CASE WHEN Tutorials.OriginalTutorialID > 0
                            THEN Tutorials.OriginalTutorialID
                            ELSE Tutorials.TutorialID
                        END AS id
                    FROM Sections
                        INNER JOIN Customisations
                            ON Customisations.ApplicationID = Sections.ApplicationID
                            AND Customisations.Active = 1
                            AND Customisations.IsAssessed = 1
                        INNER JOIN Applications
                            ON Applications.ApplicationID = Sections.ApplicationID
                        INNER JOIN CustomisationTutorials
                            ON CustomisationTutorials.CustomisationID = Customisations.CustomisationID
                        INNER JOIN Tutorials
                            ON Tutorials.TutorialID = CustomisationTutorials.TutorialID
                            AND Tutorials.SectionID = Sections.SectionID
                            AND Tutorials.ArchivedDate IS NULL
                    WHERE
                        Customisations.CustomisationID = @customisationId
                        AND Sections.SectionID = @sectionId
                        AND Sections.ArchivedDate IS NULL
                        AND Sections.PLAssessPath IS NOT NULL
                    ORDER BY
                        Tutorials.OrderByNumber,
                        Tutorials.TutorialID",
                (postLearning, tutorialId) =>
                {
                    postLearningContent ??= postLearning;

                    postLearningContent.Tutorials.Add(tutorialId);

                    return postLearningContent;
                },
                new { customisationId, sectionId },
                splitOn: "id"
            ).FirstOrDefault();
        }
    }
}
