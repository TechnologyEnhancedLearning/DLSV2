namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using System;
    using Dapper;
    using Microsoft.Data.SqlClient;

    public class LearningLogItemsTestHelper
    {
        private readonly SqlConnection connection;

        public LearningLogItemsTestHelper(SqlConnection connection)
        {
            this.connection = connection;
        }

        public LearningLogItemDetails? SelectLearningLogItemWithResourceLink(string resourceLink)
        {
            return connection.QuerySingleOrDefault<LearningLogItemDetails>(
                @"SELECT
                        LoggedDate AS AddedDate,
                        LoggedByID AS DelegateId,
                        Activity AS ResourceName,
                        ExternalUri AS ResourceLink,
                        LinkedCompetencyLearningResourceID AS CompetencyLearningResourceId
                    FROM LearningLogItems
                    WHERE ExternalUri = @resourceLink",
                new { resourceLink }
            );
        }

        public CandidateAssessmentLearningLogItem? SelectCandidateAssessmentLearningLogItem()
        {
            return connection.QuerySingleOrDefault<CandidateAssessmentLearningLogItem>(
                @"SELECT
                        ID,
                        CandidateAssessmentID,
                        LearningLogItemID
                    FROM CandidateAssessmentLearningLogItems"
            );
        }

        public LearningLogItemCompetency? SelectLearningLogItemCompetency()
        {
            return connection.QuerySingleOrDefault<LearningLogItemCompetency>(
                @"SELECT
                        ID,
                        LearningLogItemID,
                        CompetencyID,
                        AssociatedDate
                    FROM LearningLogItemCompetencies"
            );
        }

        public class LearningLogItemDetails
        {
            public int CompetencyLearningResourceId { get; set; }
            public int DelegateId { get; set; }
            public string ResourceName { get; set; }
            public string ResourceLink { get; set; }
            public DateTime AddedDate { get; set; }
        }

        public class CandidateAssessmentLearningLogItem
        {
            public int Id { get; set; }
            public int CandidateAssessmentId { get; set; }
            public int LearningLogItemId { get; set; }
        }

        public class LearningLogItemCompetency
        {
            public int Id { get; set; }
            public int LearningLogItemId { get; set; }
            public int CompetencyId { get; set; }
            public DateTime AssociatedDate { get; set; }
        }
    }
}
