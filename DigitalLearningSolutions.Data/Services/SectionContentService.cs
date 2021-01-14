namespace DigitalLearningSolutions.Data.Services
{
    using System;
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
                        dbo.CheckCustomisationSectionHasLearning(Customisations.CustomisationID, Sections.SectionID) AS HasLearning,
                        COALESCE (aspProgress.DiagAttempts, 0) AS DiagAttempts,
                        COALESCE (aspProgress.DiagLast, 0) AS DiagLast,
                        Tutorials.DiagAssessOutOf,
                        Sections.DiagAssessPath,
                        Sections.PLAssessPath,
                        COALESCE (Attempts.AttemptsPL, 0) AS AttemptsPL,
                        COALESCE (Attempts.PLPasses, 0) AS PLPasses,
                        CustomisationTutorials.DiagStatus,
                        Customisations.IsAssessed,
                        Sections.ConsolidationPath,
                        Applications.CourseSettings,
                        NextSectionIdTable.NextSectionId,
                        Tutorials.TutorialName,
                        COALESCE (aspProgress.TutStat, 0) AS TutStat,
                        COALESCE (TutStatus.Status, 'Not started') AS CompletionStatus,
                        COALESCE (aspProgress.TutTime, 0) AS TutTime,
                        Tutorials.AverageTutMins,
                        Tutorials.TutorialID AS id,
                        CustomisationTutorials.Status
                    FROM Tutorials
                        INNER JOIN CustomisationTutorials
                            ON CustomisationTutorials.TutorialID = Tutorials.TutorialID
                        INNER JOIN Customisations
                            ON Customisations.CustomisationID = CustomisationTutorials.CustomisationID
                        INNER JOIN Applications
                            ON Applications.ApplicationID = Customisations.ApplicationID
                        INNER JOIN Sections
                            ON Sections.SectionID = Tutorials.SectionID
                        LEFT JOIN NextSectionIdTable
                            ON Sections.SectionID = NextSectionIdTable.SectionID
                        LEFT JOIN Progress
                            ON Progress.CustomisationID = Customisations.CustomisationID
                            AND Progress.CandidateID = @candidateId
                            AND Progress.RemovedDate IS NULL
                            AND Progress.SystemRefreshed = 0
                        LEFT JOIN aspProgress
                            ON aspProgress.TutorialID = CustomisationTutorials.TutorialID
                            and aspProgress.ProgressID = Progress.ProgressID
                        LEFT JOIN (SELECT COUNT(AssessAttemptID) AS AttemptsPL,
                            AssessAttempts.ProgressID,
                            AssessAttempts.SectionNumber,
                            SUM(CAST(AssessAttempts.Status AS Integer)) AS PLPasses
                            FROM AssessAttempts
                            GROUP BY
                                AssessAttempts.ProgressID,
                                AssessAttempts.SectionNumber
                        ) AS Attempts ON (Progress.ProgressID = Attempts.ProgressID) AND (Attempts.SectionNumber = Sections.SectionNumber)
                        LEFT JOIN TutStatus
                            ON aspProgress.TutStat = TutStatus.TutStatusID
                    WHERE
                        CustomisationTutorials.CustomisationID = @customisationId
                        AND Sections.SectionID = @sectionId
                        AND Sections.ArchivedDate IS NULL
                        AND (CustomisationTutorials.DiagStatus = 1 OR Customisations.IsAssessed = 1 OR CustomisationTutorials.Status = 1)
                        AND Customisations.Active = 1
                        AND Tutorials.ArchivedDate IS NULL
                        ORDER BY Tutorials.OrderByNumber, Tutorials.TutorialID",
                    (section, tutorial) =>
                        {
                    if (sectionContent == null)
                    {
                        sectionContent = section;
                        sectionContent.DiagnosticAttempts = section.DiagnosticStatus ? sectionContent.DiagnosticAttempts : 0;
                        sectionContent.SectionScore = section.DiagnosticStatus ? sectionContent.SectionScore : 0;
                        sectionContent.MaxSectionScore = section.DiagnosticStatus ? sectionContent.MaxSectionScore : 0;
                    }
                    else if (section.DiagnosticStatus)
                    {
                        sectionContent.DiagnosticStatus = section.DiagnosticStatus;
                        sectionContent.DiagnosticAttempts = Math.Max(sectionContent.DiagnosticAttempts, section.DiagnosticAttempts);
                        sectionContent.SectionScore += section.SectionScore;
                        sectionContent.MaxSectionScore += section.MaxSectionScore;
                    }

                    if (tutorial.CustomisationTutorialStatus)
                    {
                        sectionContent.Tutorials.Add(tutorial);
                    }

                    return sectionContent;
                },
                new { customisationId, candidateId, sectionId },
                splitOn: "TutorialName"
            ).FirstOrDefault();
        }
    }
}
