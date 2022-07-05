namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.CourseCompletion;

    public interface ICourseCompletionService
    {
        CourseCompletion? GetCourseCompletion(int candidateId, int customisationId);
    }

    public class CourseCompletionService : ICourseCompletionService
    {
        private readonly IDbConnection connection;

        public CourseCompletionService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public CourseCompletion? GetCourseCompletion(int candidateId, int customisationId)
        {
            return connection.QueryFirstOrDefault<CourseCompletion?>(
                @"  WITH SectionCount AS (
                  SELECT Customisations.CustomisationID,
                         COUNT(Sections.SectionID) AS SectionCount
                    FROM Customisations
                         INNER JOIN Applications
                         ON Customisations.ApplicationID = Applications.ApplicationID

                         INNER JOIN Sections
                         ON Applications.ApplicationID = Sections.ApplicationID
                   WHERE Customisations.CustomisationID = @customisationId
                         AND Sections.ArchivedDate IS NULL
                   GROUP BY Customisations.CustomisationID
                  ),
                  PostLearningPasses AS (
                  SELECT PostLearningPassStatus.ProgressID,
                         SUM(PostLearningPassStatus.HasPassed) AS PostLearningPasses
                    FROM (
                         SELECT AssessAttempts.ProgressID,
                               MAX(CONVERT(tinyint, Status)) AS HasPassed
                          FROM AssessAttempts
                         WHERE AssessAttempts.CustomisationID = @customisationId
                               AND AssessAttempts.CandidateID = @candidateId
                         GROUP BY AssessAttempts.ProgressID, AssessAttempts.SectionNumber
                    ) AS PostLearningPassStatus
                   GROUP BY PostLearningPassStatus.ProgressID
                  ),
                  PercentageTutorialsCompleted AS (
                  SELECT Progress.ProgressID,
                         CAST(SUM(aspProgress.TutStat) * 100 AS FLOAT) / (COUNT(aspProgress.TutorialID) * 2.0) AS PercentageTutorialsCompleted
                    FROM Customisations
                         INNER JOIN Sections
                         ON Sections.ApplicationID = Customisations.ApplicationID

                         INNER JOIN Tutorials
                         ON Sections.SectionID = Tutorials.SectionID

                         INNER JOIN CustomisationTutorials
                         ON Customisations.CustomisationID = CustomisationTutorials.CustomisationID
                            AND Tutorials.TutorialID = CustomisationTutorials.TutorialID
                            AND CustomisationTutorials.Status = 1

                         INNER JOIN Progress
                         ON Customisations.CustomisationID = Progress.CustomisationID
                            AND Progress.CandidateID = @candidateId

                         INNER JOIN aspProgress
                         ON aspProgress.ProgressID = Progress.ProgressID
                            AND aspProgress.TutorialID = Tutorials.TutorialID

                   WHERE Customisations.CustomisationID = @customisationId
                         AND Sections.ArchivedDate IS NULL
                         AND Tutorials.ArchivedDate IS NULL
                   GROUP BY Progress.ProgressID
                  )

                  SELECT Customisations.CustomisationID AS id,
                         Applications.ApplicationName,
                         Customisations.CustomisationName,
                         Progress.Completed,
                         Progress.Evaluated,
                         Applications.AssessAttempts AS MaxPostLearningAssessmentAttempts,
                         Customisations.IsAssessed,
                         Applications.PLAPassThreshold AS PostLearningAssessmentPassThreshold,
                         Customisations.DiagCompletionThreshold AS DiagnosticAssessmentCompletionThreshold,
                         Customisations.TutCompletionThreshold AS TutorialsCompletionThreshold,
                         Progress.DiagnosticScore,
                         COALESCE(MAX(aspProgress.DiagAttempts), 0) AS DiagnosticAttempts,
                         COALESCE(PercentageTutorialsCompleted.PercentageTutorialsCompleted, 0) AS PercentageTutorialsCompleted,
                         PostLearningPasses.PostLearningPasses,
                         SectionCount.SectionCount
                    FROM Applications
                         INNER JOIN Customisations
                         ON Applications.ApplicationID = Customisations.ApplicationID

                         INNER JOIN SectionCount
                         ON Customisations.CustomisationID = SectionCount.CustomisationID

                         LEFT JOIN Progress
                         ON Customisations.CustomisationID = Progress.CustomisationID
                            AND Progress.CandidateID = @candidateId
                            AND Progress.RemovedDate IS NULL
                            AND Progress.SystemRefreshed = 0

                         LEFT JOIN aspProgress
                         ON Progress.ProgressID = aspProgress.ProgressID

                         LEFT JOIN PercentageTutorialsCompleted
                         ON PercentageTutorialsCompleted.ProgressID = Progress.ProgressID

                         LEFT JOIN PostLearningPasses
                         ON Progress.ProgressID = PostLearningPasses.ProgressID

                   WHERE Customisations.CustomisationID = @customisationId
                         AND Applications.IncludeCertification = 1
                         AND Applications.DefaultContentTypeID <> 4
                   GROUP BY
                         Customisations.CustomisationID,
                         Applications.ApplicationName,
                         Customisations.CustomisationName,
                         Progress.Completed,
                         Progress.Evaluated,
                         Applications.AssessAttempts,
                         Customisations.IsAssessed,
                         Applications.PLAPassThreshold,
                         Customisations.DiagCompletionThreshold,
                         Customisations.TutCompletionThreshold,
                         Progress.DiagnosticScore,
                         PercentageTutorialsCompleted.PercentageTutorialsCompleted,
                         PostLearningPasses.PostLearningPasses,
                         SectionCount.SectionCount;",
                new { candidateId, customisationId });
        }
    }
}
