namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.CourseContent;
    using Microsoft.Extensions.Logging;

    public interface ICourseContentService
    {
        CourseContent? GetCourseContent(int candidateId, int customisationId);
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

        public CourseContent? GetCourseContent(int candidateId, int customisationId)
        {
            CourseContent? courseContent = null;
            return connection.Query<CourseContent, CourseSection, CourseContent>(
                @"SELECT Customisations.CustomisationID AS id,
                         Applications.ApplicationName,
                         Customisations.CustomisationName,
                         dbo.GetMinsForCustomisation(Customisations.CustomisationID) AS AverageDuration,
                         Centres.CentreName,
                         Centres.BannerText,
                         Sections.SectionName,
                         dbo.CheckCustomisationSectionHasLearning(Customisations.CustomisationID, Sections.SectionID) AS HasLearning,
                         (CASE
                            WHEN Progress.CandidateID IS NULL
                                 OR dbo.CheckCustomisationSectionHasLearning(Customisations.CustomisationID, Sections.SectionID) = 0
                            THEN 0
                            ELSE CAST(SUM(aspProgress.TutStat) * 100 AS FLOAT) / (COUNT(Tutorials.TutorialID) * 2)
                         END) AS PercentComplete
                  FROM Applications
                  INNER JOIN Customisations ON Applications.ApplicationID = Customisations.ApplicationID
                  INNER JOIN Sections ON Sections.ApplicationID = Applications.ApplicationID
                  INNER JOIN Centres ON Customisations.CentreID = Centres.CentreID
                  LEFT JOIN Tutorials ON Sections.SectionID = Tutorials.SectionID
                  LEFT JOIN Progress ON Customisations.CustomisationID = Progress.CustomisationID AND Progress.CandidateID = @candidateId
                  LEFT JOIN aspProgress ON aspProgress.ProgressID = Progress.ProgressID AND aspProgress.TutorialID = Tutorials.TutorialID
                  WHERE Customisations.CustomisationID = @customisationId
                    AND ((Progress.CandidateID IS NULL) OR (Progress.SystemRefreshed = 0 AND Progress.RemovedDate IS NULL))
                  GROUP BY
                         Sections.SectionID,
                         Customisations.CustomisationID,
                         Applications.ApplicationName,
                         Customisations.CustomisationName,
                         Centres.CentreName,
                         Centres.BannerText,
                         Sections.SectionName,
                         Sections.SectionNumber,
                         Progress.CandidateID
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
