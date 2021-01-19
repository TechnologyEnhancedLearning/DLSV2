namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.DiagnosticAssessment;
    using Microsoft.Extensions.Logging;

    public interface IDiagnosticAssessmentDataService
    {
        DiagnosticAssessment? GetDiagnosticAssessment(int customisationId, int candidateId, int sectionId);
        DiagnosticContent? GetDiagnosticContent(int customisationId, int sectionId);
    }

    public class DiagnosticAssessmentDataService : IDiagnosticAssessmentDataService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<DiagnosticAssessmentDataService> logger;

        public DiagnosticAssessmentDataService(IDbConnection connection, ILogger<DiagnosticAssessmentDataService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public DiagnosticAssessment? GetDiagnosticAssessment(int customisationId, int candidateId, int sectionId)
        {
            DiagnosticAssessment? diagnosticAssessment = null;
            return connection.Query<DiagnosticAssessment, DiagnosticTutorial, DiagnosticAssessment>(
                @"
                    WITH NextTutorialAndSectionNumbers AS (
                        SELECT TOP (1)
                            MIN(NextTutorials.TutorialID) AS NextTutorialId,
                            MIN(NextSections.SectionID) AS NextSectionId
                        FROM Sections
                            INNER JOIN Customisations
                                ON Customisations.ApplicationID = Sections.ApplicationID
                            INNER JOIN Applications
                                ON Applications.ApplicationID = Sections.ApplicationID
                                                    
                            LEFT JOIN CustomisationTutorials AS NextSectionCustomisationTutorials
                                ON NextSectionCustomisationTutorials.CustomisationID = Customisations.CustomisationID
                            LEFT JOIN Tutorials AS NextSectionTutorials
                                ON NextSectionTutorials.TutorialID = NextSectionCustomisationTutorials.TutorialID
                                AND NextSectionTutorials.ArchivedDate IS NULL
                            LEFT JOIN Sections AS NextSections
                                ON NextSections.SectionID = NextSectionTutorials.SectionID
                                AND NextSections.ArchivedDate IS NULL
                                AND (
                                    Sections.SectionNumber < NextSections.SectionNumber
                                    OR Sections.SectionID < NextSections.SectionID
                                )
                                AND (
                                    NextSectionCustomisationTutorials.Status = 1
                                    OR NextSectionCustomisationTutorials.DiagStatus = 1
                                    OR NextSections.PLAssessPath IS NOT NULL
                                )
                            LEFT JOIN CustomisationTutorials AS NextCustomisationTutorials
                                ON NextCustomisationTutorials.CustomisationID = @customisationId
                                AND NextCustomisationTutorials.Status = 1
                            LEFT JOIN Tutorials AS NextTutorials
                                ON NextTutorials.TutorialID = NextCustomisationTutorials.TutorialID
                                AND NextTutorials.ArchivedDate IS NULL
                                AND NextTutorials.SectionID = @sectionId
                        WHERE
                            Customisations.CustomisationID = @customisationId
                            AND Customisations.Active = 1
                            AND Sections.SectionID = @sectionId
                            AND Sections.ArchivedDate IS NULL
                            AND Sections.DiagAssessPath IS NOT NULL
                        GROUP BY
                            NextTutorials.OrderByNumber, NextSections.SectionNumber
                        ORDER BY
                            CASE
		                        WHEN NextTutorials.OrderByNumber IS NULL THEN 1
                                ELSE 0
                            END,
	                        NextTutorials.OrderByNumber,
                            CASE
		                        WHEN NextSections.SectionNumber IS NULL THEN 1
                                ELSE 0
                            END,
	                        NextSections.SectionNumber
                    )
                    SELECT
                        Applications.ApplicationName,
                        Customisations.CustomisationName,
                        Sections.SectionName,
                        COALESCE (aspProgress.DiagAttempts, 0) AS DiagAttempts,
                        COALESCE (aspProgress.DiagLast, 0) AS DiagLast,
                        Tutorials.DiagAssessOutOf,
                        Sections.DiagAssessPath,
                        Customisations.DiagObjSelect,
                        NextTutorialAndSectionNumbers.NextTutorialId,
                        NextTutorialAndSectionNumbers.NextSectionId,
                        Tutorials.TutorialName,
                        CASE WHEN Tutorials.OriginalTutorialID > 0
                             THEN Tutorials.OriginalTutorialID
                             ELSE Tutorials.TutorialID
                        END AS id
                    FROM Sections
                        INNER JOIN Customisations
                            ON Customisations.ApplicationID = Sections.ApplicationID
                            AND Customisations.Active = 1
                        INNER JOIN Applications
                            ON Applications.ApplicationID = Sections.ApplicationID
                        INNER JOIN CustomisationTutorials
                            ON CustomisationTutorials.CustomisationID = Customisations.CustomisationID
                        INNER JOIN Tutorials
                            ON CustomisationTutorials.TutorialID = Tutorials.TutorialID
                            AND Tutorials.SectionID = Sections.SectionID
                        LEFT JOIN Progress
                            ON Progress.CustomisationID = Customisations.CustomisationID
                            AND Progress.CandidateID = @candidateId
                            AND Progress.RemovedDate IS NULL
                            AND Progress.SystemRefreshed = 0
                        LEFT JOIN aspProgress
                            ON aspProgress.TutorialID = CustomisationTutorials.TutorialID
                            AND aspProgress.ProgressID = Progress.ProgressID
                        LEFT JOIN NextTutorialAndSectionNumbers
                            ON NextTutorialAndSectionNumbers.NextTutorialId IS NOT NULL
                            OR NextTutorialAndSectionNumbers.NextSectionId IS NOT NULL
                    WHERE
                        Customisations.CustomisationID = @customisationId
                        AND Sections.SectionID = @sectionId
                        AND Sections.ArchivedDate IS NULL
                        AND Sections.DiagAssessPath IS NOT NULL
                        AND CustomisationTutorials.DiagStatus = 1
                        AND Tutorials.DiagAssessOutOf > 0
                        AND Tutorials.ArchivedDate IS NULL
                    ORDER BY
                        Tutorials.OrderByNumber,
                        id",
                (diagnostic, tutorial) =>
                {
                    if (diagnosticAssessment == null)
                    {
                        diagnosticAssessment = diagnostic;
                    }
                    else
                    {
                        diagnosticAssessment.DiagnosticAttempts = Math.Max(
                            diagnosticAssessment.DiagnosticAttempts,
                            diagnostic.DiagnosticAttempts
                        );
                        diagnosticAssessment.SectionScore += diagnostic.SectionScore;
                        diagnosticAssessment.MaxSectionScore += diagnostic.MaxSectionScore;
                    }

                    diagnosticAssessment.Tutorials.Add(tutorial);

                    return diagnosticAssessment;
                },
                new { customisationId, candidateId, sectionId },
                splitOn: "TutorialName"
            ).FirstOrDefault();
        }

        public DiagnosticContent? GetDiagnosticContent(int customisationId, int sectionId)
        {
            DiagnosticContent? diagnosticContent = null;
            return connection.Query<DiagnosticContent, int, DiagnosticContent>(
                @"
                    SELECT
                        Applications.ApplicationName,
                        Customisations.CustomisationName,
                        Sections.SectionName,
                        Sections.DiagAssessPath,
                        Customisations.DiagObjSelect,
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
                        INNER JOIN Applications
                            ON Applications.ApplicationID = Sections.ApplicationID
                        INNER JOIN CustomisationTutorials
                            ON CustomisationTutorials.CustomisationID = Customisations.CustomisationID
                            AND CustomisationTutorials.DiagStatus = 1
                        INNER JOIN Tutorials
                            ON Tutorials.TutorialID = CustomisationTutorials.TutorialID
                            AND Tutorials.SectionID = Sections.SectionID
                            AND Tutorials.DiagAssessOutOf > 0
                            AND Tutorials.ArchivedDate IS NULL
                    WHERE
                        Customisations.CustomisationID = @customisationId
                        AND Sections.SectionID = @sectionId
                        AND Sections.ArchivedDate IS NULL
                        AND Sections.DiagAssessPath IS NOT NULL
                    ORDER BY
                        Tutorials.OrderByNumber,
                        Tutorials.TutorialID",
                (diagnostic, tutorialId) =>
                {
                    diagnosticContent ??= diagnostic;

                    diagnosticContent.Tutorials.Add(tutorialId);

                    return diagnosticContent;
                },
                new { customisationId, sectionId },
                splitOn: "id"
            ).FirstOrDefault();
        }
    }
}
