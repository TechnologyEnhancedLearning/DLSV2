﻿namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.DiagnosticAssessment;
    using Microsoft.Extensions.Logging;

    public interface IDiagnosticAssessmentService
    {
        public DiagnosticAssessment? GetDiagnosticAssessment(int customisationId, int candidateId, int sectionId);
    }

    public class DiagnosticAssessmentService : IDiagnosticAssessmentService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<DiagnosticAssessmentService> logger;

        public DiagnosticAssessmentService(IDbConnection connection, ILogger<DiagnosticAssessmentService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public DiagnosticAssessment? GetDiagnosticAssessment(int customisationId, int candidateId, int sectionId)
        {
            DiagnosticAssessment? diagnosticAssessment = null;
            return connection.Query<DiagnosticAssessment, DiagnosticTutorial, DiagnosticAssessment>(
                @"
                    SELECT
                        Applications.ApplicationName,
                        Customisations.CustomisationName,
                        Sections.SectionName,
                        COALESCE (aspProgress.DiagAttempts, 0) AS DiagAttempts,
                        COALESCE (aspProgress.DiagLast, 0) AS DiagLast,
                        Tutorials.DiagAssessOutOf,
                        Sections.DiagAssessPath,
                        Customisations.DiagObjSelect,
                        Tutorials.TutorialName,
                        Tutorials.TutorialID AS id,
                        CustomisationTutorials.Status
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
                            AND (CustomisationTutorials.Status = 1 OR CustomisationTutorials.DiagStatus = 1 OR Customisations.IsAssessed = 1)
                        LEFT JOIN Progress
                            ON Progress.CustomisationID = Customisations.CustomisationID
                            AND Progress.CandidateID = @candidateId
                            AND Progress.RemovedDate IS NULL
                            AND Progress.SystemRefreshed = 0
                        LEFT JOIN aspProgress
                            ON aspProgress.TutorialID = CustomisationTutorials.TutorialID
                            AND aspProgress.ProgressID = Progress.ProgressID
                    WHERE
                        Customisations.CustomisationID = @customisationId
                        AND Sections.SectionID = @sectionId
                        AND Sections.ArchivedDate IS NULL
                        AND Sections.DiagAssessPath IS NOT NULL
                    ORDER BY
                        Tutorials.OrderByNumber,
                        Tutorials.TutorialID",
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

                    if (tutorial.IsDisplayed)
                    {
                            diagnosticAssessment.Tutorials.Add(tutorial);
                    }

                    return diagnosticAssessment;
                },
                new { customisationId, candidateId, sectionId },
                splitOn: "TutorialName"
            ).FirstOrDefault();
        }

        public DiagnosticContent? GetDiagnosticContent(int customisationId, int sectionId)
        {
            DiagnosticContent? diagnosticContent = null;
            return connection.Query<DiagnosticContent, int?, DiagnosticContent>(
                @"
                    SELECT
                        Applications.ApplicationName,
                        Customisations.CustomisationName,
                        Sections.SectionName,
                        Sections.DiagAssessPath,
                        Customisations.DiagObjSelect,
                        Applications.PLAPassThreshold,
                        Customisations.CurrentVersion,
                        Tutorials.TutorialID AS id
                    FROM Sections
                        INNER JOIN Customisations
                            ON Customisations.ApplicationID = Sections.ApplicationID
                            AND Customisations.Active = 1
                        INNER JOIN Applications
                            ON Applications.ApplicationID = Sections.ApplicationID
                        INNER JOIN CustomisationTutorials
                            ON CustomisationTutorials.CustomisationID = Customisations.CustomisationID
                        LEFT JOIN Tutorials
                            ON CustomisationTutorials.TutorialID = Tutorials.TutorialID
                            AND Tutorials.SectionID = Sections.SectionID
                            AND CustomisationTutorials.Status = 1
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
                    if (diagnosticContent == null)
                    {
                        diagnosticContent = diagnostic;
                    }

                    if (tutorialId != null)
                    {
                        diagnosticContent.Tutorials.Add(tutorialId.Value);
                    }

                    return diagnosticContent;
                },
                new { customisationId, sectionId },
                splitOn: "id"
            ).FirstOrDefault();
        }
    }
}
