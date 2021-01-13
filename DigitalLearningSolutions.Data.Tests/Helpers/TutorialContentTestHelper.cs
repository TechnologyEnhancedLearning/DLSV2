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
    }
}
