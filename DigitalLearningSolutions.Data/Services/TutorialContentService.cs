namespace DigitalLearningSolutions.Data.Services
{
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.TutorialContent;

    public interface ITutorialContentService
    {
        TutorialContent? GetTutorialContent(int candidateId, int customisationId, int sectionId, int tutorialId);
    }

    public class TutorialContentService : ITutorialContentService
    {
        private readonly IDbConnection connection;

        public TutorialContentService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public TutorialContent? GetTutorialContent(int candidateId, int customisationId, int sectionId, int tutorialId)
        {
            return connection.QueryFirstOrDefault<TutorialContent?>(
                @"SELECT Tutorials.TutorialID AS Id,
                         Tutorials.TutorialName AS Name,
                         TutStatus.Status,
                         aspProgress.TutTime AS TimeSpent,
                         Tutorials.AverageTutMins AS AverageTutorialDuration,
                         aspProgress.DiagLast AS CurrentScore,
                         Tutorials.DiagAssessOutOf AS PossibleScore,
                         CustomisationTutorials.DiagStatus AS CanShowDiagnosticStatus,
                         aspProgress.DiagAttempts AS AttemptCount,
                         Tutorials.Objectives,
                         Tutorials.VideoPath,
                         Tutorials.TutorialPath,
                         Tutorials.SupportingMatsPath AS SupportingMaterialPath
                    FROM Tutorials
                         INNER JOIN aspProgress
                         ON aspProgress.TutorialID = Tutorials.TutorialID

                         INNER JOIN TutStatus
                         ON aspProgress.TutStat = TutStatus.TutStatusID

                         INNER JOIN CustomisationTutorials
                         ON CustomisationTutorials.TutorialID = Tutorials.TutorialID

                         INNER JOIN Progress
                         ON aspProgress.ProgressID = Progress.ProgressID
                            AND CustomisationTutorials.CustomisationID = Progress.CustomisationID
                   WHERE Progress.CandidateID = @candidateId
                     AND CustomisationTutorials.CustomisationID = @customisationId
                     AND Tutorials.SectionId = @sectionId
                     AND Tutorials.TutorialID = @tutorialId
                     AND Progress.RemovedDate IS NULL
                     AND Progress.SystemRefreshed = 0;",
            new { candidateId, customisationId, sectionId, tutorialId });
        }
    }
}
