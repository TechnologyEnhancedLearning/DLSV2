namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models;
    using Microsoft.Data.SqlClient;

    public class SectionContentTestHelper
    {
        private SqlConnection connection;

        public SectionContentTestHelper(SqlConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<OldTutorial> TutorialsFromOldStoredProcedure(int progressId, int sectionId)
        {
            return connection.Query<OldTutorial>("uspReturnProgressDetail_V3", new { progressId, sectionId }, commandType: CommandType.StoredProcedure);
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
    }
}
