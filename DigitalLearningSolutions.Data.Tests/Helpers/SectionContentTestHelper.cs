namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.SectionContent;
    using Microsoft.Data.SqlClient;

    public class SectionContentTestHelper
    {
        private SqlConnection connection;

        public SectionContentTestHelper(SqlConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<OldSectionTutorial> TutorialsFromOldStoredProcedure(int progressId, int sectionId)
        {
            return connection.Query<OldSectionTutorial>("uspReturnProgressDetail_V3", new { progressId, sectionId }, commandType: CommandType.StoredProcedure);
        }
    }
}
