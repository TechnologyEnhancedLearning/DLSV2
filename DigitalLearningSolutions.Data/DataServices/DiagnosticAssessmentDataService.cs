namespace DigitalLearningSolutions.Data.DataServices
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
            // NextTutorialID is the ID of the first tutorial in the section, according to Tutorials.OrderBy
            // or null if the last in the section. Similar for NextSectionID, using SectionID and SectionNumber

            // Find these by making a list of other tutorials in the course, to then find tutorials in this section,
            // and other sections (because a section must contain at least one tutorial), using a similar approach to
            // the one used in the TutorialContentDataService.

            // Using this list of other tutorials in the course we can work out if there is another item in the
            // section (if there is an viewable tutorial, or a post learning assessment, or consolidation material),
            // and if there are other sections (valid tutorials with a different section ID, or with assessments or
            // consolidation material). See the SectionContentDataService for the definition of a valid section.

            DiagnosticAssessment? diagnosticAssessment = null;
            return connection.Query<DiagnosticAssessment, DiagnosticTutorial, DiagnosticAssessment>(
                @"  WITH CourseTutorials AS (
                    SELECT Tutorials.TutorialID,
                           Tutorials.OrderByNumber,
                           CustomisationTutorials.Status,
                           Sections.SectionID,
                           Sections.SectionNumber
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
                    ),
                    NextTutorial AS (
                    SELECT TOP 1
                           CourseTutorials.SectionID AS CurrentSectionID,
                           CourseTutorials.TutorialID AS NextTutorialID
                      FROM CourseTutorials
                           INNER JOIN CustomisationTutorials AS OtherCustomisationTutorials
                              ON OtherCustomisationTutorials.CustomisationID = @customisationId
                              AND OtherCustomisationTutorials.Status = 1
                     WHERE CourseTutorials.SectionID = @sectionId
                     ORDER BY CourseTutorials.OrderByNumber, CourseTutorials.TutorialID
                    )
                    SELECT
                        Applications.ApplicationName,
                        Applications.ApplicationInfo,
                        Customisations.CustomisationName,
                        Sections.SectionName,
                        COALESCE (aspProgress.DiagAttempts, 0) AS DiagAttempts,
                        COALESCE (aspProgress.DiagLast, 0) AS DiagLast,
                        Tutorials.DiagAssessOutOf,
                        Sections.DiagAssessPath,
                        Customisations.DiagObjSelect,
                        Sections.PLAssessPath,
                        Customisations.IsAssessed,
                        Applications.IncludeCertification,
                        Progress.Completed,
                        Applications.AssessAttempts AS MaxPostLearningAssessmentAttempts,
                        Applications.PLAPassThreshold AS PostLearningAssessmentPassThreshold,
                        Customisations.DiagCompletionThreshold AS DiagnosticAssessmentCompletionThreshold,
                        Customisations.TutCompletionThreshold AS TutorialsCompletionThreshold,
                        NextTutorial.NextTutorialId,
                        NextSection.NextSectionId,
                        CAST (CASE WHEN EXISTS(SELECT 1
                                                 FROM CourseTutorials
                                                WHERE SectionID <> @sectionId)
                                        THEN 1
                                   ELSE 0
                              END AS BIT) AS OtherSectionsExist,
                        CAST (CASE WHEN (Customisations.IsAssessed = 1 AND Sections.PLAssessPath IS NOT NULL)
                                        OR Sections.ConsolidationPath IS NOT NULL
                                        THEN 1
                                   WHEN EXISTS(SELECT 1
                                                 FROM CourseTutorials
                                                WHERE SectionID = @sectionId
                                                      AND Status = 1
                                              )
                                        THEN 1
                                   ELSE 0
                              END AS BIT) AS OtherItemsInSectionExist,
                         Customisations.Password,
                         Progress.PasswordSubmitted,
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
                        LEFT JOIN NextSection
                            ON Sections.SectionID = NextSection.CurrentSectionID
                        LEFT JOIN NextTutorial
                            ON Sections.SectionID = NextTutorial.CurrentSectionID
                    WHERE
                        Customisations.CustomisationID = @customisationId
                        AND Sections.SectionID = @sectionId
                        AND Sections.ArchivedDate IS NULL
                        AND Sections.DiagAssessPath IS NOT NULL
                        AND CustomisationTutorials.DiagStatus = 1
                        AND Tutorials.DiagAssessOutOf > 0
                        AND Tutorials.ArchivedDate IS NULL
                        AND Applications.DefaultContentTypeID <> 4
                    ORDER BY
                        Tutorials.OrderByNumber,
                        id;",
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
                            (int)diagnostic.DiagnosticAttempts
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
                        AND Applications.DefaultContentTypeID <> 4
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
