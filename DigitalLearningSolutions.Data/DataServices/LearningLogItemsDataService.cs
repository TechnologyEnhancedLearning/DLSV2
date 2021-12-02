namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.LearningResources;

    public interface ILearningLogItemsDataService
    {
        IEnumerable<LearningLogItem> GetLearningLogItems(int delegateId);

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
        private const string LearningHubResourceActivityLabel = "Learning Hub Resource";
        private readonly IDbConnection connection;

        public LearningLogItemsDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<LearningLogItem> GetLearningLogItems(int delegateId)
        {
            return connection.Query<LearningLogItem>(
                $@"SELECT
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
                        LinkedCompetencyLearningResourceID,
                        clr.LHResourceReferenceID AS LearningHubResourceReferenceID
                    FROM LearningLogItems l
                    INNER JOIN ActivityTypes a ON a.ID = l.ActivityTypeID
					INNER JOIN CompetencyLearningResources AS clr ON clr.ID = l.LinkedCompetencyLearningResourceID
                    WHERE LoggedById = @delegateId
					AND a.TypeLabel = '{LearningHubResourceActivityLabel}'",
                new { delegateId }
            );
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
