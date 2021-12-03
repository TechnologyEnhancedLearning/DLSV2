namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Data;
    using Dapper;

    public interface ILearningLogItemsDataService
    {
        int InsertLearningLogItem(
            int delegateId,
            DateTime addedDate,
            string activityName,
            string resourceLink,
            int competencyLearningResourceId
        );

        void InsertCandidateAssessmentLearningLogItem(int assessmentId, int learningLogId);

        void InsertLearningLogItemCompetencies(int learningLogId, int competencyId, DateTime associatedDate);
    }

    public class LearningLogItemsDataService : ILearningLogItemsDataService
    {
        private readonly IDbConnection connection;

        public LearningLogItemsDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public int InsertLearningLogItem(
            int delegateId,
            DateTime addedDate,
            string activityName,
            string resourceLink,
            int competencyLearningResourceId
        )
        {
            var learningLogItemId = connection.QuerySingle<int>(
                @"INSERT INTO LearningLogItems(
                        LoggedDate,
                        LoggedByID,
                        Activity,
                        ExternalUri,
                        LinkedCompetencyLearningResourceID,
                        ActivityTypeID,
                        DueDate,
                        CompletedDate,
                        DurationMins,
                        Outcomes,
                        LinkedCustomisationID,
                        VerifiedByID,
                        VerifierComments,
                        ArchivedDate,
                        ArchivedByID,
                        LoggedByAdminID,
                        SeqInt,
                        LastAccessedDate
                    )
                    OUTPUT Inserted.LearningLogItemID
                    VALUES (
                        @addedDate,
                        @delegateId,
                        @activityName,
                        @resourceLink,
                        @competencyLearningResourceId,
                        (SELECT TOP 1 ID FROM ActivityTypes WHERE TypeLabel = 'Learning Hub Resource'),
                        NULL,
                        NULL,
                        0,
                        NULL,
                        NULL,
                        NULL,
                        NULL,
                        NULL,
                        NULL,
                        NULL,
                        NULL,
                        NULL)",
                new { addedDate, delegateId, activityName, resourceLink, competencyLearningResourceId }
            );

            return learningLogItemId;
        }

        public void InsertCandidateAssessmentLearningLogItem(int assessmentId, int learningLogId)
        {
            connection.Execute(
                @"INSERT INTO CandidateAssessmentLearningLogItems
                    (CandidateAssessmentID, LearningLogItemID)
                    VALUES (@assessmentId, @learningLogId)",
                new { assessmentId, learningLogId }
            );
        }

        public void InsertLearningLogItemCompetencies(int learningLogId, int competencyId, DateTime associatedDate)
        {
            connection.Execute(
                @"INSERT INTO LearningLogItemCompetencies
                    (LearningLogItemID, CompetencyID, AssociatedDate)
                    VALUES
                    (@learningLogId, @competencyId, @associatedDate)",
                new { learningLogId, competencyId, associatedDate }
            );
        }
    }
}
