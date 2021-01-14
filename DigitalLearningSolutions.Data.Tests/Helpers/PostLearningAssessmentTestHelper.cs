namespace DigitalLearningSolutions.Data.Tests.Helpers
{
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

        public OldPostLearningAssessmentScores? ScoresFromOldStoredProcedure(int progressId, int sectionId)
        {
            return connection.Query<OldPostLearningAssessmentScores>(
                "uspReturnSectionsForCandCust_V2",
                new { progressId },
                commandType: CommandType.StoredProcedure
            ).FirstOrDefault(assessment => assessment.SectionID == sectionId);
        }

        public static PostLearningContent CreateDefaultPostLearningContent(
            string applicationName = "application name",
            string customisationName = "customisation name",
            string sectionName = "section name",
            string postLearningAssessmentPath = "https://www.dls.nhs.uk/CMS/CMSContent/Course125/PLAssess/MC077_CL_MedChart_(P)_Prescriber_ASSESSMENT/imsmanifest.xml",
            int postLearningPassThreshold = 50,
            int currentVersion = 1
        )
        {
            return new PostLearningContent(
                applicationName,
                customisationName,
                sectionName,
                postLearningAssessmentPath,
                postLearningPassThreshold,
                currentVersion
            );
        }
    }
}
