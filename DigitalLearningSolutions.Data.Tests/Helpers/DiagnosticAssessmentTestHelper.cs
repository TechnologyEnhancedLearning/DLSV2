namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.DiagnosticAssessment;
    using DigitalLearningSolutions.Data.Models.TutorialContent;
    using Microsoft.Data.SqlClient;

    public class DiagnosticAssessmentTestHelper
    {
        private SqlConnection connection;

        public DiagnosticAssessmentTestHelper(SqlConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<OldTutorial> TutorialsFromOldStoredProcedure(int progressId, int sectionId)
        {
            return connection.Query<OldTutorial>("uspReturnProgressDetail_V3", new { progressId, sectionId }, commandType: CommandType.StoredProcedure);
        }

        public IEnumerable<OldScores> ScoresFromOldStoredProcedure(int progressId, int sectionId)
        {
            return connection.Query<OldScores>("uspReturnSectionsForCandCust_V2", new { progressId }, commandType: CommandType.StoredProcedure)
                .Where(section => section.SectionId == sectionId);
        }
    }
}
