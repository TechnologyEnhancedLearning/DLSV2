namespace DigitalLearningSolutions.Data.Tests.TestHelpers
{
    using System;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using Microsoft.Data.SqlClient;

    public class LearningLogItemsTestHelper
    {
        private readonly SqlConnection connection;

        public LearningLogItemsTestHelper(SqlConnection connection)
        {
            this.connection = connection;
        }

        public LearningLogItem? SelectLearningLogItemWithResourceLink(string resourceLink)
        {
            return connection.QuerySingleOrDefault<LearningLogItem>(
                @"SELECT
                        LearningLogItemID,
                        LoggedDate,
                        LoggedByID,
                        DueDate,
                        CompletedDate,
                        DurationMins,
                        Activity,
                        Outcomes,
                        LinkedCustomisationID,
                        VerifiedByID,
                        VerifierComments,
                        ArchivedDate,
                        ArchivedByID,
                        ICSGUID,
                        LoggedByAdminID,
                        TypeLabel AS ActivityType,
                        ExternalUri,
                        SeqInt,
                        LastAccessedDate,
                        LearningResourceReferenceID
                    FROM LearningLogItems l
                    INNER JOIN ActivityTypes a ON a.ID = l.ActivityTypeID
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
