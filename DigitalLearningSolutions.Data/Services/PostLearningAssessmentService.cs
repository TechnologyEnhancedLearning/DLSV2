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
            // NextSectionID is the ID of the next section in the course, according to SectionNumber
            // or null if the last in the course.

            // Using this list of other tutorials in the course we can work out if there is another item in the
            // section (if there is an viewable tutorial, or a post learning assessment, or consolidation material),
            // and if there are other sections (valid tutorials with a different tutorial ID, or with assessments or
            // consolidation material. See the SectionContentDataService for the definition of a valid section.

            return connection.QueryFirstOrDefault<PostLearningAssessment>(
                @"  WITH CourseTutorials AS (
                    SELECT Tutorials.TutorialID,
                           Tutorials.OrderByNumber,
                           CustomisationTutorials.Status,
                           Sections.SectionID,
                           Sections.SectionNumber,
                           CAST (CASE WHEN CustomisationTutorials.DiagStatus = 1 AND Sections.DiagAssessPath IS NOT NULL
                                           THEN 1
                                      ELSE 0
                                 END AS BIT) AS HasDiagnostic
                      FROM Tutorials
                           INNER JOIN Customisations
                               ON Customisations.CustomisationID = @customisationId

                           INNER JOIN Sections
                               ON Tutorials.SectionID = Sections.SectionID
                               AND Sections.ArchivedDate IS NULL

                           INNER JOIN CustomisationTutorials
                               ON CustomisationTutorials.CustomisationID = @customisationId
                               AND CustomisationTutorials.TutorialID = Tutorials.TutorialID
                               AND Tutorials.ArchivedDate IS NULL
                               AND (
                                    CustomisationTutorials.Status = 1
                                    OR (CustomisationTutorials.DiagStatus = 1 AND Sections.DiagAssessPath IS NOT NULL AND Tutorials.DiagAssessOutOf > 0)
                                    OR (Customisations.IsAssessed = 1 AND Sections.PLAssessPath IS NOT NULL)
                               )
                    ),
                    NextSection AS (
                    SELECT TOP 1
                           CurrentSection.SectionID AS CurrentSectionID,
                           CourseTutorials.SectionID AS NextSectionID
                      FROM Sections AS CurrentSection
                           INNER JOIN CourseTutorials
                               ON CourseTutorials.SectionID <> CurrentSection.SectionID

                     WHERE CurrentSection.SectionID = @sectionId
                           AND CurrentSection.SectionNumber <= CourseTutorials.SectionNumber
                           AND (
                                CurrentSection.SectionNumber < CourseTutorials.SectionNumber
                                OR CurrentSection.SectionID < CourseTutorials.SectionID
                           )

                     ORDER BY CourseTutorials.SectionNumber, CourseTutorials.SectionID
                    )
                    SELECT
                        Applications.ApplicationName,
                        Applications.ApplicationInfo,
                        Customisations.CustomisationName,
                        Sections.SectionName,
                        COALESCE (Attempts.BestScore, 0) AS BestScore,
                        COALESCE (Attempts.AttemptsPL, 0) AS AttemptsPL,
                        COALESCE (Attempts.PLPasses, 0) AS PLPasses,
                        CAST (COALESCE (Progress.PLLocked, 0) AS bit) AS PLLocked,
                        Applications.IncludeCertification,
                        Customisations.IsAssessed,
                        Progress.Completed,
                        Applications.AssessAttempts AS MaxPostLearningAssessmentAttempts,
                        Applications.PLAPassThreshold AS PostLearningAssessmentPassThreshold,
                        Customisations.DiagCompletionThreshold AS DiagnosticAssessmentCompletionThreshold,
                        Customisations.TutCompletionThreshold AS TutorialsCompletionThreshold,
                        NextSection.NextSectionID,
                        CAST (CASE WHEN EXISTS(SELECT 1
                                                 FROM CourseTutorials
                                                WHERE SectionID <> @sectionId)
                                        THEN 1
                                   ELSE 0
                              END AS BIT) AS OtherSectionsExist,
                        CAST (CASE WHEN Sections.ConsolidationPath IS NOT NULL
                                        THEN 1
                                   WHEN EXISTS(SELECT 1
                                                 FROM CourseTutorials
                                                WHERE SectionID = @sectionId
                                                      AND (Status = 1 OR HasDiagnostic = 1)
                                              )
                                        THEN 1
                                   ELSE 0
                              END AS BIT) AS OtherItemsInSectionExist,
                        Customisations.Password,
                        Progress.PasswordSubmitted
                    FROM Sections
                        INNER JOIN Customisations
                            ON Customisations.ApplicationID = Sections.ApplicationID
                        INNER JOIN Applications
                            ON Applications.ApplicationID = Sections.ApplicationID
                        LEFT JOIN NextSection
                            ON Sections.SectionID = NextSection.CurrentSectionID
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
                        AND Sections.PLAssessPath IS NOT NULL
                        AND Applications.DefaultContentTypeID <> 4;",
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
                        AND Applications.DefaultContentTypeID <> 4
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
