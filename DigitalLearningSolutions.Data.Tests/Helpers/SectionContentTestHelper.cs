namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using Dapper;
    using Microsoft.Data.SqlClient;

    public class SectionContentTestHelper
    {
        private SqlConnection connection;

        public SectionContentTestHelper(SqlConnection connection)
        {
            this.connection = connection;
        }

        public void UpdateSectionNumber(int sectionId, int sectionNumber)
        {
            connection.Execute(
                @"UPDATE Sections
                        SET SectionNumber = @sectionNumber
                        WHERE SectionID = @sectionId",
                new { sectionId, sectionNumber }
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

        public void UpdateCustomisationTutorialStatuses(
            int customisationId,
            int tutorialId,
            int status,
            int diagnosticStatus
        )
        {
            connection.Execute(
                @"UPDATE CustomisationTutorials
                        SET Status = @status, DiagStatus = @diagnosticStatus
                        WHERE CustomisationID = @customisationId AND TutorialID = @tutorialId",
                new { customisationId, tutorialId, status, diagnosticStatus }
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

        public void UpdateDiagnosticStatus(int tutorialId, int customisationId, int status)
        {
            connection.Execute(
                @"UPDATE CustomisationTutorials
                        SET DiagStatus = @status
                        WHERE TutorialID = @tutorialId AND CustomisationID = @customisationId",
                new { tutorialId, customisationId, status }
            );
        }
    }
}
