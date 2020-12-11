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

            CourseContent? courseContent = null;
            return connection.Query<CourseContent, CourseSection, CourseContent>(
                @"SELECT Customisations.CustomisationID AS id,
                         Applications.ApplicationName,
                         Customisations.CustomisationName,
                         dbo.GetMinsForCustomisation(Customisations.CustomisationID) AS AverageDuration,
                         Centres.CentreName,
                         Centres.BannerText,
                         Applications.IncludeCertification,
                         Progress.Completed,
                         Sections.SectionName,
                         Sections.SectionID AS id,
                         dbo.CheckCustomisationSectionHasLearning(Customisations.CustomisationID, Sections.SectionID) AS HasLearning,
                         (CASE
                            WHEN Progress.CandidateID IS NULL
                                 OR dbo.CheckCustomisationSectionHasLearning(Customisations.CustomisationID, Sections.SectionID) = 0
                            THEN 0
                            ELSE CAST(SUM(aspProgress.TutStat) * 100 AS FLOAT) / (COUNT(Tutorials.TutorialID) * 2)
                         END) AS PercentComplete,
                         SUM(aspProgress.TutTime) as TimeMins,
                         Sections.AverageSectionMins as AverageSectionTime
                  FROM Applications
                  INNER JOIN Customisations ON Applications.ApplicationID = Customisations.ApplicationID
                  INNER JOIN Sections ON Sections.ApplicationID = Applications.ApplicationID
                  INNER JOIN Centres ON Customisations.CentreID = Centres.CentreID
                  INNER JOIN Tutorials ON Sections.SectionID = Tutorials.SectionID
                  INNER JOIN CustomisationTutorials ON Customisations.CustomisationID = CustomisationTutorials.CustomisationID
                                                   AND Tutorials.TutorialID = CustomisationTutorials.TutorialID
                  LEFT JOIN Progress ON Customisations.CustomisationID = Progress.CustomisationID AND Progress.CandidateID = @candidateId AND Progress.RemovedDate IS NULL AND Progress.SystemRefreshed = 0
                  LEFT JOIN aspProgress ON aspProgress.ProgressID = Progress.ProgressID AND aspProgress.TutorialID = Tutorials.TutorialID
                  WHERE Customisations.CustomisationID = @customisationId
                    AND Sections.ArchivedDate IS NULL
                    AND (CustomisationTutorials.Status = 1 OR CustomisationTutorials.DiagStatus = 1 OR Customisations.IsAssessed = 1)
                  GROUP BY
                         Sections.SectionID,
                         Customisations.CustomisationID,
                         Applications.ApplicationName,
                         Customisations.CustomisationName,
                         Centres.CentreName,
                         Centres.BannerText,
                         Applications.IncludeCertification,
                         Progress.Completed,
                         Sections.SectionName,
                         Sections.SectionID,
                         Sections.SectionNumber,
                         Progress.CandidateID,
						 Sections.AverageSectionMins
                  ORDER BY Sections.SectionNumber;",
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
                    EnrolledByAdminID = 0
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
                        $"Candidate id: {candidateId}, customisation id: {customisationId}, centre id: {centreId}");
                    break;
                case 100:
                    logger.LogError(
                        "Not enrolled candidate on course as customisation id doesn't match centre id. " +
                        $"Candidate id: {candidateId}, customisation id: {customisationId}, centre id: {centreId}");
                    break;
                case 101:
                    logger.LogError(
                        "Not enrolled candidate on course as candidate id doesn't match centre id. " +
                        $"Candidate id: {candidateId}, customisation id: {customisationId}, centre id: {centreId}");
                    break;
                default:
                    logger.LogError(
                        "Not enrolled candidate on course as stored procedure failed. " +
                        $"Unknown error code: {errorCode}, candidate id: {candidateId}, customisation id: {customisationId}, centre id: {centreId}");
                    break;
            }

            return null;
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
