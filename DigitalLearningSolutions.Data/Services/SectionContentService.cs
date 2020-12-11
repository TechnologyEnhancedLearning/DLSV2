namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.SectionContent;
    using Microsoft.Extensions.Logging;

    public interface ISectionContentService
    {
        SectionContent? GetSectionContent(int customisationId, int candidateId, int sectionId);
    }

    public class SectionContentService : ISectionContentService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<SectionContentService> logger;

        public SectionContentService(IDbConnection connection, ILogger<SectionContentService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public SectionContent? GetSectionContent(int customisationId, int candidateId, int sectionId)
        {
            SectionContent? sectionContent = null;
            return connection.Query<SectionContent, SectionTutorial, SectionContent>(
                @"
                    SELECT 
                        SectionContent.ApplicationName,
                        SectionContent.CustomisationName,
                        SectionContent.SectionName,
                        COALESCE (SectionContent.TimeMins, 0) AS TimeMins,
                        SectionContent.AverageSectionTime,
                        SectionContent.HasLearning,
                        SectionContent.PercentComplete,
                        SectionContent.DiagAttempts,
                        COALESCE (SectionContent.SecScore, 0) AS SecScore,
                        SectionContent.SecOutOf,
                        SectionContent.DiagAssessPath,
                        SectionContent.PLAssessPath,
                        SectionContent.AttemptsPL,
                        SectionContent.PLPassed,
                        CustomisationTutorials.DiagStatus,
                        SectionContent.IsAssessed,
                        Tutorials.TutorialName,
                        COALESCE (TutStatus.Status, 'Not started') AS CompletionStatus,
                        COALESCE (aspProgress.TutTime, 0) AS TutorialTime,
                        Tutorials.AverageTutMins AS AverageTutorialTime
                    FROM
                        (SELECT
                            Customisations.CustomisationName,
                            Applications.ApplicationName,
                            Sections.SectionName,
                            SUM(aspProgress.TutTime) AS TimeMins, 
                            Sections.AverageSectionMins AS AverageSectionTime, 
                            dbo.CheckCustomisationSectionHasLearning(Customisations.CustomisationID, Sections.SectionID) AS HasLearning,
                            (CASE
                                WHEN Progress.CandidateID IS NULL
                                    OR dbo.CheckCustomisationSectionHasLearning(Progress.CustomisationID, Sections.SectionID) = 0
                                THEN 0
                                ELSE CAST(SUM(aspProgress.TutStat) * 100 AS FLOAT) / (COUNT(Tutorials.TutorialID) * 2)
                            END) AS PercentComplete,
                            MAX(ISNULL(aspProgress.DiagAttempts, 0)) AS DiagAttempts,
                            SUM(aspProgress.DiagLast) AS SecScore,
                            SUM(Tutorials.DiagAssessOutOf) AS SecOutOf,
                            Sections.DiagAssessPath, 
                            Sections.PLAssessPath,
                            Sections.SectionID,
                            COALESCE (Attempts.AttemptsPL,0) AS AttemptsPL,
                            COALESCE (Attempts.PLPassed,0) AS PLPassed,
                            Customisations.IsAssessed
                        FROM Tutorials
                            INNER JOIN CustomisationTutorials
                                ON CustomisationTutorials.TutorialID = Tutorials.TutorialID
                            INNER JOIN Customisations
                                ON Customisations.CustomisationID = CustomisationTutorials.CustomisationID
                            INNER JOIN Applications
                                ON Applications.ApplicationID = Customisations.ApplicationID
                            INNER JOIN Sections
                                ON Sections.SectionID = Tutorials.SectionID
                            LEFT JOIN Progress
                                ON Progress.CustomisationID = Customisations.CustomisationID
                                AND Progress.CandidateID = @candidateId
                                AND Progress.RemovedDate IS NULL
                                AND Progress.SystemRefreshed = 0
                            LEFT JOIN aspProgress
                                ON aspProgress.TutorialID = CustomisationTutorials.TutorialID
                                AND aspProgress.ProgressID = Progress.ProgressID
                            LEFT JOIN (SELECT
                                            COUNT(AssessAttemptID) AS AttemptsPL,
                                            AssessAttempts.ProgressID,
                                            AssessAttempts.SectionNumber,
                                            COALESCE (MAX(ISNULL(CAST(AssessAttempts.Status AS Integer), 0)), 0) AS PLPassed
                                        FROM AssessAttempts
                                        GROUP BY
                                            AssessAttempts.ProgressID,
                                            AssessAttempts.SectionNumber
                                        ) AS Attempts ON (Progress.ProgressID = Attempts.ProgressID) AND (Attempts.SectionNumber = Sections.SectionNumber)
                        WHERE
                            CustomisationTutorials.CustomisationID = @customisationId
                            AND Sections.SectionID = @sectionId
                            AND (Sections.ArchivedDate IS NULL)
                            AND (CustomisationTutorials.DiagStatus = 1 OR Customisations.IsAssessed = 1 OR CustomisationTutorials.Status = 1)
                        GROUP BY
                            Sections.SectionID,
                            Customisations.CustomisationID,
                            Progress.ProgressID,
                            Progress.CustomisationID,
                            Progress.CandidateID,
                            Customisations.CustomisationName,
                            Applications.ApplicationName,
                            Sections.SectionName, 
                            Sections.DiagAssessPath,
                            Sections.PLAssessPath,
                            CustomisationTutorials.DiagStatus,
                            Sections.SectionNumber,
                            Sections.AverageSectionMins,
                            Attempts.AttemptsPL,
                            Attempts.PLPassed,
                            Progress.ProgressId,
                            Customisations.IsAssessed
                        ) AS SectionContent

                        INNER JOIN Tutorials ON SectionContent.SectionID = Tutorials.SectionID
                        INNER JOIN CustomisationTutorials ON Tutorials.TutorialID = CustomisationTutorials.TutorialID
                        LEFT JOIN Progress
                            ON Progress.CustomisationID = CustomisationTutorials.CustomisationID
                            AND Progress.CandidateID = @candidateId
                            AND Progress.RemovedDate IS NULL
                            AND Progress.SystemRefreshed = 0
                        LEFT JOIN aspProgress
                            ON aspProgress.TutorialID = CustomisationTutorials.TutorialID
                            AND aspProgress.ProgressID = Progress.ProgressID
                        LEFT JOIN TutStatus
                            ON aspProgress.TutStat = TutStatus.TutStatusID
                        WHERE CustomisationTutorials.CustomisationID = @customisationId
                        ORDER BY CustomisationTutorials.TutorialID",
                    (section, tutorial) =>
                {
                    sectionContent ??= section;

                    sectionContent.Tutorials.Add(tutorial);
                    return sectionContent;
                },
                new { customisationId, candidateId, sectionId },
                splitOn: "TutorialName"
            ).FirstOrDefault();
        }
    }
}
