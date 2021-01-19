namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.TutorialContent;

    public interface ITutorialContentService
    {
        TutorialInformation? GetTutorialInformation(
            int candidateId,
            int customisationId,
            int sectionId,
            int tutorialId
        );
        TutorialContent? GetTutorialContent(int customisationId, int sectionId, int tutorialId);
        TutorialVideo? GetTutorialVideo(int customisationId, int sectionId, int tutorialId);
    }

    public class TutorialContentService : ITutorialContentService
    {
        private readonly IDbConnection connection;

        public TutorialContentService(IDbConnection connection)
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
                // or null if the last in the section.

                // Similar for sections, using Sections.SectionNumber

                @"  WITH OtherTutorials AS (
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
                         CROSS JOIN OtherTutorials

                   WHERE CurrentSection.SectionID = @sectionId
                         AND OtherTutorials.SectionID <> @sectionId
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
                                                 WHERE SectionID = @sectionId AND Status = 1)
                                         THEN 1
                                    ELSE 0
                               END AS BIT) AS OtherItemsInSectionExist
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
            new { candidateId, customisationId, sectionId, tutorialId });
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
                new { customisationId, sectionId, tutorialId });
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
                    new { customisationId, sectionId, tutorialId });
            }
            catch (DataException e)
            {
                if (e.InnerException is VideoNotFoundException)
                {
                    return null;
                }
                else throw;
            }
        }
    }
}
