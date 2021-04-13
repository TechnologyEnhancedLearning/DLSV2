namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using Dapper;
    using Microsoft.Data.SqlClient;

    public class TutorialContentTestHelper
    {
        private SqlConnection connection;

        public TutorialContentTestHelper(SqlConnection connection)
        {
            this.connection = connection;
        }

        public void ArchiveTutorial(int tutorialId)
        {
            connection.Execute(
                @"UPDATE Tutorials
                     SET ArchivedDate = GETDATE()

                   WHERE Tutorials.TutorialID = @tutorialId;",
                new { tutorialId }
            );
        }

        public void UpdateDiagnosticAttempts(int tutorialId, int progressId, int diagAttempts)
        {
            connection.Execute(
                @"UPDATE aspProgress
                        SET DiagAttempts = @diagAttempts
                        WHERE TutorialID = @tutorialId AND ProgressID = @progressId",
                new { tutorialId, progressId, diagAttempts }
            );
        }

        public void UpdateDiagnosticScore(int tutorialId, int progressId, int score)
        {
            connection.Execute(
                @"UPDATE aspProgress
                        SET DiagLast = @score
                        WHERE TutorialID = @tutorialId AND ProgressID = @progressId",
                new { tutorialId, progressId, score }
            );
        }

        public void UpdateDiagAssessOutOf(int tutorialId, int diagAssessOutOf)
        {
            connection.Execute(
                @"UPDATE Tutorials
                     SET DiagAssessOutOf = @diagAssessOutOf

                   WHERE Tutorials.TutorialID = @tutorialId;",
                new { tutorialId, diagAssessOutOf }
            );
        }

        public void UpdateDiagnosticStatus(int tutorialId, int customisationId, int status)
        {
            connection.Execute(
                @"UPDATE CustomisationTutorials
                        SET DiagStatus = @status
                        WHERE TutorialID = @tutorialId AND CustomisationID = @customisationId",
                new { tutorialId, customisationId, status }
            );
        }

        public void UpdateTutorialStatus(int tutorialId, int customisationId, int status)
        {
            connection.Execute(
                @"UPDATE CustomisationTutorials
                        SET Status = @status
                        WHERE TutorialID = @tutorialId AND CustomisationID = @customisationId",
                new { tutorialId, customisationId, status }
            );
        }

        public void RemoveCustomisationTutorial(int customisationId, int tutorialId)
        {
            connection.Execute(
                @"
                    DELETE CustomisationTutorials
                        WHERE CustomisationID = @customisationId
                            AND TutorialID = @tutorialID;",
                new { customisationId, tutorialId }
            );
        }
    }
}
