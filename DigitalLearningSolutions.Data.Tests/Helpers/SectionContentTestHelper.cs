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

        public void UpdateConsolidationPath(int sectionId, string? newPath)
        {
            connection.Execute(
                @"UPDATE Sections
                     SET ConsolidationPath = @newPath

                   WHERE SectionID = @sectionId;",
                new { sectionId, newPath }
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
    }
}
