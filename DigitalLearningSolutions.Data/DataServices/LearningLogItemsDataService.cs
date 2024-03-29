﻿namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.LearningResources;
    using Microsoft.Extensions.Logging;

    public interface ILearningLogItemsDataService
    {
        IEnumerable<LearningLogItem> GetLearningLogItems(int delegateUserId);

        LearningLogItem? GetLearningLogItem(int learningLogItemId);

        int InsertLearningLogItem(
            int delegateId,
            DateTime addedDate,
            string activityName,
            string resourceLink,
            int learningResourceReferenceId
        );

        void InsertCandidateAssessmentLearningLogItem(int candidateAssessmentId, int learningLogId);

        void InsertLearningLogItemCompetencies(int learningLogId, int competencyId, DateTime associatedDate);

        void UpdateLearningLogItemLastAccessedDate(int id, DateTime lastAccessedDate);

        public void SetCompletionDate(int learningLogItemId, DateTime? completedDate);

        public void SetCompleteByDate(int learningLogItemId, DateTime? completeByDate);

        void RemoveLearningLogItem(int learningLogId, int removedById, DateTime removedDate);

        int MarkLearningLogItemsCompleteByProgressId(int progressId);
    }

    public class LearningLogItemsDataService : ILearningLogItemsDataService
    {
        private const string LearningHubResourceActivityLabel = "Learning Hub Resource";

        private const string LearningLogItemColumns =
            @"          LearningLogItemID,
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
                        LearningResourceReferenceID,
                        lrr.ResourceRefID AS LearningHubResourceReferenceID";

        private readonly IDbConnection connection;

        private readonly ILogger<LearningLogItemsDataService> logger;

        public LearningLogItemsDataService(IDbConnection connection, ILogger<LearningLogItemsDataService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public IEnumerable<LearningLogItem> GetLearningLogItems(int delegateUserId)
        {
            return connection.Query<LearningLogItem>(
                $@"SELECT
                        {LearningLogItemColumns}
                    FROM LearningLogItems l
                    INNER JOIN ActivityTypes a ON a.ID = l.ActivityTypeID
                    INNER JOIN LearningResourceReferences AS lrr ON lrr.ID = l.LearningResourceReferenceID
                    WHERE LoggedById = @delegateUserId
                    AND a.TypeLabel = '{LearningHubResourceActivityLabel}'",
                new { delegateUserId }
            );
        }

        public LearningLogItem? GetLearningLogItem(int learningLogItemId)
        {
            return connection.QuerySingleOrDefault<LearningLogItem>(
                $@"SELECT
                        {LearningLogItemColumns}
                    FROM LearningLogItems l
                    INNER JOIN ActivityTypes a ON a.ID = l.ActivityTypeID
                    INNER JOIN LearningResourceReferences AS lrr ON lrr.ID = l.LearningResourceReferenceID
                    WHERE a.TypeLabel = '{LearningHubResourceActivityLabel}'
                    AND LearningLogItemID = @learningLogItemId",
                new { learningLogItemId }
            );
        }

        public int InsertLearningLogItem(
            int delegateUserId,
            DateTime addedDate,
            string activityName,
            string resourceLink,
            int learningResourceReferenceId
        )
        {
            var learningLogItemId = connection.QuerySingle<int>(
                @"INSERT INTO LearningLogItems(
                        LoggedDate,
                        LoggedByID,
                        Activity,
                        ExternalUri,
                        LearningResourceReferenceID,
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
                        @delegateUserId,
                        @activityName,
                        @resourceLink,
                        @learningResourceReferenceId,
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
                new { addedDate, delegateUserId, activityName, resourceLink, learningResourceReferenceId }
            );

            return learningLogItemId;
        }

        public void InsertCandidateAssessmentLearningLogItem(int candidateAssessmentId, int learningLogId)
        {
            connection.Execute(
                @"INSERT INTO CandidateAssessmentLearningLogItems
                    (CandidateAssessmentID, LearningLogItemID)
                    VALUES (@candidateAssessmentId, @learningLogId)",
                new { candidateAssessmentId, learningLogId }
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

        public void UpdateLearningLogItemLastAccessedDate(int id, DateTime lastAccessedDate)
        {
            connection.Execute(
                @"UPDATE LearningLogItems
                        SET LastAccessedDate = @lastAccessedDate
                    WHERE LearningLogItemID = @id",
                new { id, lastAccessedDate }
            );
        }

        public void SetCompletionDate(int learningLogItemId, DateTime? completedDate)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE LearningLogItems
                        SET CompletedDate = @completedDate
                        WHERE LearningLogItemID = @learningLogItemId",
                new { learningLogItemId, completedDate }
            );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not setting current learning log completed date as db update failed. " +
                    $"Learning log item id: {learningLogItemId}, completed date: {completedDate}"
                );
            }
        }

        public void SetCompleteByDate(int learningLogItemId, DateTime? completeByDate)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE LearningLogItems
                        SET DueDate = @completeByDate
                        WHERE LearningLogItemID = @learningLogItemId",
                new { learningLogItemId, completeByDate }
            );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not setting current learning log item complete by date as db update failed. " +
                    $"Learning log item id: {learningLogItemId}, complete by date: {completeByDate}"
                );
            }
        }

        public void RemoveLearningLogItem(int learningLogId, int removedById, DateTime removedDate)
        {
            connection.Execute(
                @"UPDATE LearningLogItems SET
                        ArchivedDate = @removedDate,
                        ArchivedById = @removedById
                    WHERE LearningLogItemId = @learningLogId",
                new { learningLogId, removedById, removedDate }
            );
        }

        public int MarkLearningLogItemsCompleteByProgressId(int progressId)
        {
            return connection.Execute(
                "UpdateLearningLogItemsMarkCompleteForRelatedCourseCompletion",
                new { progressId },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
