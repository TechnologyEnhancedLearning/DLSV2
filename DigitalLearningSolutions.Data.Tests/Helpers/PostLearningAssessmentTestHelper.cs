namespace DigitalLearningSolutions.Data.Tests.Helpers
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.PostLearningAssessment;
    using Microsoft.Data.SqlClient;

    public class PostLearningAssessmentTestHelper
    {
        private SqlConnection connection;

        public PostLearningAssessmentTestHelper(SqlConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<OldPostLearningAssessmentScores> ScoresFromOldStoredProcedure(int progressId, int sectionId)
        {
            return connection.Query<OldPostLearningAssessmentScores>("uspReturnSectionsForCandCust_V2", new { progressId }, commandType: CommandType.StoredProcedure)
                .Where(assessment => assessment.SectionID == sectionId);
        }
    }
}
