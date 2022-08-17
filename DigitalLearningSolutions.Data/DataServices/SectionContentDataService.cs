namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.SectionContent;
    using Microsoft.Extensions.Logging;

    public interface ISectionContentDataService
    {
        SectionContent? GetSectionContent(int customisationId, int candidateId, int sectionId);

        IEnumerable<Section> GetSectionsForApplication(int applicationId);

        Section? GetSectionById(int sectionId);
    }

    public class SectionContentDataService : ISectionContentDataService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<SectionContentDataService> logger;

        public SectionContentDataService(IDbConnection connection, ILogger<SectionContentDataService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public SectionContent? GetSectionContent(int customisationId, int candidateId, int sectionId)
        {
            // NextSectionID is the ID of the next section in the course, according to SectionNumber
            // or null if the last in the course.

            // Find these by making a list of other sections in the course. A section is valid if it is not archived
            // and has one of the following:

            // A tutorial which can be viewed; if it has CustomisationTutorials.Status 1.

            // A diagnostic assessment; if DiagAssessPath != null, and it contains a tutorial with
            // CustomisationTutorials.DiagStatus = 1. NB: this doesn't need to have Status = 1

            // A post learning assessment; if PLAssessPath != null and Customisations.IsAssessed = 1
            // Consolidation material; if ConsolidationPath != null

            SectionContent? sectionContent = null;
            return connection.Query<SectionContent, SectionTutorial, SectionContent>(
                @"WITH OtherSections AS (
                        SELECT
                            CurrentSection.SectionID AS CurrentSectionID,
                            CurrentSection.SectionNumber AS CurrentSectionNumber,
                            OtherSections.SectionID AS OtherSectionID,
                            OtherSections.SectionNumber AS OtherSectionNumber
                        FROM Sections AS CurrentSection
                                INNER JOIN CustomisationTutorials AS OtherCustomisationTutorials
                                    ON OtherCustomisationTutorials.CustomisationID = @customisationId
                                INNER JOIN Customisations
                                    ON OtherCustomisationTutorials.CustomisationID = Customisations.CustomisationID
                                INNER JOIN Tutorials AS OtherSectionsTutorials
                                    ON OtherCustomisationTutorials.TutorialID = OtherSectionsTutorials.TutorialID
                                    AND OtherSectionsTutorials.ArchivedDate IS NULL
                                INNER JOIN Sections AS OtherSections
                                    ON OtherSectionsTutorials.SectionID = OtherSections.SectionID
                                    AND CurrentSection.SectionID <> OtherSections.SectionID
                        WHERE CurrentSection.SectionID = @sectionId
                            AND Customisations.Active = 1
                            AND OtherSections.ArchivedDate IS NULL
                            AND (
                                 OtherCustomisationTutorials.Status = 1
                                 OR (OtherCustomisationTutorials.DiagStatus = 1 AND OtherSections.DiagAssessPath IS NOT NULL)
                                 OR (Customisations.IsAssessed = 1 AND OtherSections.PLAssessPath IS NOT NULL)
                            )
                    ),
                    NextSectionIdTable AS (
                        SELECT TOP (1)
                            CurrentSectionID,
                            OtherSectionID AS NextSectionID,
                            OtherSectionNumber AS NextSectionNumber
                        FROM OtherSections
                        WHERE CurrentSectionNumber <= OtherSections.OtherSectionNumber
                            AND (
                                 CurrentSectionNumber < OtherSections.OtherSectionNumber
                                 OR CurrentSectionID < OtherSections.OtherSectionID
                            )
                        ORDER BY OtherSectionNumber, OtherSectionID
                    )
                    SELECT
                        Applications.ApplicationName,
                        Applications.ApplicationInfo,
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
                        Applications.IncludeCertification,
                        Progress.Completed,
                        Applications.AssessAttempts AS MaxPostLearningAssessmentAttempts,
                        Applications.PLAPassThreshold AS PostLearningAssessmentPassThreshold,
                        Customisations.DiagCompletionThreshold AS DiagnosticAssessmentCompletionThreshold,
                        Customisations.TutCompletionThreshold AS TutorialsCompletionThreshold,
                        CAST (CASE
                                    WHEN EXISTS(SELECT 1 FROM OtherSections) THEN 1
                                    ELSE 0
                              END AS BIT) AS OtherSectionsExist,
                        NextSectionIdTable.NextSectionId,
                        Customisations.Password,
                        Progress.PasswordSubmitted,
                        Tutorials.TutorialName,
                        COALESCE (aspProgress.TutStat, 0) AS TutStat,
                        COALESCE (TutStatus.Status, 'Not started') AS CompletionStatus,
                        COALESCE (aspProgress.TutTime, 0) AS TutTime,
                        Tutorials.AverageTutMins,
                        Tutorials.TutorialID AS id,
                        CustomisationTutorials.Status,
                        COALESCE (aspProgress.DiagLast, 0) AS CurrentScore,
                        Tutorials.DiagAssessOutOf AS PossibleScore,
                        CustomisationTutorials.DiagStatus AS TutorialDiagnosticStatus,
                        COALESCE (aspProgress.DiagAttempts, 0) AS TutorialDiagnosticAttempts
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
                            ON Sections.SectionID = NextSectionIdTable.CurrentSectionID
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
                        AND Applications.DefaultContentTypeID <> 4
                        ORDER BY Tutorials.OrderByNumber, Tutorials.TutorialID",
                (section, tutorial) =>
                {
                    if (sectionContent == null)
                    {
                        sectionContent = section;
                        sectionContent.DiagnosticAttempts =
                            section.DiagnosticStatus ? sectionContent.DiagnosticAttempts : 0;
                        sectionContent.SectionScore = section.DiagnosticStatus ? sectionContent.SectionScore : 0;
                        sectionContent.MaxSectionScore = section.DiagnosticStatus ? sectionContent.MaxSectionScore : 0;
                    }
                    else if (section.DiagnosticStatus)
                    {
                        sectionContent.DiagnosticStatus = section.DiagnosticStatus;
                        sectionContent.DiagnosticAttempts = Math.Max(
                            sectionContent.DiagnosticAttempts,
                            section.DiagnosticAttempts
                        );
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

        public IEnumerable<Section> GetSectionsForApplication(int applicationId)
        {
            return connection.Query<Section>(
                @"SELECT
                        SectionID,
                        SectionName
                    FROM dbo.Sections
                    WHERE ApplicationID = @applicationId
                    AND ArchivedDate IS NULL",
                new { applicationId }
            );
        }

        public Section? GetSectionById(int sectionId)
        {
            return connection.Query<Section>(
                @"SELECT
                        SectionID,
                        SectionName
                    FROM dbo.Sections
                    WHERE SectionId = @sectionId",
                new { sectionId }
            ).SingleOrDefault();
        }
    }
}
