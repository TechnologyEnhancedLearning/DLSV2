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

        public void UpdatePostLearningAssessmentPath(int sectionId, string? newPath)
        {
            connection.Execute(
                @"UPDATE Sections
                     SET PLAssessPath = @newPath

                   WHERE SectionID = @sectionId;",
                new { sectionId, newPath }
            );
        }

        public void UpdateDiagnosticAssessmentPath(int sectionId, string? newPath)
        {
            connection.Execute(
                @"UPDATE Sections
                     SET DiagAssessPath = @newPath

                   WHERE SectionID = @sectionId;",
                new { sectionId, newPath }
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
