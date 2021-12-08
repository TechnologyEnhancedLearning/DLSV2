namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Tracker;
    using DigitalLearningSolutions.Data.Models.TutorialContent;

    public interface ITutorialContentDataService
    {
        TutorialInformation? GetTutorialInformation(
            int candidateId,
            int customisationId,
            int sectionId,
            int tutorialId
        );

        TutorialContent? GetTutorialContent(int customisationId, int sectionId, int tutorialId);

        TutorialVideo? GetTutorialVideo(int customisationId, int sectionId, int tutorialId);

        IEnumerable<Tutorial> GetTutorialsBySectionId(int sectionId, int customisationId);

        IEnumerable<int> GetTutorialIdsForCourse(int customisationId);

        void UpdateOrInsertCustomisationTutorialStatuses(
            int tutorialId,
            int customisationId,
            bool diagnosticEnabled,
            bool learningEnabled
        );

        IEnumerable<Objective> GetNonArchivedObjectivesBySectionAndCustomisationId(int sectionId, int customisationId);

        IEnumerable<CcObjective> GetNonArchivedCcObjectivesBySectionAndCustomisationId(
            int sectionId,
            int customisationId,
            bool isPostLearning
        );
    }

    public class TutorialContentDataService : ITutorialContentDataService
    {
        private readonly IDbConnection connection;

        public TutorialContentDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public TutorialInformation? GetTutorialInformation(
            int candidateId,
            int customisationId,
            int sectionId,
            int tutorialId
        )
        {
            return connection.QueryFirstOrDefault<TutorialInformation>(
                // NextTutorialID is the ID of the next tutorial in the section, according to Tutorials.OrderBy
                // or null if the last in the section. Similar for NextSectionID, using SectionID and SectionNumber

                // Find these by making a list of other tutorials in the course, to then find next tutorials in
                // this section, and other sections (because a section must contain at least one tutorial).

                // A tutorial can be viewed (ie can be a NextTutorial) if it has CustomisationTutorials.Status 1.

                // Using this list of other tutorials in the course we can work out if there is another item in the
                // section (if there is an viewable tutorial, or a post learning assessment, or consolidation material),
                // and if there are other sections (valid tutorials with a different tutorial ID, or with assessments or
                // consolidation material. See the SectionContentDataService for the definition of a valid section.
                @"  WITH OtherTutorials AS (
                  SELECT Tutorials.TutorialID,
                         Tutorials.OrderByNumber,
                         CustomisationTutorials.Status,
                         Sections.SectionID,
                         Sections.SectionNumber,
                         CAST (CASE WHEN CustomisationTutorials.DiagStatus = 1 AND Sections.DiagAssessPath IS NOT NULL
                                         THEN 1
                                    ELSE 0
                               END AS BIT) AS HasDiagnostic
                    FROM Tutorials
                         INNER JOIN Customisations
                         ON Customisations.CustomisationID = @customisationId

                         INNER JOIN Sections
                         ON Tutorials.SectionID = Sections.SectionID
                            AND Sections.ArchivedDate IS NULL

                         INNER JOIN CustomisationTutorials
                         ON CustomisationTutorials.CustomisationID = @customisationId
                            AND CustomisationTutorials.TutorialID = Tutorials.TutorialID
                            AND Tutorials.TutorialID <> @tutorialId
                            AND Tutorials.ArchivedDate IS NULL
                            AND (
                                 CustomisationTutorials.Status = 1
                                 OR (CustomisationTutorials.DiagStatus = 1 AND Sections.DiagAssessPath IS NOT NULL AND Tutorials.DiagAssessOutOf > 0)
                                 OR (Customisations.IsAssessed = 1 AND Sections.PLAssessPath IS NOT NULL)
                            )
                  ),
                  NextTutorial AS (
                  SELECT TOP 1
                         CurrentTutorial.TutorialID AS CurrentTutorialID,
                         OtherTutorials.TutorialID AS NextTutorialID
                    FROM Tutorials AS CurrentTutorial
                         INNER JOIN OtherTutorials
                         ON CurrentTutorial.SectionID = OtherTutorials.SectionID

                         INNER JOIN CustomisationTutorials AS OtherCustomisationTutorials
                         ON OtherCustomisationTutorials.CustomisationID = @customisationId
                            AND OtherCustomisationTutorials.Status = 1

                   WHERE CurrentTutorial.TutorialID = @tutorialId
                         AND CurrentTutorial.OrderByNumber <= OtherTutorials.OrderByNumber
                         AND (
                              CurrentTutorial.OrderByNumber < OtherTutorials.OrderByNumber
                              OR CurrentTutorial.TutorialID < OtherTutorials.TutorialID
                         )
                   ORDER BY OtherTutorials.OrderByNumber, OtherTutorials.TutorialID
                  ),
                  NextSection AS (
                  SELECT TOP 1
                         CurrentSection.SectionID AS CurrentSectionID,
                         OtherTutorials.SectionID AS NextSectionID
                    FROM Sections AS CurrentSection
                         INNER JOIN OtherTutorials ON OtherTutorials.SectionID <> CurrentSection.SectionID

                   WHERE CurrentSection.SectionID = @sectionId
                         AND CurrentSection.SectionNumber <= OtherTutorials.SectionNumber
                         AND (
                              CurrentSection.SectionNumber < OtherTutorials.SectionNumber
                              OR CurrentSection.SectionID < OtherTutorials.SectionID
                         )

                   ORDER BY OtherTutorials.SectionNumber, OtherTutorials.SectionID
                  )
                  SELECT Tutorials.TutorialID AS Id,
                         Tutorials.TutorialName AS Name,
                         Sections.SectionName,
                         Applications.ApplicationName,
                         Applications.ApplicationInfo,
                         Customisations.CustomisationName,
                         COALESCE(TutStatus.Status, 'Not started') AS Status,
                         COALESCE(aspProgress.TutTime, 0) AS TimeSpent,
                         Tutorials.AverageTutMins AS AverageTutorialDuration,
                         COALESCE(aspProgress.DiagLast, 0) AS CurrentScore,
                         Tutorials.DiagAssessOutOf AS PossibleScore,
                         CustomisationTutorials.DiagStatus AS CanShowDiagnosticStatus,
                         COALESCE(aspProgress.DiagAttempts, 0) AS AttemptCount,
                         Tutorials.Objectives,
                         Tutorials.VideoPath,
                         Tutorials.TutorialPath,
                         Tutorials.SupportingMatsPath AS SupportingMaterialPath,
                         CASE
                              WHEN Customisations.IsAssessed = 0 THEN NULL
                              ELSE Sections.PLAssessPath
                         END AS PostLearningAssessmentPath,
                         Applications.CourseSettings,
                         Applications.IncludeCertification,
                         Customisations.IsAssessed,
                         Progress.Completed,
                         Applications.AssessAttempts AS MaxPostLearningAssessmentAttempts,
                         Applications.PLAPassThreshold AS PostLearningAssessmentPassThreshold,
                         Customisations.DiagCompletionThreshold AS DiagnosticAssessmentCompletionThreshold,
                         Customisations.TutCompletionThreshold AS TutorialsCompletionThreshold,
                         NextTutorial.NextTutorialID,
                         NextSection.NextSectionID,
                         CAST (CASE WHEN EXISTS(SELECT 1
                                                  FROM OtherTutorials
                                                 WHERE SectionID <> @sectionId)
                                         THEN 1
                                    ELSE 0
                               END AS BIT) AS OtherSectionsExist,
                         CAST (CASE WHEN (CustomisationTutorials.DiagStatus = 1 AND Sections.DiagAssessPath IS NOT NULL)
                                         OR (Customisations.IsAssessed = 1 AND Sections.PLAssessPath IS NOT NULL)
                                         OR Sections.ConsolidationPath IS NOT NULL
                                         THEN 1
                                    WHEN EXISTS(SELECT 1
                                                  FROM OtherTutorials
                                                 WHERE SectionID = @sectionId
                                                       AND (Status = 1 OR HasDiagnostic = 1)
                                               )
                                         THEN 1
                                    ELSE 0
                               END AS BIT) AS OtherItemsInSectionExist,
                        Customisations.Password,
                        Progress.PasswordSubmitted
                    FROM Tutorials
                         INNER JOIN CustomisationTutorials
                         ON CustomisationTutorials.TutorialID = Tutorials.TutorialID

                         INNER JOIN Sections
                         ON Tutorials.SectionID = Sections.SectionID

                         INNER JOIN Customisations
                         ON CustomisationTutorials.CustomisationID = Customisations.CustomisationID

                         INNER JOIN Applications
                         ON Customisations.ApplicationID = Applications.ApplicationID

                         LEFT JOIN NextTutorial
                         ON Tutorials.TutorialID = NextTutorial.CurrentTutorialID

                         LEFT JOIN NextSection AS NextSection
                         ON Sections.SectionID = NextSection.CurrentSectionID

                         LEFT JOIN Progress
                         ON CustomisationTutorials.CustomisationID = Progress.CustomisationID
                            AND Progress.CandidateID = @candidateId
                            AND Progress.RemovedDate IS NULL
                            AND Progress.SystemRefreshed = 0

                         LEFT JOIN aspProgress
                         ON aspProgress.TutorialID = Tutorials.TutorialID
                            AND aspProgress.ProgressID = Progress.ProgressID

                         LEFT JOIN TutStatus
                         ON aspProgress.TutStat = TutStatus.TutStatusID
                   WHERE CustomisationTutorials.CustomisationID = @customisationId
                     AND Tutorials.SectionId = @sectionId
                     AND Tutorials.TutorialID = @tutorialId
                     AND Customisations.Active = 1
                     AND CustomisationTutorials.Status = 1
                     AND Sections.ArchivedDate IS NULL
                     AND Tutorials.ArchivedDate IS NULL;",
                new { candidateId, customisationId, sectionId, tutorialId }
            );
        }

        public TutorialContent? GetTutorialContent(int customisationId, int sectionId, int tutorialId)
        {
            return connection.QueryFirstOrDefault<TutorialContent>(
                @"SELECT Tutorials.TutorialName,
                         Sections.SectionName,
                         Applications.ApplicationName,
                         Customisations.CustomisationName,
                         Tutorials.TutorialPath,
                         Customisations.CurrentVersion
                    FROM CustomisationTutorials
                         INNER JOIN Tutorials
                         ON CustomisationTutorials.TutorialID = Tutorials.TutorialID

                         INNER JOIN Customisations
                         ON CustomisationTutorials.CustomisationID = Customisations.CustomisationID

                         INNER JOIN Applications
                         ON Customisations.ApplicationID = Applications.ApplicationID

                         INNER JOIN Sections
                         ON Tutorials.SectionID = Sections.SectionID
                   WHERE Customisations.CustomisationID = @customisationId
                         AND Sections.SectionID = @sectionId
                         AND Tutorials.TutorialId = @tutorialId
                         AND Customisations.Active = 1
                         AND CustomisationTutorials.Status = 1
                         AND Sections.ArchivedDate IS NULL
                         AND Tutorials.ArchivedDate IS NULL;",
                new { customisationId, sectionId, tutorialId }
            );
        }

        public TutorialVideo? GetTutorialVideo(int customisationId, int sectionId, int tutorialId)
        {
            try
            {
                return connection.QueryFirstOrDefault<TutorialVideo>(
                    @"SELECT Tutorials.TutorialName,
                         Sections.SectionName,
                         Applications.ApplicationName,
                         Customisations.CustomisationName,
                         Tutorials.VideoPath
                    FROM CustomisationTutorials
                         INNER JOIN Tutorials
                         ON CustomisationTutorials.TutorialID = Tutorials.TutorialID

                         INNER JOIN Customisations
                         ON CustomisationTutorials.CustomisationID = Customisations.CustomisationID

                         INNER JOIN Applications
                         ON Customisations.ApplicationID = Applications.ApplicationID

                         INNER JOIN Sections
                         ON Tutorials.SectionID = Sections.SectionID
                   WHERE Customisations.CustomisationID = @customisationId
                         AND Sections.SectionID = @sectionId
                         AND Tutorials.TutorialId = @tutorialId
                         AND Customisations.Active = 1
                         AND Sections.ArchivedDate IS NULL
                         AND CustomisationTutorials.Status = 1
                         AND Tutorials.ArchivedDate IS NULL;",
                    new { customisationId, sectionId, tutorialId }
                );
            }
            catch (DataException e)
            {
                if (e.InnerException is VideoNotFoundException)
                {
                    return null;
                }

                throw;
            }
        }

        public IEnumerable<Tutorial> GetTutorialsBySectionId(int sectionId, int customisationId)
        {
            return connection.Query<Tutorial>(
                @"SELECT 
                        tu.TutorialID,
                        tu.TutorialName,
                        ct.[Status],
                        ct.DiagStatus
                    FROM dbo.Tutorials AS tu
                    LEFT JOIN dbo.CustomisationTutorials AS ct
                        ON ct.TutorialID = tu.TutorialID AND ct.CustomisationID = @customisationId
                    WHERE tu.SectionID = @sectionId
                    AND tu.ArchivedDate IS NULL",
                new { sectionId, customisationId }
            );
        }

        public IEnumerable<int> GetTutorialIdsForCourse(int customisationId)
        {
            return connection.Query<int>(
                @"SELECT t.TutorialID
                    FROM Customisations AS c
                    INNER JOIN Applications AS a ON c.ApplicationID = a.ApplicationID
                    INNER JOIN Sections AS s ON a.ApplicationID = s.ApplicationID
                    INNER JOIN Tutorials AS t ON s.SectionID = t.SectionID
                    WHERE (c.CustomisationID = @customisationId)",
                new { customisationId }
            );
        }

        public void UpdateOrInsertCustomisationTutorialStatuses(
            int tutorialId,
            int customisationId,
            bool diagnosticEnabled,
            bool learningEnabled
        )
        {
            connection.Execute(
                @"UPDATE CustomisationTutorials
                    SET
                        Status = @learningEnabled,
                        DiagStatus = @diagnosticEnabled
                    WHERE CustomisationID = @customisationId
                        AND TutorialID = @TutorialID

                    IF @@ROWCOUNT = 0
                    BEGIN
                        INSERT INTO CustomisationTutorials (CustomisationID, TutorialID, [Status], DiagStatus)
                        VALUES (@customisationId, @tutorialId, @learningEnabled, @diagnosticEnabled)
                    END",
                new { customisationId, tutorialId, learningEnabled, diagnosticEnabled }
            );
        }

        public IEnumerable<Objective> GetNonArchivedObjectivesBySectionAndCustomisationId(
            int sectionId,
            int customisationId
        )
        {
            return connection.Query<Objective>(
                @"SELECT 
                        CASE
                            WHEN tu.OriginalTutorialID > 0 THEN tu.OriginalTutorialID
                            ELSE tu.TutorialID
                        END AS TutorialID,
                        tu.CMIInteractionIDs AS Interactions,
                        tu.DiagAssessOutOf AS Possible
                    FROM dbo.Tutorials AS tu
                    LEFT JOIN dbo.CustomisationTutorials AS ct
                        ON ct.TutorialID = tu.TutorialID
                    WHERE tu.SectionID = @sectionId
                    AND ct.CustomisationID = @customisationId
                    AND tu.ArchivedDate IS NULL",
                new { sectionId, customisationId }
            );
        }

        public IEnumerable<CcObjective> GetNonArchivedCcObjectivesBySectionAndCustomisationId(
            int sectionId,
            int customisationId,
            bool isPostLearning
        )
        {
            return connection.Query<CcObjective>(
                @"SELECT 
                        CASE
                            WHEN tu.OriginalTutorialID > 0 THEN tu.OriginalTutorialID
                            ELSE tu.TutorialID
                        END AS TutorialID,
                        tu.TutorialName,
                        tu.DiagAssessOutOf AS Possible
                    FROM dbo.Tutorials AS tu
                    INNER JOIN dbo.CustomisationTutorials AS ct
                        ON ct.TutorialID = tu.TutorialID
                    WHERE tu.SectionID = @sectionId
                    AND ct.CustomisationID = @customisationId
                    AND tu.ArchivedDate IS NULL
                    AND (@isPostLearning = 1 OR (ct.DiagStatus = 1 AND tu.DiagAssessOutOf > 0))",
                new { sectionId, customisationId, isPostLearning }
            );
        }
    }
}
