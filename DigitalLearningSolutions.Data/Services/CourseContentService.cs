namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.CourseContent;
    using Microsoft.Extensions.Logging;

    public interface ICourseContentService
    {
        CourseContent? GetCourseContent(int candidateId, int customisationId);

        int? GetOrCreateProgressId(int candidateId, int customisationId, int centreId);

        void UpdateProgress(int progressId);

        string? GetCoursePassword(int customisationId);

        void LogPasswordSubmitted(int progressId);
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

        public CourseContent? GetCourseContent(int candidateId, int customisationId)
        {
            // Get course content, section names and section progress for a candidate
            // When the candidate is not doing this course, (ie there isn't an entry in the progress table for the
            // given customisation and candidate IDs) we still get the general course information, just with the
            // percentage completion set to 0.
            // This is achieved using LEFT JOINs on Progress, so we get the candidates progress details or some nulls.

            // This query starts by getting a record per tutorial (with progress for that tutorial from aspProgress)
            // and then aggregates them into a list of sections with tutorial percentage completion of each section.

            // The CustomisationDurations finds the sum of all valid tutorials in the entire course, aggregating over
            // all sections and tutorials (unlike aggregating over just the tutorials in a section for the main query).

            CourseContent? courseContent = null;
            return connection.Query<CourseContent, CourseSection, CourseContent>(
                @" WITH CustomisationDurations AS (
                  SELECT CustomisationId,
                         SUM(AverageDuration) AS AverageDuration
                    FROM (SELECT Customisations.CustomisationID,
                                 CASE
                                 WHEN Tutorials.OverrideTutorialMins > 0 THEN Tutorials.OverrideTutorialMins
                                 ELSE Tutorials.AverageTutMins END AS AverageDuration
                            FROM CustomisationTutorials
                            INNER JOIN Customisations ON CustomisationTutorials.CustomisationID = Customisations.CustomisationID
                            INNER JOIN Tutorials ON CustomisationTutorials.TutorialID = Tutorials.TutorialID
                            WHERE CustomisationTutorials.CustomisationID = @customisationId
                                  AND CustomisationTutorials.Status = 1
                                  AND Tutorials.ArchivedDate IS NULL
                       ) AS TutorialDurations
                   GROUP BY CustomisationID
                  )
                  SELECT Customisations.CustomisationID AS id,
                         Applications.ApplicationName,
                         Applications.ApplicationInfo,
                         Customisations.CustomisationName,
                         CustomisationDurations.AverageDuration,
                         Centres.CentreName,
                         Centres.BannerText,
                         Applications.IncludeCertification,
                         Progress.Completed,
                         Applications.AssessAttempts AS MaxPostLearningAssessmentAttempts,
                         Customisations.IsAssessed,
                         Applications.PLAPassThreshold AS PostLearningAssessmentPassThreshold,
                         Customisations.DiagCompletionThreshold AS DiagnosticAssessmentCompletionThreshold,
                         Customisations.TutCompletionThreshold AS TutorialsCompletionThreshold,
                         Applications.CourseSettings,
                         Customisations.Password,
                         Progress.PasswordSubmitted,
                         Sections.SectionName,
                         Sections.SectionID AS id,
                         dbo.CheckCustomisationSectionHasLearning(Customisations.CustomisationID, Sections.SectionID) AS HasLearning,
                         (CASE
                            WHEN Progress.CandidateID IS NULL
                                 OR dbo.CheckCustomisationSectionHasLearning(Customisations.CustomisationID, Sections.SectionID) = 0
                            THEN 0
                            ELSE CAST(SUM(aspProgress.TutStat) * 100 AS FLOAT) / (COUNT(Tutorials.TutorialID) * 2)
                         END) AS PercentComplete,
                         COALESCE (Attempts.PLPasses, 0) AS PLPasses
                    FROM Applications
                   INNER JOIN Customisations ON Applications.ApplicationID = Customisations.ApplicationID
                   INNER JOIN Sections ON Sections.ApplicationID = Applications.ApplicationID
                   INNER JOIN Centres ON Customisations.CentreID = Centres.CentreID
                   INNER JOIN Tutorials ON Sections.SectionID = Tutorials.SectionID
                   INNER JOIN CustomisationTutorials ON Customisations.CustomisationID = CustomisationTutorials.CustomisationID
                                                   AND Tutorials.TutorialID = CustomisationTutorials.TutorialID
                    LEFT JOIN CustomisationDurations ON CustomisationDurations.CustomisationID = Customisations.CustomisationID
                    LEFT JOIN Progress ON Customisations.CustomisationID = Progress.CustomisationID AND Progress.CandidateID = @candidateId AND Progress.RemovedDate IS NULL AND Progress.SystemRefreshed = 0
                    LEFT JOIN aspProgress ON aspProgress.ProgressID = Progress.ProgressID AND aspProgress.TutorialID = Tutorials.TutorialID
                    LEFT JOIN (SELECT AssessAttempts.ProgressID,
                            AssessAttempts.SectionNumber,
                            SUM(CAST(AssessAttempts.Status AS Integer)) AS PLPasses
                            FROM AssessAttempts
                            GROUP BY
                                AssessAttempts.ProgressID,
                                AssessAttempts.SectionNumber
                        ) AS Attempts ON (Progress.ProgressID = Attempts.ProgressID) AND (Attempts.SectionNumber = Sections.SectionNumber)
                   WHERE Customisations.CustomisationID = @customisationId
                     AND Customisations.Active = 1
                     AND Sections.ArchivedDate IS NULL
                     AND Tutorials.ArchivedDate IS NULL
                     AND Applications.ArchivedDate IS NULL
                     AND (CustomisationTutorials.Status = 1 OR CustomisationTutorials.DiagStatus = 1 OR Customisations.IsAssessed = 1)
                     AND Applications.DefaultContentTypeID <> 4
                   GROUP BY
                         Sections.SectionID,
                         Customisations.CustomisationID,
                         Applications.ApplicationName,
                         Applications.ApplicationInfo,
                         Customisations.CustomisationName,
                         Customisations.Password,
                         Progress.PasswordSubmitted,
                         CustomisationDurations.AverageDuration,
                         Centres.CentreName,
                         Centres.BannerText,
                         Applications.IncludeCertification,
                         Progress.Completed,
                         Applications.AssessAttempts,
                         Attempts.PLPasses,
                         Customisations.IsAssessed,
                         Applications.PLAPassThreshold,
                         Customisations.DiagCompletionThreshold,
                         Customisations.TutCompletionThreshold,
                         Applications.CourseSettings,
                         Sections.SectionName,
                         Sections.SectionID,
                         Sections.SectionNumber,
                         Progress.CandidateID
                   ORDER BY Sections.SectionNumber, Sections.SectionID;",
                (course, section) =>
                {
                    courseContent ??= course;

                    courseContent.Sections.Add(section);
                    return courseContent;
                },
                new { candidateId, customisationId },
                splitOn: "SectionName"
            ).FirstOrDefault();
        }

        public string? GetCoursePassword(int customisationId)
        {
            return connection.QueryFirstOrDefault<string?>(
                @" SELECT Password FROM Customisations AS c
                    INNER JOIN Applications AS ap ON ap.ApplicationID = c.ApplicationID
                    WHERE CustomisationID = @customisationId AND ap.DefaultContentTypeID <> 4",
                new { customisationId }
            );
        }

        public int? GetOrCreateProgressId(int candidateId, int customisationId, int centreId)
        {
            var progressId = GetProgressId(candidateId, customisationId);

            if (progressId != null)
            {
                return progressId;
            }

            var errorCode = connection.QueryFirst<int>(
                @"uspCreateProgressRecord_V3",
                new
                {
                    candidateId,
                    customisationId,
                    centreId,
                    EnrollmentMethodID = 1,
                    EnrolledByAdminID = 0,
                },
                commandType: CommandType.StoredProcedure
            );

            switch (errorCode)
            {
                case 0:
                    return GetProgressId(candidateId, customisationId);
                case 1:
                    logger.LogError(
                        "Not enrolled candidate on course as progress already exists. " +
                        $"Candidate id: {candidateId}, customisation id: {customisationId}, centre id: {centreId}"
                    );
                    break;
                case 100:
                    logger.LogError(
                        "Not enrolled candidate on course as customisation id doesn't match centre id. " +
                        $"Candidate id: {candidateId}, customisation id: {customisationId}, centre id: {centreId}"
                    );
                    break;
                case 101:
                    logger.LogError(
                        "Not enrolled candidate on course as candidate id doesn't match centre id. " +
                        $"Candidate id: {candidateId}, customisation id: {customisationId}, centre id: {centreId}"
                    );
                    break;
                default:
                    logger.LogError(
                        "Not enrolled candidate on course as stored procedure failed. " +
                        $"Unknown error code: {errorCode}, candidate id: {candidateId}, customisation id: {customisationId}, centre id: {centreId}"
                    );
                    break;
            }

            return null;
        }

        public void LogPasswordSubmitted(int progressId)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE Progress
                        SET PasswordSubmitted = 1
                        WHERE Progress.ProgressID = @progressId",
                new { progressId }
            );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not loggin password submitted as db update failed. " +
                    $"Progress id: {progressId}"
                );
            }
        }

        public void UpdateProgress(int progressId)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE Progress
                        SET LoginCount = (SELECT COALESCE(COUNT(*), 0)
                            FROM Sessions AS S
                            WHERE S.CandidateID = Progress.CandidateID
                              AND S.CustomisationID = Progress.CustomisationID
                              AND S.LoginTime >= Progress.FirstSubmittedTime),
                            Duration = (SELECT COALESCE(SUM(S1.Duration), 0)
                            FROM Sessions AS S1
                            WHERE S1.CandidateID = Progress.CandidateID
                              AND S1.CustomisationID = Progress.CustomisationID
                              AND S1.LoginTime >= Progress.FirstSubmittedTime),
                            SubmittedTime = GETUTCDATE()
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

        private int? GetProgressId(int candidateId, int customisationId)
        {
            try
            {
                return connection.QueryFirst<int>(
                    @"SELECT ProgressId
                    FROM Progress
                    WHERE CandidateID = @candidateId
                      AND CustomisationID = @customisationId
                      AND SystemRefreshed = 0
                      AND RemovedDate IS NULL",
                    new { candidateId, customisationId }
                );
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
    }
}
