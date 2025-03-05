﻿namespace DigitalLearningSolutions.Data.DataServices
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Common;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Data.Models.Frameworks.Import;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;
    using Microsoft.Extensions.Logging;
    using AssessmentQuestion = DigitalLearningSolutions.Data.Models.Frameworks.AssessmentQuestion;
    using CompetencyResourceAssessmentQuestionParameter =
        DigitalLearningSolutions.Data.Models.Frameworks.CompetencyResourceAssessmentQuestionParameter;

    public interface IFrameworkDataService
    {
        //GET DATA
        //  Frameworks:
        DashboardData? GetDashboardDataForAdminID(int adminId);

        IEnumerable<DashboardToDoItem> GetDashboardToDoItems(int adminId);

        DetailFramework? GetFrameworkDetailByFrameworkId(int frameworkId, int adminId);

        BaseFramework? GetBaseFrameworkByFrameworkId(int frameworkId, int adminId);

        BrandedFramework? GetBrandedFrameworkByFrameworkId(int frameworkId, int adminId);

        DetailFramework? GetDetailFrameworkByFrameworkId(int frameworkId, int adminId);
        IEnumerable<CompetencyFlag> GetSelectedCompetencyFlagsByCompetecyIds(int[] ids);
        IEnumerable<CompetencyFlag> GetSelectedCompetencyFlagsByCompetecyId(int competencyId);
        IEnumerable<CompetencyFlag> GetCompetencyFlagsByFrameworkId(int frameworkId, int? competencyId, bool? selected = null);
        IEnumerable<Flag> GetCustomFlagsByFrameworkId(int? frameworkId, int? flagId);

        IEnumerable<BrandedFramework> GetFrameworkByFrameworkName(string frameworkName, int adminId);

        IEnumerable<BrandedFramework> GetFrameworksForAdminId(int adminId);

        IEnumerable<BrandedFramework> GetAllFrameworks(int adminId);

        int GetAdminUserRoleForFrameworkId(int adminId, int frameworkId);

        string? GetFrameworkConfigForFrameworkId(int frameworkId);

        //  Collaborators:
        IEnumerable<CollaboratorDetail> GetCollaboratorsForFrameworkId(int frameworkId);

        CollaboratorNotification? GetCollaboratorNotification(int id, int invitedByAdminId);

        //  Competencies/groups:
        IEnumerable<FrameworkCompetencyGroup> GetFrameworkCompetencyGroups(int frameworkId);

        IEnumerable<FrameworkCompetency> GetFrameworkCompetenciesUngrouped(int frameworkId);

        CompetencyGroupBase? GetCompetencyGroupBaseById(int Id);

        FrameworkCompetency? GetFrameworkCompetencyById(int Id);

        int GetMaxFrameworkCompetencyID();

        int GetMaxFrameworkCompetencyGroupID();

        //  Assessment questions:
        IEnumerable<AssessmentQuestion> GetAllCompetencyQuestions(int adminId);

        IEnumerable<AssessmentQuestion> GetFrameworkDefaultQuestionsById(int frameworkId, int adminId);

        IEnumerable<AssessmentQuestion> GetCompetencyAssessmentQuestionsByFrameworkCompetencyId(
            int frameworkCompetencyId,
            int adminId
        );

        IEnumerable<AssessmentQuestion> GetCompetencyAssessmentQuestionsById(int competencyId, int adminId);

        IEnumerable<GenericSelectList> GetAssessmentQuestionInputTypes();

        IEnumerable<GenericSelectList> GetAssessmentQuestions(int frameworkId, int adminId);

        FrameworkDefaultQuestionUsage GetFrameworkDefaultQuestionUsage(int frameworkId, int assessmentQuestionId);

        IEnumerable<GenericSelectList> GetAssessmentQuestionsForCompetency(int frameworkCompetencyId, int adminId);

        AssessmentQuestionDetail GetAssessmentQuestionDetailById(int assessmentQuestionId, int adminId);

        LevelDescriptor GetLevelDescriptorForAssessmentQuestionId(int assessmentQuestionId, int adminId, int level);

        IEnumerable<CompetencyResourceAssessmentQuestionParameter>
            GetSignpostingResourceParametersByFrameworkAndCompetencyId(int frameworkId, int competencyId);

        IEnumerable<LevelDescriptor> GetLevelDescriptorsForAssessmentQuestionId(
            int assessmentQuestionId,
            int adminId,
            int minValue,
            int maxValue,
            bool zeroBased
        );

        Competency? GetFrameworkCompetencyForPreview(int frameworkCompetencyId);
        IEnumerable<BulkCompetency> GetBulkCompetenciesForFramework(int frameworkId);
        List<int> GetFrameworkCompetencyOrder(int frameworkId, List<int> frameworkCompetencyIds);

        //  Comments:
        IEnumerable<CommentReplies> GetCommentsForFrameworkId(int frameworkId, int adminId);

        CommentReplies? GetCommentRepliesById(int commentId, int adminId);

        Comment? GetCommentById(int adminId, int commentId);

        List<Recipient> GetCommentRecipients(int frameworkId, int adminId, int? replyToCommentId);

        // Reviews:
        IEnumerable<CollaboratorDetail> GetReviewersForFrameworkId(int frameworkId);

        IEnumerable<FrameworkReview> GetFrameworkReviewsForFrameworkId(int frameworkId);

        FrameworkReview? GetFrameworkReview(int frameworkId, int adminId, int reviewId);

        FrameworkReviewOutcomeNotification? GetFrameworkReviewNotification(int reviewId);

        //INSERT DATA
        BrandedFramework CreateFramework(DetailFramework detailFramework, int adminId);

        int InsertCompetencyGroup(string groupName, string? groupDescription, int adminId, int? frameworkId);

        int InsertFrameworkCompetencyGroup(int groupId, int frameworkID, int adminId);

        IEnumerable<FrameworkCompetency> GetAllCompetenciesForAdminId(string name, int adminId);

        int InsertCompetency(string name, string? description, int adminId);

        int InsertFrameworkCompetency(int competencyId, int? frameworkCompetencyGroupID, int adminId, int frameworkId, bool alwaysShowDescription = false);

        int AddCollaboratorToFramework(int frameworkId, string userEmail, bool canModify, int? centreID);

        int AddCustomFlagToFramework(int frameworkId, string flagName, string flagGroup, string flagTagClass);
        void UpdateFrameworkCustomFlag(int frameworkId, int id, string flagName, string flagGroup, string flagTagClass);

        void AddFrameworkDefaultQuestion(int frameworkId, int assessmentQuestionId, int adminId, bool addToExisting);

        CompetencyResourceAssessmentQuestionParameter?
            GetCompetencyResourceAssessmentQuestionParameterByCompetencyLearningResourceId(
                int competencyResourceAssessmentQuestionParameterId
            );

        LearningResourceReference? GetLearningResourceReferenceByCompetencyLearningResouceId(
            int competencyLearningResourceID
        );

        int EditCompetencyResourceAssessmentQuestionParameter(CompetencyResourceAssessmentQuestionParameter parameter);

        void AddCompetencyAssessmentQuestion(int frameworkCompetencyId, int assessmentQuestionId, int adminId);

        int InsertAssessmentQuestion(
            string question,
            int assessmentQuestionInputTypeId,
            string? maxValueDescription,
            string? minValueDescription,
            string? scoringInstructions,
            int minValue,
            int maxValue,
            bool includeComments,
            int adminId,
            string? commentsPrompt,
            string? commentsHint
        );

        int GetCompetencyAssessmentQuestionRoleRequirementsCount(int assessmentQuestionId, int competencyId);

        void InsertLevelDescriptor(
            int assessmentQuestionId,
            int levelValue,
            string levelLabel,
            string? levelDescription,
            int adminId
        );

        int InsertComment(int frameworkId, int adminId, string comment, int? replyToCommentId);

        void InsertFrameworkReview(int frameworkId, int frameworkCollaboratorId, bool required);

        int InsertFrameworkReReview(int reviewId);

        //UPDATE DATA
        BrandedFramework? UpdateFrameworkBranding(
            int frameworkId,
            int brandId,
            int categoryId,
            int topicId,
            int adminId
        );

        bool UpdateFrameworkName(int frameworkId, int adminId, string frameworkName);

        void UpdateFrameworkDescription(int frameworkId, int adminId, string? frameworkDescription);

        void UpdateFrameworkConfig(int frameworkId, int adminId, string? frameworkConfig);

        void UpdateFrameworkCompetencyGroup(
            int frameworkCompetencyGroupId,
            int competencyGroupId,
            string name,
            string? description,
            int adminId
        );

        void UpdateFrameworkCompetency(int frameworkCompetencyId, string name, string? description, int adminId, bool? alwaysShowDescription);
        int UpdateCompetencyFlags(int frameworkId, int competencyId, int[] selectedFlagIds);

        void MoveFrameworkCompetencyGroup(int frameworkCompetencyGroupId, bool singleStep, string direction);

        void MoveFrameworkCompetency(int frameworkCompetencyId, bool singleStep, string direction);

        void UpdateAssessmentQuestion(
            int id,
            string question,
            int assessmentQuestionInputTypeId,
            string? maxValueDescription,
            string? minValueDescription,
            string? scoringInstructions,
            int minValue,
            int maxValue,
            bool includeComments,
            int adminId,
            string? commentsPrompt,
            string? commentsHint
        );

        void UpdateLevelDescriptor(int id, int levelValue, string levelLabel, string? levelDescription, int adminId);

        void ArchiveComment(int commentId);

        void UpdateFrameworkStatus(int frameworkId, int statusId, int adminId);

        void SubmitFrameworkReview(int frameworkId, int reviewId, bool signedOff, int? commentId);

        void UpdateReviewRequestedDate(int reviewId);

        void ArchiveReviewRequest(int reviewId);

        void MoveCompetencyAssessmentQuestion(
            int competencyId,
            int assessmentQuestionId,
            bool singleStep,
            string direction
        );

        //Delete data
        void RemoveCustomFlag(int flagId);
        void RemoveCollaboratorFromFramework(int frameworkId, int id);

        void DeleteFrameworkCompetencyGroup(int frameworkCompetencyGroupId, int competencyGroupId, int adminId);

        void DeleteFrameworkCompetency(int frameworkCompetencyId, int adminId);

        void DeleteFrameworkDefaultQuestion(
            int frameworkId,
            int assessmentQuestionId,
            int adminId,
            bool deleteFromExisting
        );

        void DeleteCompetencyAssessmentQuestion(int frameworkCompetencyId, int assessmentQuestionId, int adminId);

        void DeleteCompetencyLearningResource(int competencyLearningResourceId, int adminId);
    }

    public class FrameworkDataService : IFrameworkDataService
    {
        private const string BaseFrameworkFields =
            @"FW.ID,
            FrameworkName,
            OwnerAdminID,
            (SELECT Forename + ' ' + Surname + (CASE WHEN Active = 1 THEN '' ELSE ' (Inactive)' END) AS Expr1 FROM AdminUsers WHERE (AdminID = FW.OwnerAdminID)) AS Owner,
            BrandID,
            CategoryID,
            TopicID,
            CreatedDate,
            PublishStatusID,
            UpdatedByAdminID,
            (SELECT Forename + ' ' + Surname + (CASE WHEN Active = 1 THEN '' ELSE ' (Inactive)' END) AS Expr1 FROM AdminUsers AS AdminUsers_1 WHERE (AdminID = FW.UpdatedByAdminID)) AS UpdatedBy,
            CASE WHEN FW.OwnerAdminID = @adminId THEN 3 WHEN fwc.CanModify = 1 THEN 2 WHEN fwc.CanModify = 0 THEN 1 ELSE 0 END AS UserRole,
            fwr.ID AS FrameworkReviewID";

        private const string BrandedFrameworkFields =
            @",(SELECT BrandName
                                 FROM    Brands
                                 WHERE (BrandID = FW.BrandID)) AS Brand,
                                 (SELECT CategoryName
                                 FROM    CourseCategories
                                 WHERE (CourseCategoryID = FW.CategoryID)) AS Category,
                                 (SELECT CourseTopic
                 FROM    CourseTopics
                                 WHERE (CourseTopicID = FW.TopicID)) AS Topic";

        private const string DetailFrameworkFields =
            @",FW.Description
              ,FW.FrameworkConfig";

        private const string FlagFields = @"fl.ID AS FlagId, fl.FrameworkId, fl.FlagName, fl.FlagGroup, fl.FlagTagClass";

        private const string FrameworkTables =
            @"Frameworks AS FW LEFT OUTER JOIN
             FrameworkCollaborators AS fwc ON fwc.FrameworkID = FW.ID AND fwc.AdminID = @adminId AND COALESCE(IsDeleted, 0) = 0
            LEFT OUTER JOIN FrameworkReviews AS fwr ON fwc.ID = fwr.FrameworkCollaboratorID AND fwr.Archived IS NULL AND fwr.ReviewComplete IS NULL";

        private const string AssessmentQuestionFields =
            @"SELECT AQ.ID, AQ.Question, AQ.MinValue, AQ.MaxValue, AQ.AssessmentQuestionInputTypeID, AQI.InputTypeName, AQ.AddedByAdminId, CASE WHEN AQ.AddedByAdminId = @adminId THEN 1 ELSE 0 END AS UserIsOwner, AQ.CommentsPrompt, AQ.CommentsHint";

        private const string AssessmentQuestionDetailFields =
            @", AQ.MinValueDescription, AQ.MaxValueDescription, AQ.IncludeComments, AQ.ScoringInstructions ";

        private const string AssessmentQuestionTables =
            @"FROM AssessmentQuestions AS AQ LEFT OUTER JOIN AssessmentQuestionInputTypes AS AQI ON AQ.AssessmentQuestionInputTypeID = AQI.ID ";

        private const string FrameworksCommentColumns = @"ID,
                        ReplyToFrameworkCommentID,
                        AdminID,
                        (SELECT Forename + ' ' + Surname + (CASE WHEN Active = 1 THEN '' ELSE ' (Inactive)' END) FROM AdminUsers WHERE AdminID = FrameworkComments.AdminId) AS Commenter,
                        CAST(CASE WHEN AdminID = @adminId THEN 1 ELSE 0 END AS Bit) AS UserIsCommenter,
                        AddedDate,
                        Comments,
                        LastEdit";

        private readonly IDbConnection connection;
        private readonly ILogger<FrameworkDataService> logger;

        public FrameworkDataService(IDbConnection connection, ILogger<FrameworkDataService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public DetailFramework? GetFrameworkDetailByFrameworkId(int frameworkId, int adminId)
        {
            return connection.QueryFirstOrDefault<DetailFramework>(
                $@"SELECT {BaseFrameworkFields} {BrandedFrameworkFields} {DetailFrameworkFields}
                      FROM {FrameworkTables}
                      WHERE FW.ID = @frameworkId",
                new { frameworkId, adminId }
            );
        }

        public BaseFramework GetBaseFrameworkByFrameworkId(int frameworkId, int adminId)
        {
            return connection.QueryFirstOrDefault<BaseFramework>(
                $@"SELECT {BaseFrameworkFields}
                      FROM {FrameworkTables}
                      WHERE FW.ID = @frameworkId",
                new { frameworkId, adminId }
            );
        }

        public BrandedFramework GetBrandedFrameworkByFrameworkId(int frameworkId, int adminId)
        {
            return connection.QueryFirstOrDefault<BrandedFramework>(
                $@"SELECT {BaseFrameworkFields} {BrandedFrameworkFields}
                      FROM {FrameworkTables}
                      WHERE FW.ID = @frameworkId",
                new { frameworkId, adminId }
            );
        }

        public DetailFramework? GetDetailFrameworkByFrameworkId(int frameworkId, int adminId)
        {
            return connection.QueryFirstOrDefault<DetailFramework>(
                $@"SELECT {BaseFrameworkFields} {BrandedFrameworkFields} {DetailFrameworkFields}
                      FROM {FrameworkTables}
                      WHERE FW.ID = @frameworkId",
                new { frameworkId, adminId }
            );
        }

        public IEnumerable<CompetencyFlag> GetCompetencyFlagsByFrameworkId(int frameworkId, int? competencyId = null, bool? selected = null)
        {
            var competencyIdFilter = competencyId.HasValue ? "cf.CompetencyId = @competencyId" : "1=1";
            var selectedFilter = selected.HasValue ? $"cf.Selected = {(selected.Value ? 1 : 0)}" : "1=1";
            return connection.Query<CompetencyFlag>(
                $@"SELECT CompetencyId, Selected, {FlagFields}
	                FROM Flags fl
	                INNER JOIN Frameworks AS fw ON fl.FrameworkID = fw.ID
	                LEFT OUTER JOIN CompetencyFlags AS cf ON cf.FlagID = fl.ID AND {competencyIdFilter}
                    WHERE fl.FrameworkId = @frameworkId AND {selectedFilter}",
                new { competencyId, frameworkId }
            );
        }

        public IEnumerable<CompetencyFlag> GetSelectedCompetencyFlagsByCompetecyIds(int[] ids)
        {
            var commaSeparatedIds = String.Join(',', ids.Distinct());
            var competencyIdFilter = ids.Count() > 0 ? $"cf.CompetencyID IN ({commaSeparatedIds})" : "1=1";
            return connection.Query<CompetencyFlag>(
                $@"SELECT CompetencyId, Selected, {FlagFields}
	                FROM CompetencyFlags AS cf
	                INNER JOIN Flags AS fl ON cf.FlagID = fl.ID
                    WHERE cf.Selected = 1 AND {competencyIdFilter}"
            );
        }

        public IEnumerable<CompetencyFlag> GetSelectedCompetencyFlagsByCompetecyId(int competencyId)
        {
            return GetSelectedCompetencyFlagsByCompetecyIds(new int[1] { competencyId });
        }

        public IEnumerable<Flag> GetCustomFlagsByFrameworkId(int? frameworkId, int? flagId = null)
        {
            var flagIdFilter = flagId.HasValue ? "fl.ID = @flagId" : "1=1";
            var frameworkFilter = frameworkId.HasValue ? "FrameworkId = @frameworkId" : "1=1";
            return connection.Query<Flag>(
                $@"SELECT {FlagFields}
	                FROM Flags fl
                    WHERE {frameworkFilter} AND {flagIdFilter}",
                new { frameworkId, flagId });
        }

        public IEnumerable<BrandedFramework> GetFrameworkByFrameworkName(string frameworkName, int adminId)
        {
            return connection.Query<BrandedFramework>(
                $@"SELECT {BaseFrameworkFields} {BrandedFrameworkFields} {DetailFrameworkFields}
                      FROM {FrameworkTables}
                      WHERE FW.FrameworkName = @frameworkName",
                new { adminId, frameworkName }
            );
        }

        public IEnumerable<BrandedFramework> GetFrameworksForAdminId(int adminId)
        {
            return connection.Query<BrandedFramework>(
                $@"SELECT {BaseFrameworkFields} {BrandedFrameworkFields}
                      FROM {FrameworkTables}
                      WHERE (OwnerAdminID = @adminId) OR
             (@adminId IN
                 (SELECT AdminID
                 FROM    FrameworkCollaborators
                 WHERE (FrameworkID = FW.ID) AND (IsDeleted=0) ))",
                new { adminId }
            );
        }

        public IEnumerable<BrandedFramework> GetAllFrameworks(int adminId)
        {
            return connection.Query<BrandedFramework>(
                $@"SELECT {BaseFrameworkFields} {BrandedFrameworkFields}
                      FROM {FrameworkTables}",
                new { adminId }
            );
        }

        public IEnumerable<FrameworkCompetency> GetAllCompetenciesForAdminId(string name, int adminId)
        {
            var adminFilter = adminId > 0 ? "AND c.UpdatedByAdminID = @adminId" : string.Empty;
            return connection.Query<FrameworkCompetency>(
                $@"SELECT f.FrameworkName,c.Description,c.UpdatedByAdminID,c.Name from Competencies as c
                    INNER JOIN FrameworkCompetencies AS fc ON c.ID = fc.CompetencyID
                    INNER JOIN Frameworks AS f ON fc.FrameworkID = f.ID
                    WHERE fc.FrameworkID = f.ID AND c.Name = @name {adminFilter}",
                new { name, adminId }
            );
        }

        public BrandedFramework CreateFramework(DetailFramework detailFramework, int adminId)
        {
            string frameworkName = detailFramework.FrameworkName;
            var description = detailFramework.Description;
            var frameworkConfig = detailFramework.FrameworkConfig;
            var brandId = detailFramework.BrandID;
            var categoryId = detailFramework.CategoryID;
            int? topicId = detailFramework.TopicID;
            if ((detailFramework.FrameworkName.Length == 0) | (adminId < 1))
            {
                logger.LogWarning(
                    $"Not inserting framework as it failed server side validation. AdminId: {adminId}, frameworkName: {frameworkName}"
                );
                return new BrandedFramework();
            }

            var existingFrameworks = connection.QuerySingle<int>(
                @"SELECT COUNT(*) FROM Frameworks WHERE FrameworkName = @frameworkName",
                new { frameworkName }
            );
            if (existingFrameworks > 0)
            {
                return new BrandedFramework();
            }

            var numberOfAffectedRows = connection.Execute(
                @"INSERT INTO Frameworks (
                    BrandID
                    ,CategoryID
                    ,TopicID
                    ,FrameworkName
                    ,Description
                    ,FrameworkConfig
                    ,OwnerAdminID
                    ,PublishStatusID
                    ,UpdatedByAdminID)
                    VALUES (@brandId, @categoryId, @topicId, @frameworkName, @description, @frameworkConfig, @adminId, 1, @adminId)",
                new { brandId, categoryId, topicId, frameworkName, description, frameworkConfig, adminId }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not inserting framework as db insert failed. " +
                    $"FrameworkName: {frameworkName}, admin id: {adminId}"
                );
                return new BrandedFramework();
            }

            return connection.QueryFirstOrDefault<BrandedFramework>(
                $@"SELECT {BaseFrameworkFields}
                      FROM {FrameworkTables}
                      WHERE FrameworkName = @frameworkName AND OwnerAdminID = @adminId",
                new { frameworkName, adminId }
            );
        }

        public BrandedFramework? UpdateFrameworkBranding(
            int frameworkId,
            int brandId,
            int categoryId,
            int topicId,
            int adminId
        )
        {
            if ((frameworkId < 1) | (brandId < 1) | (categoryId < 1) | (topicId < 1) | (adminId < 1))
            {
                logger.LogWarning(
                    $"Not updating framework as it failed server side validation. frameworkId: {frameworkId}, brandId: {brandId}, categoryId: {categoryId}, topicId: {topicId},  AdminId: {adminId}"
                );
                return null;
            }

            var numberOfAffectedRows = connection.Execute(
                @"UPDATE Frameworks SET BrandID = @brandId, CategoryID = @categoryId, TopicID = @topicId, UpdatedByAdminID = @adminId
                    WHERE ID = @frameworkId",
                new { brandId, categoryId, topicId, adminId, frameworkId }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating framework as db update failed. " +
                    $"frameworkId: {frameworkId}, brandId: {brandId}, categoryId: {categoryId}, topicId: {topicId},  AdminId: {adminId}"
                );
                return null;
            }

            return GetBrandedFrameworkByFrameworkId(frameworkId, adminId);
        }

        public int InsertCompetencyGroup(string groupName, string? groupDescription, int adminId, int? frameworkId)
        {
            if ((groupName.Length == 0) | (adminId < 1))
            {
                logger.LogWarning(
                    $"Not inserting competency group as it failed server side validation. AdminId: {adminId}, GroupName: {groupName}"
                );
                return -2;
            }
            groupDescription = (groupDescription?.Trim() == "" ? null : groupDescription);
            var existingId = connection.QuerySingle<int>(
                @"SELECT COALESCE
                 ((SELECT TOP (1) ID
                  FROM    CompetencyGroups
                  WHERE (Name = @groupName) AND EXISTS
                                   (SELECT 1 AS Expr1
                                   FROM    FrameworkCompetencyGroups
                                   WHERE (CompetencyGroupID = CompetencyGroups.ID) AND (FrameworkID = @frameworkId) OR
                                                (CompetencyGroupID = CompetencyGroups.ID) AND (@frameworkId IS NULL))), 0) AS CompetencyGroupID",
                new { groupName, groupDescription, frameworkId }
            );
            if (existingId > 0)
            {
                return existingId;
            }

            existingId = connection.QuerySingle<int>(
                @"INSERT INTO CompetencyGroups ([Name], [Description], UpdatedByAdminID)
                    OUTPUT INSERTED.Id
                    VALUES (@groupName, @groupDescription, @adminId)",
                new { groupName, groupDescription, adminId }
            );
            
            return existingId;
        }

        public int InsertFrameworkCompetencyGroup(int groupId, int frameworkId, int adminId)
        {
            if ((groupId < 1) | (frameworkId < 1) | (adminId < 1))
            {
                logger.LogWarning(
                    $"Not inserting framework competency group as it failed server side validation. AdminId: {adminId}, frameworkId: {frameworkId}, groupId: {groupId}"
                );
                return -2;
            }

            var existingId = connection.QuerySingle<int>(
                @"SELECT COALESCE ((SELECT ID FROM FrameworkCompetencyGroups WHERE CompetencyGroupID = @groupID AND FrameworkID = @frameworkID), 0) AS FrameworkCompetencyGroupID",
                new { groupId, frameworkId }
            );
            if (existingId > 0)
            {
                return existingId;
            }

            existingId = connection.QuerySingle<int>(
                @"INSERT INTO FrameworkCompetencyGroups (CompetencyGroupID, UpdatedByAdminID, Ordering, FrameworkID)
                    OUTPUT INSERTED.Id
                    VALUES (@groupId, @adminId, COALESCE
                             ((SELECT        MAX(Ordering)
                                 FROM            [FrameworkCompetencyGroups]
                                 WHERE        ([FrameworkID] = @frameworkId)), 0)+1, @frameworkId)",
                new { groupId, adminId, frameworkId }
            );
           
            return existingId;
        }

        public int InsertCompetency(string name, string? description, int adminId)
        {
            if ((name.Length == 0) | (adminId < 1))
            {
                logger.LogWarning(
                    $"Not inserting competency as it failed server side valiidation. AdminId: {adminId}, name: {name}, description:{description}"
                );
                return -2;
            }
            description = (description?.Trim() == "" ? null : description);

            var existingId = connection.QuerySingle<int>(
                @"INSERT INTO Competencies ([Name], [Description], UpdatedByAdminID)
                    OUTPUT INSERTED.Id
                    VALUES (@name, @description, @adminId)",
                new { name, description, adminId }
            );

            return existingId;
        }

        public int InsertFrameworkCompetency(
            int competencyId,
            int? frameworkCompetencyGroupID,
            int adminId,
            int frameworkId,
            bool alwaysShowDescription = false
        )
        {
            if ((competencyId < 1) | (adminId < 1) | (frameworkId < 1))
            {
                logger.LogWarning(
                    $"Not inserting framework competency as it failed server side valiidation. AdminId: {adminId}, frameworkCompetencyGroupID: {frameworkCompetencyGroupID}, competencyId:{competencyId}, frameworkId:{frameworkId}"
                );
                return -2;
            }

            var existingId = 0;
            if (frameworkCompetencyGroupID == null)
            {
                existingId = connection.QuerySingle<int>(
                    @"SELECT COALESCE ((SELECT ID FROM FrameworkCompetencies WHERE [CompetencyID] = @competencyId AND FrameworkCompetencyGroupID IS NULL), 0) AS FrameworkCompetencyID",
                    new { competencyId, frameworkCompetencyGroupID }
                );
            }
            else
            {
                existingId = connection.QuerySingle<int>(
                    @"SELECT COALESCE ((SELECT ID FROM FrameworkCompetencies WHERE [CompetencyID] = @competencyId AND FrameworkCompetencyGroupID = @frameworkCompetencyGroupID), 0) AS FrameworkCompetencyID",
                    new { competencyId, frameworkCompetencyGroupID }
                );
            }

            if (existingId > 0)
            {
                return existingId;
            }

            var numberOfAffectedRows = connection.Execute(
                @"INSERT INTO FrameworkCompetencies ([CompetencyID], FrameworkCompetencyGroupID, UpdatedByAdminID, Ordering, FrameworkID)
                    VALUES (@competencyId, @frameworkCompetencyGroupID, @adminId, COALESCE
                             ((SELECT        MAX(Ordering) AS OrderNum
                                 FROM            [FrameworkCompetencies]
                                 WHERE        ([FrameworkCompetencyGroupID] = @frameworkCompetencyGroupID)), 0)+1, @frameworkId)",
                new { competencyId, frameworkCompetencyGroupID, adminId, frameworkId }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    $"Not inserting framework competency as db insert failed. AdminId: {adminId}, frameworkCompetencyGroupID: {frameworkCompetencyGroupID}, competencyId: {competencyId}"
                );
                return -1;
            }

            if (frameworkCompetencyGroupID == null)
            {
                existingId = connection.QuerySingle<int>(
                    @"SELECT COALESCE ((SELECT ID FROM FrameworkCompetencies WHERE [CompetencyID] = @competencyId AND FrameworkCompetencyGroupID IS NULL), 0) AS FrameworkCompetencyID",
                    new { competencyId, frameworkCompetencyGroupID }
                );
            }
            else
            {
                existingId = connection.QuerySingle<int>(
                    @"SELECT COALESCE ((SELECT ID FROM FrameworkCompetencies WHERE [CompetencyID] = @competencyId AND FrameworkCompetencyGroupID = @frameworkCompetencyGroupID), 0) AS FrameworkCompetencyID",
                    new { competencyId, frameworkCompetencyGroupID }
                );
            }

            AddDefaultQuestionsToCompetency(competencyId, frameworkId);
            return existingId;
        }

        public IEnumerable<CollaboratorDetail> GetCollaboratorsForFrameworkId(int frameworkId)
        {
            return connection.Query<CollaboratorDetail>(
                @"SELECT
                        0 AS ID,
                        fw.ID AS FrameworkID,
                        au.AdminID AS AdminID,
                        1 AS CanModify,
                        au.Email AS UserEmail,
                        au.Active AS UserActive,
                        'Owner' AS FrameworkRole
                    FROM Frameworks AS fw
                    INNER JOIN AdminUsers AS au ON fw.OwnerAdminID = au.AdminID
                    WHERE (fw.ID = @FrameworkID)
                    UNION ALL
                    SELECT
                        ID,
                        FrameworkID,
                        fc.AdminID,
                        CanModify,
                        UserEmail,
                        au.Active AS UserActive,
                        CASE WHEN CanModify = 1 THEN 'Contributor' ELSE 'Reviewer' END AS FrameworkRole
                    FROM FrameworkCollaborators fc
                    INNER JOIN AdminUsers AS au ON fc.AdminID = au.AdminID
                    AND fc.IsDeleted = 0
                    WHERE (FrameworkID = @FrameworkID)",
                new { frameworkId }
            );
        }

        public int AddCollaboratorToFramework(int frameworkId, string? userEmail, bool canModify, int? centreID)
        {
            if (userEmail is null || userEmail.Length == 0)
            {
                logger.LogWarning(
                    $"Not adding collaborator to framework as it failed server side valiidation. frameworkId: {frameworkId}, userEmail: {userEmail}, canModify:{canModify}"
                );
                return -3;
            }

            var existingId = connection.QuerySingle<int>(
                @"SELECT COALESCE
                 ((SELECT ID
                  FROM    FrameworkCollaborators
                  WHERE (FrameworkID = @frameworkId) AND (UserEmail = @userEmail) AND (IsDeleted=0)), 0) AS ID",
                new { frameworkId, userEmail }
            );
            if (existingId > 0)
            {
                return -2;
            }

            var adminId = (int?)connection.ExecuteScalar(
                @"SELECT AdminID FROM AdminUsers WHERE Email = @userEmail AND Active = 1 AND CentreID = @centreID",
                new { userEmail, centreID }
            );
            if (adminId is null)
            {
                return -4;
            }

            var ownerEmail = (string?)connection.ExecuteScalar(@"SELECT AU.Email FROM Frameworks F
                INNER JOIN AdminUsers AU ON AU.AdminID=F.OwnerAdminID
                WHERE F.ID=@frameworkId", new { frameworkId });
            if (ownerEmail == userEmail)
            {
                return -5;
            }

            var numberOfAffectedRows = connection.Execute(
                @"INSERT INTO FrameworkCollaborators (FrameworkID, AdminID, UserEmail, CanModify)
                    VALUES (@frameworkId, @adminId, @userEmail, @canModify)",
                new { frameworkId, adminId, userEmail, canModify }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    $"Not inserting framework collaborator as db insert failed. AdminId: {adminId}, userEmail: {userEmail}, frameworkId: {frameworkId}, canModify: {canModify}"
                );
                return -1;
            }

            if (adminId > 0)
            {
                connection.Execute(
                    @"UPDATE AdminUsers SET IsFrameworkContributor = 1 WHERE AdminId = @adminId AND IsFrameworkContributor = 0",
                    new { adminId }
                );
            }

            existingId = connection.QuerySingle<int>(
                @"SELECT COALESCE
                 ((SELECT ID
                  FROM    FrameworkCollaborators
                  WHERE (FrameworkID = @frameworkId) AND (UserEmail = @userEmail) AND (IsDeleted=0)), 0) AS AdminID",
                new { frameworkId, adminId, userEmail }
            );
            return existingId;
        }

        public void RemoveCollaboratorFromFramework(int frameworkId, int id)
        {
            var adminId = (int?)connection.ExecuteScalar(
                @"SELECT AdminID FROM FrameworkCollaborators WHERE  (FrameworkID = @frameworkId) AND (ID = @id)",
                new { frameworkId, id }
            );
            connection.Execute(
                @"UPDATE FrameworkCollaborators SET IsDeleted=1 WHERE (FrameworkID = @frameworkId) AND (ID = @id);UPDATE AdminUsers SET IsFrameworkContributor = 0 WHERE AdminID = @adminId AND AdminID NOT IN (SELECT DISTINCT AdminID FROM FrameworkCollaborators);",
                new { frameworkId, id, adminId }
            );
        }

        public void RemoveCustomFlag(int flagId)
        {
            connection.Execute(
                @"DELETE FROM CompetencyFlags WHERE FlagID = @flagId", new { flagId }
            );
            connection.Execute(
                @"DELETE FROM Flags WHERE ID = @flagId", new { flagId }
            );
        }

        public IEnumerable<FrameworkCompetencyGroup> GetFrameworkCompetencyGroups(int frameworkId)
        {
            var result = connection.Query<FrameworkCompetencyGroup, FrameworkCompetency, FrameworkCompetencyGroup>(
                @"SELECT fcg.ID, fcg.CompetencyGroupID, cg.Name, fcg.Ordering, fc.ID, c.ID AS CompetencyID, c.Name, c.Description, fc.Ordering, COUNT(caq.AssessmentQuestionID) AS AssessmentQuestions
                    ,(SELECT COUNT(*) FROM CompetencyLearningResources clr WHERE clr.CompetencyID = c.ID AND clr.RemovedDate IS NULL) AS CompetencyLearningResourcesCount
                    FROM   FrameworkCompetencyGroups AS fcg INNER JOIN
                      CompetencyGroups AS cg ON fcg.CompetencyGroupID = cg.ID LEFT OUTER JOIN
                       FrameworkCompetencies AS fc ON fcg.ID = fc.FrameworkCompetencyGroupID LEFT OUTER JOIN
                       Competencies AS c ON fc.CompetencyID = c.ID LEFT OUTER JOIN
                      CompetencyAssessmentQuestions AS caq ON c.ID = caq.CompetencyID
                    WHERE (fcg.FrameworkID = @frameworkId)
                    GROUP BY fcg.ID, fcg.CompetencyGroupID, cg.Name, fcg.Ordering, fc.ID, c.ID, c.Name, c.Description, fc.Ordering
                    ORDER BY fcg.Ordering, fc.Ordering",
                (frameworkCompetencyGroup, frameworkCompetency) =>
                {
                    if (frameworkCompetency != null)
                        frameworkCompetencyGroup.FrameworkCompetencies.Add(frameworkCompetency);
                    return frameworkCompetencyGroup;
                },
                new { frameworkId }
            );
            return result.GroupBy(frameworkCompetencyGroup => frameworkCompetencyGroup.CompetencyGroupID).Select(
                group =>
                {
                    var groupedFrameworkCompetencyGroup = group.First();
                    groupedFrameworkCompetencyGroup.FrameworkCompetencies = group.Where(frameworkCompetencyGroup => frameworkCompetencyGroup.FrameworkCompetencies.Count > 0)
                    .Select(
                        frameworkCompetencyGroup => frameworkCompetencyGroup.FrameworkCompetencies.Single()
                    ).ToList();
                    return groupedFrameworkCompetencyGroup;
                }
            );
        }

        public IEnumerable<FrameworkCompetency> GetFrameworkCompetenciesUngrouped(int frameworkId)
        {
            return connection.Query<FrameworkCompetency>(
                @"SELECT fc.ID, c.ID AS CompetencyID, c.Name, c.Description, fc.Ordering, COUNT(caq.AssessmentQuestionID) AS AssessmentQuestions,(select COUNT(CompetencyId) from CompetencyLearningResources where CompetencyID=c.ID) AS CompetencyLearningResourcesCount
                    FROM FrameworkCompetencies AS fc
                        INNER JOIN Competencies AS c ON fc.CompetencyID = c.ID
                        LEFT OUTER JOIN
                      CompetencyAssessmentQuestions AS caq ON c.ID = caq.CompetencyID
                    WHERE fc.FrameworkID = @frameworkId
                        AND fc.FrameworkCompetencyGroupID IS NULL
GROUP BY fc.ID, c.ID, c.Name, c.Description, fc.Ordering
                    ORDER BY fc.Ordering",
                new { frameworkId }
            );
        }

        public bool UpdateFrameworkName(int frameworkId, int adminId, string frameworkName)
        {
            if ((frameworkName.Length == 0) | (adminId < 1) | (frameworkId < 1))
            {
                logger.LogWarning(
                    $"Not updating framework name as it failed server side validation. AdminId: {adminId}, frameworkName: {frameworkName}, frameworkId: {frameworkId}"
                );
                return false;
            }

            var existingFrameworks = connection.QuerySingle<int>(
                @"SELECT COUNT(*) FROM Frameworks WHERE FrameworkName = @frameworkName AND ID <> @frameworkId",
                new { frameworkName, frameworkId }
            );
            if (existingFrameworks > 0)
            {
                return false;
            }

            var numberOfAffectedRows = connection.Execute(
                @"UPDATE Frameworks SET FrameworkName = @frameworkName, UpdatedByAdminID = @adminId
                    WHERE ID = @frameworkId",
                new { frameworkName, adminId, frameworkId }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating framework name as db update failed. " +
                    $"FrameworkName: {frameworkName}, admin id: {adminId}, frameworkId: {frameworkId}"
                );
                return false;
            }

            return true;
        }

        public void UpdateFrameworkDescription(int frameworkId, int adminId, string? frameworkDescription)
        {
            if ((adminId < 1) | (frameworkId < 1))
            {
                logger.LogWarning(
                    $"Not updating framework description as it failed server side validation. AdminId: {adminId}, frameworkDescription: {frameworkDescription}, frameworkId: {frameworkId}"
                );
            }

            var numberOfAffectedRows = connection.Execute(
                @"UPDATE Frameworks SET Description = @frameworkDescription, UpdatedByAdminID = @adminId
                    WHERE ID = @frameworkId",
                new { frameworkDescription, adminId, frameworkId }
            );
        }

        public void UpdateFrameworkConfig(int frameworkId, int adminId, string? frameworkConfig)
        {
            if ((adminId < 1) | (frameworkId < 1))
            {
                logger.LogWarning(
                    $"Not updating framework config as it failed server side validation. AdminId: {adminId}, frameworkConfig: {frameworkConfig}, frameworkId: {frameworkId}"
                );
            }

            var numberOfAffectedRows = connection.Execute(
                @"UPDATE Frameworks SET FrameworkConfig = @frameworkConfig, UpdatedByAdminID = @adminId
                    WHERE ID = @frameworkId",
                new { frameworkConfig, adminId, frameworkId }
            );
        }

        public CompetencyGroupBase? GetCompetencyGroupBaseById(int Id)
        {
            return connection.QueryFirstOrDefault<CompetencyGroupBase>(
                @"SELECT fcg.ID, fcg.CompetencyGroupID, cg.Name, cg.Description
                    FROM   FrameworkCompetencyGroups AS fcg
                        INNER JOIN CompetencyGroups AS cg ON fcg.CompetencyGroupID = cg.ID
                    WHERE (fcg.ID = @Id)",
                new { Id }
            );
        }

        public FrameworkCompetency? GetFrameworkCompetencyById(int Id)
        {
            return connection.QueryFirstOrDefault<FrameworkCompetency>(
                @"SELECT fc.ID, c.ID AS CompetencyID, c.Name, c.Description, fc.Ordering, c.AlwaysShowDescription
                    FROM FrameworkCompetencies AS fc
                        INNER JOIN Competencies AS c ON fc.CompetencyID = c.ID
                    WHERE fc.ID = @Id",
                new { Id }
            );
        }

        public void UpdateFrameworkCompetencyGroup(
            int frameworkCompetencyGroupId,
            int competencyGroupId,
            string name,
            string? description,
            int adminId
        )
        {
            if ((frameworkCompetencyGroupId < 1) | (adminId < 1) | (competencyGroupId < 1) | (name.Length < 3))
            {
                logger.LogWarning(
                    $"Not updating framework competency group as it failed server side validation. AdminId: {adminId}, frameworkCompetencyGroupId: {frameworkCompetencyGroupId}, competencyGroupId: {competencyGroupId}, name: {name}"
                );
                return;
            }

            var usedElsewhere = connection.QuerySingle<int>(
                @"SELECT COUNT(*) FROM FrameworkCompetencyGroups
                    WHERE CompetencyGroupId = @competencyGroupId
                    AND ID <> @frameworkCompetencyGroupId",
                new { frameworkCompetencyGroupId, competencyGroupId }
            );
            if (usedElsewhere > 0)
            {
                var newCompetencyGroupId = InsertCompetencyGroup(name, description, adminId, null);
                if (newCompetencyGroupId > 0)
                {
                    var numberOfAffectedRows = connection.Execute(
                        @"UPDATE FrameworkCompetencyGroups
                            SET CompetencyGroupID = @newCompetencyGroupId, UpdatedByAdminID = @adminId
                            WHERE ID = @frameworkCompetencyGroupId",
                        new { newCompetencyGroupId, adminId, frameworkCompetencyGroupId }
                    );
                    if (numberOfAffectedRows < 1)
                    {
                        logger.LogWarning(
                            "Not updating competency group id as db update failed. " +
                            $"newCompetencyGroupId: {newCompetencyGroupId}, admin id: {adminId}, frameworkCompetencyGroupId: {frameworkCompetencyGroupId}"
                        );
                    }
                }
            }
            else
            {
                var numberOfAffectedRows = connection.Execute(
                    @"UPDATE CompetencyGroups SET Name = @name, UpdatedByAdminID = @adminId, Description = @description
                    WHERE ID = @competencyGroupId",
                    new { name, adminId, competencyGroupId, description }
                );
                if (numberOfAffectedRows < 1)
                {
                    logger.LogWarning(
                        "Not updating competency group name as db update failed. " +
                        $"Name: {name}, admin id: {adminId}, competencyGroupId: {competencyGroupId}"
                    );
                }
            }
        }

        public void UpdateFrameworkCompetency(int frameworkCompetencyId, string name, string? description, int adminId, bool? alwaysShowDescription)
        {
            if ((frameworkCompetencyId < 1) | (adminId < 1) | (name.Length < 3))
            {
                logger.LogWarning(
                    $"Not updating framework competency as it failed server side validation. AdminId: {adminId}, frameworkCompetencyId: {frameworkCompetencyId}, name: {name}, description: {description}"
                );
                return;
            }

            //DO WE NEED SOMETHING IN HERE TO CHECK WHETHER IT IS USED ELSEWHERE AND WARN THE USER?
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE Competencies SET Name = @name, Description = @description, UpdatedByAdminID = @adminId, AlwaysShowDescription = CASE WHEN @alwaysShowDescription IS NULL THEN AlwaysShowDescription ELSE @alwaysShowDescription END
                    FROM   Competencies INNER JOIN FrameworkCompetencies AS fc ON Competencies.ID = fc.CompetencyID
                    WHERE (fc.Id = @frameworkCompetencyId)",
                new { name, description, adminId, frameworkCompetencyId, alwaysShowDescription}
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating competency group name as db update failed. " +
                    $"Name: {name}, admin id: {adminId}, frameworkCompetencyId: {frameworkCompetencyId}"
                );
            }
        }

        public int UpdateCompetencyFlags(int frameworkId, int competencyId, int[] selectedFlagIds)
        {
            int totalRowsAffected = 0;
            string? commaSeparatedSelectedFlagIds = null;
            if (selectedFlagIds?.Length > 0)
            {
                commaSeparatedSelectedFlagIds = String.Join(',', selectedFlagIds);
                totalRowsAffected += connection.Execute(
                    @$"INSERT INTO CompetencyFlags(CompetencyID, FlagID, Selected)
						SELECT @competencyId, f.ID, 1
						FROM Flags f
						WHERE f.ID IN ({commaSeparatedSelectedFlagIds}) AND NOT EXISTS(
							SELECT FlagID FROM CompetencyFlags
							WHERE FlagID = f.ID AND CompetencyID = @competencyId
						)",
                    new { competencyId, selectedFlagIds });
            }
            totalRowsAffected += connection.Execute(
                @$"UPDATE CompetencyFlags
                    SET Selected = (CASE WHEN FlagID IN ({commaSeparatedSelectedFlagIds ?? "null"}) THEN 1 ELSE 0 END)
                    WHERE CompetencyID = @competencyId AND Selected <> (CASE WHEN FlagID IN ({commaSeparatedSelectedFlagIds ?? "null"}) THEN 1 ELSE 0 END)",
                new { competencyId, frameworkId });
            return totalRowsAffected;
        }

        public int AddCustomFlagToFramework(int frameworkId, string flagName, string flagGroup, string flagTagClass)
        {
            return connection.QuerySingle<int>(
                @"INSERT INTO Flags(FrameworkID, FlagName, FlagGroup, FlagTagClass)
                    OUTPUT INSERTED.ID
                    VALUES(@frameworkId, @flagName, @flagGroup, @flagTagClass);",
                new { frameworkId, flagName, flagGroup, flagTagClass });
        }

        public void UpdateFrameworkCustomFlag(int frameworkId, int id, string flagName, string flagGroup, string flagTagClass)
        {
            connection.Execute(
                @$"UPDATE Flags
                   SET FrameworkID = @frameworkId, FlagName = @flagName, FlagGroup = @flagGroup, FlagTagClass = @flagTagClass
                   WHERE ID = @id",
                new { frameworkId, id, flagName, flagGroup, flagTagClass });
        }

        public void MoveFrameworkCompetencyGroup(int frameworkCompetencyGroupId, bool singleStep, string direction) // Valid directions are 'UP' and 'DOWN'
        {
            connection.Execute(
                "ReorderFrameworkCompetencyGroup",
                new { frameworkCompetencyGroupId, direction, singleStep },
                commandType: CommandType.StoredProcedure
            );
        }

        public void MoveFrameworkCompetency(int frameworkCompetencyId, bool singleStep, string direction) // Valid directions are 'UP' and 'DOWN'
        {
            connection.Execute(
                "ReorderFrameworkCompetency",
                new { frameworkCompetencyId, direction, singleStep },
                commandType: CommandType.StoredProcedure
            );
        }

        public void DeleteFrameworkCompetencyGroup(int frameworkCompetencyGroupId, int competencyGroupId, int adminId)
        {
            if ((frameworkCompetencyGroupId < 1) | (adminId < 1))
            {
                logger.LogWarning(
                    $"Not deleting framework competency group as it failed server side validation. AdminId: {adminId}," +
                    $"frameworkCompetencyGroupId: {frameworkCompetencyGroupId},"
                );
                return;
            }

            connection.Execute(
                @"UPDATE FrameworkCompetencyGroups
                   SET UpdatedByAdminID = @adminId
                    WHERE ID = @frameworkCompetencyGroupId",
                new { adminId, frameworkCompetencyGroupId }
            );


            connection.Execute(
                @"DELETE FROM FrameworkCompetencies
                WHERE FrameworkCompetencyGroupID = @frameworkCompetencyGroupId",
                new { frameworkCompetencyGroupId }
            );

            var numberOfAffectedRows = connection.Execute(
                @"DELETE FROM FrameworkCompetencyGroups
                WHERE ID = @frameworkCompetencyGroupId",
                new { frameworkCompetencyGroupId }
            );

            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not deleting framework competency group as db update failed. " +
                    $"frameworkCompetencyGroupId: {frameworkCompetencyGroupId}, adminId: {adminId}"
                );
            }

            //Check if used elsewhere and delete competency group if not:
            var usedElsewhere = connection.QuerySingle<int>(
                @"SELECT COUNT(*) FROM FrameworkCompetencyGroups
                    WHERE CompetencyGroupId = @competencyGroupId",
                new { competencyGroupId }
            );
            if (usedElsewhere == 0)
            {
                usedElsewhere = connection.QuerySingle<int>(
                    @"SELECT COUNT(*) FROM SelfAssessmentStructure
                    WHERE CompetencyGroupId = @competencyGroupId",
                    new { competencyGroupId }
                );
            }

            if (usedElsewhere == 0)
            {
                connection.Execute(
                    @"UPDATE CompetencyGroups
                   SET UpdatedByAdminID = @adminId
                    WHERE ID = @competencyGroupId",
                    new { adminId, competencyGroupId }
                );
                numberOfAffectedRows = connection.Execute(
                    @"DELETE FROM CompetencyGroups WHERE ID = @competencyGroupId",
                    new { competencyGroupId }
                );
                if (numberOfAffectedRows < 1)
                {
                    logger.LogWarning(
                        "Not deleting competency group as db update failed. " +
                        $"competencyGroupId: {competencyGroupId}, adminId: {adminId}"
                    );
                }
            }
        }

        public void DeleteFrameworkCompetency(int frameworkCompetencyId, int adminId)
        {
            var competencyId = connection.QuerySingle<int>(
                @"SELECT CompetencyID FROM FrameworkCompetencies WHERE ID = @frameworkCompetencyId",
                new { frameworkCompetencyId }
            );
            if ((frameworkCompetencyId < 1) | (adminId < 1) | (competencyId < 1))
            {
                logger.LogWarning(
                    $"Not deleting framework competency group as it failed server side validation. AdminId: {adminId}, frameworkCompetencyId: {frameworkCompetencyId}, competencyId: {competencyId}"
                );
                return;
            }

            connection.Execute(
                @"UPDATE FrameworkCompetencies
                   SET UpdatedByAdminID = @adminId
                    WHERE ID = @frameworkCompetencyId",
                new { adminId, frameworkCompetencyId }
            );
            var numberOfAffectedRows = connection.Execute(
                @"DELETE FROM FrameworkCompetencies WHERE ID = @frameworkCompetencyId",
                new { frameworkCompetencyId }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not deleting framework competency as db update failed. " +
                    $"frameworkCompetencyId: {frameworkCompetencyId}, competencyId: {competencyId}, adminId: {adminId}"
                );
            }

            //Check if used elsewhere and delete competency group if not:
            var usedElsewhere = connection.QuerySingle<int>(
                @"SELECT COUNT(*) FROM FrameworkCompetencies
                    WHERE CompetencyID = @competencyId",
                new { competencyId }
            );
            if (usedElsewhere == 0)
            {
                usedElsewhere = connection.QuerySingle<int>(
                    @"SELECT COUNT(*) FROM SelfAssessmentStructure
                    WHERE CompetencyID = @competencyId",
                    new { competencyId }
                );
            }

            if (usedElsewhere == 0)
            {
                connection.Execute(
                    @"UPDATE Competencies
                   SET UpdatedByAdminID = @adminId
                    WHERE ID = @competencyId",
                    new { adminId, competencyId }
                );
                numberOfAffectedRows = connection.Execute(
                    @"DELETE FROM CompetencyAssessmentQuestions WHERE CompetencyID = @competencyId;
                     DELETE FROM CompetencyFlags WHERE CompetencyID = @competencyId;
                     DELETE FROM CompetencyResourceAssessmentQuestionParameters WHERE CompetencyLearningResourceID IN (SELECT ID FROM CompetencyLearningResources WHERE CompetencyID = @competencyId);
                     DELETE FROM CompetencyLearningResources WHERE CompetencyID = @competencyId;
                     DELETE FROM Competencies WHERE ID = @competencyId;",
                    new { competencyId }
                );
                if (numberOfAffectedRows < 1)
                {
                    logger.LogWarning(
                        "Not deleting competency as db update failed. " +
                        $"competencyId: {competencyId}, adminId: {adminId}"
                    );
                }
            }
        }

        public void DeleteCompetencyLearningResource(int competencyLearningResourceId, int adminId)
        {
            var numberOfAffectedRows = connection.Execute(
                @"
                IF EXISTS(
                        SELECT * FROM CompetencyLearningResources AS clr
                        WHERE clr.ID = @competencyLearningResourceId
                        AND NOT EXISTS (SELECT * FROM LearningLogItems AS lli WHERE lli.LearningResourceReferenceID = clr.LearningResourceReferenceID)
                        AND NOT EXISTS (SELECT * FROM CompetencyResourceAssessmentQuestionParameters AS p WHERE p.CompetencyLearningResourceID = clr.ID)
                        )
                    BEGIN
                        DELETE FROM CompetencyLearningResources
                        WHERE ID = @competencyLearningResourceId
                    END
                ELSE
                    BEGIN
                        UPDATE CompetencyLearningResources
                        SET RemovedDate = GETDATE(),
                            RemovedByAdminID = @adminId
                        WHERE ID = @competencyLearningResourceId
                    END",
                new { competencyLearningResourceId, adminId }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not deleting competency learning resource as db update failed. " +
                    $"competencyLearningResourceId: {competencyLearningResourceId}, adminId: {adminId}"
                );
            }
        }

        public IEnumerable<AssessmentQuestion> GetAllCompetencyQuestions(int adminId)
        {
            return connection.Query<AssessmentQuestion>(
                $@"{AssessmentQuestionFields}
                    {AssessmentQuestionTables}
                     ORDER BY [Question]",
                new { adminId }
            );
        }

        public IEnumerable<AssessmentQuestion> GetFrameworkDefaultQuestionsById(int frameworkId, int adminId)
        {
            return connection.Query<AssessmentQuestion>(
                $@"{AssessmentQuestionFields}
                    {AssessmentQuestionTables}
                    INNER JOIN FrameworkDefaultQuestions AS FDQ ON AQ.ID = FDQ.AssessmentQuestionID
                    WHERE FDQ.FrameworkId = @frameworkId",
                new { frameworkId, adminId }
            );
        }

        public IEnumerable<AssessmentQuestion> GetCompetencyAssessmentQuestionsById(int competencyId, int adminId)
        {
            return connection.Query<AssessmentQuestion>(
                $@"{AssessmentQuestionFields}
                    {AssessmentQuestionTables}
                     INNER JOIN CompetencyAssessmentQuestions AS CAQ ON AQ.ID = CAQ.AssessmentQuestionID
                     WHERE CAQ.CompetencyID = @competencyId
                     ORDER BY [Question]",
                new { competencyId, adminId }
            );
        }

        public void AddFrameworkDefaultQuestion(
            int frameworkId,
            int assessmentQuestionId,
            int adminId,
            bool addToExisting
        )
        {
            if ((frameworkId < 1) | (adminId < 1) | (assessmentQuestionId < 1))
            {
                logger.LogWarning(
                    $"Not inserting framework default question as it failed server side validation. AdminId: {adminId}, frameworkId: {frameworkId}, assessmentQuestionId: {assessmentQuestionId}"
                );
                return;
            }

            var numberOfAffectedRows = connection.Execute(
                @"INSERT INTO FrameworkDefaultQuestions (FrameworkId, AssessmentQuestionID)
                      VALUES (@frameworkId, @assessmentQuestionId)",
                new { frameworkId, assessmentQuestionId }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not inserting framework default question as db update failed. " +
                    $"frameworkId: {frameworkId}, assessmentQuestionId: {assessmentQuestionId}"
                );
            }
            else if (addToExisting)
            {
                numberOfAffectedRows = connection.Execute(
                    @"INSERT INTO CompetencyAssessmentQuestions (CompetencyID, AssessmentQuestionID, Ordering)
                        SELECT DISTINCT CompetencyID, @assessmentQuestionId AS AssessmentQuestionID, COALESCE
                             ((SELECT        MAX(Ordering)
                                 FROM            [CompetencyAssessmentQuestions]
                                 WHERE        ([CompetencyId] = fc.CompetencyID)), 0)+1 AS Ordering
                        FROM FrameworkCompetencies AS fc
                        WHERE FrameworkID = @frameworkId AND NOT EXISTS (SELECT * FROM CompetencyAssessmentQuestions WHERE CompetencyID = fc.CompetencyID AND AssessmentQuestionID = @assessmentQuestionId)",
                    new { assessmentQuestionId, frameworkId }
                );
            }
        }

        public void DeleteFrameworkDefaultQuestion(
            int frameworkId,
            int assessmentQuestionId,
            int adminId,
            bool deleteFromExisting
        )
        {
            if ((frameworkId < 1) | (adminId < 1) | (assessmentQuestionId < 1))
            {
                logger.LogWarning(
                    $"Not deleting framework default question as it failed server side validation. AdminId: {adminId}, frameworkId: {frameworkId}, assessmentQuestionId: {assessmentQuestionId}"
                );
                return;
            }

            var numberOfAffectedRows = connection.Execute(
                @"DELETE FROM FrameworkDefaultQuestions
                    WHERE FrameworkId = @frameworkId AND AssessmentQuestionID = @assessmentQuestionId",
                new { frameworkId, assessmentQuestionId }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not deleting framework default question as db update failed. " +
                    $"frameworkId: {frameworkId}, assessmentQuestionId: {assessmentQuestionId}"
                );
            }
            else if (deleteFromExisting)
            {
                numberOfAffectedRows = connection.Execute(
                    @"DELETE FROM CompetencyAssessmentQuestions
                        WHERE AssessmentQuestionID = @assessmentQuestionId
                        AND CompetencyID IN (
                            SELECT CompetencyID FROM FrameworkCompetencies
                            WHERE FrameworkID = @frameworkId)",
                    new { frameworkId, assessmentQuestionId }
                );
            }
        }

        public IEnumerable<GenericSelectList> GetAssessmentQuestions(int frameworkId, int adminId)
        {
            return connection.Query<GenericSelectList>(
                @"SELECT AQ.ID, CASE WHEN AddedByAdminId = @adminId THEN '* ' ELSE '' END + Question + ' (' + InputTypeName + ' ' + CAST(MinValue AS nvarchar) + ' to ' + CAST(MaxValue As nvarchar) + ')' AS Label
                    FROM AssessmentQuestions AS AQ LEFT OUTER JOIN AssessmentQuestionInputTypes AS AQI ON AQ.AssessmentQuestionInputTypeID = AQI.ID
                    WHERE AQ.ID NOT IN (SELECT AssessmentQuestionID FROM FrameworkDefaultQuestions WHERE FrameworkId = @frameworkId)
                    ORDER BY Label",
                new { frameworkId, adminId }
            );
        }

        public IEnumerable<GenericSelectList> GetAssessmentQuestionsForCompetency(
            int frameworkCompetencyId,
            int adminId
        )
        {
            return connection.Query<GenericSelectList>(
                @"SELECT AQ.ID, CASE WHEN AddedByAdminId = @adminId THEN '* ' ELSE '' END + Question + ' (' + InputTypeName + ' ' + CAST(MinValue AS nvarchar) + ' to ' + CAST(MaxValue As nvarchar) + ')' AS Label
                    FROM AssessmentQuestions AS AQ LEFT OUTER JOIN AssessmentQuestionInputTypes AS AQI ON AQ.AssessmentQuestionInputTypeID = AQI.ID
                    WHERE AQ.ID NOT IN (SELECT AssessmentQuestionID FROM CompetencyAssessmentQuestions AS CAQ INNER JOIN FrameworkCompetencies AS FC ON CAQ.CompetencyID = FC.CompetencyID WHERE FC.ID = @frameworkCompetencyId) ORDER BY Question",
                new { frameworkCompetencyId, adminId }
            );
        }

        public IEnumerable<GenericSelectList> GetAssessmentQuestionInputTypes()
        {
            return connection.Query<GenericSelectList>(
                @"SELECT ID, InputTypeName AS Label
                    FROM AssessmentQuestionInputTypes"
            );
        }

        public FrameworkDefaultQuestionUsage GetFrameworkDefaultQuestionUsage(int frameworkId, int assessmentQuestionId)
        {
            return connection.QueryFirstOrDefault<FrameworkDefaultQuestionUsage>(
                @"SELECT @assessmentQuestionId AS ID,
                 (SELECT AQ.Question + ' (' + AQI.InputTypeName + ' ' + CAST(AQ.MinValue AS nvarchar) + ' to ' + CAST(AQ.MaxValue AS nvarchar) + ')' AS Expr1
                 FROM    AssessmentQuestions AS AQ LEFT OUTER JOIN
                              AssessmentQuestionInputTypes AS AQI ON AQ.AssessmentQuestionInputTypeID = AQI.ID
                 WHERE (AQ.ID = @assessmentQuestionId)) AS Question, COUNT(CompetencyID) AS Competencies,
                 (SELECT COUNT(CompetencyID) AS Expr1
                 FROM    CompetencyAssessmentQuestions
                 WHERE (AssessmentQuestionID = @assessmentQuestionId) AND (CompetencyID IN
                                  (SELECT CompetencyID
                                  FROM    FrameworkCompetencies
                                  WHERE (FrameworkID = @frameworkId)))) AS CompetencyAssessmentQuestions
FROM   FrameworkCompetencies AS FC
WHERE (FrameworkID = @frameworkId)",
                new { frameworkId, assessmentQuestionId }
            );
        }

        public IEnumerable<AssessmentQuestion> GetCompetencyAssessmentQuestionsByFrameworkCompetencyId(
            int frameworkCompetencyId,
            int adminId
        )
        {
            return connection.Query<AssessmentQuestion>(
                $@"{AssessmentQuestionFields}
                    {AssessmentQuestionTables}
                    INNER JOIN CompetencyAssessmentQuestions AS CAQ ON AQ.ID = CAQ.AssessmentQuestionID
                    INNER JOIN FrameworkCompetencies AS FC ON CAQ.CompetencyId = FC.CompetencyId
                    WHERE FC.Id = @frameworkCompetencyId
                     ORDER BY CAQ.Ordering",
                new
                {
                    frameworkCompetencyId,
                    adminId,
                }
            );
        }

        public void AddCompetencyAssessmentQuestion(int frameworkCompetencyId, int assessmentQuestionId, int adminId)
        {
            if ((frameworkCompetencyId < 1) | (adminId < 1) | (assessmentQuestionId < 1))
            {
                logger.LogWarning(
                    $"Not inserting competency assessment question as it failed server side validation. AdminId: {adminId}, frameworkCompetencyId: {frameworkCompetencyId}, assessmentQuestionId: {assessmentQuestionId}"
                );
                return;
            }

            var numberOfAffectedRows = connection.Execute(
                @"INSERT INTO CompetencyAssessmentQuestions (CompetencyId, AssessmentQuestionID, Ordering)
                      SELECT CompetencyID, @assessmentQuestionId, COALESCE
                             ((SELECT        MAX(Ordering)
                                 FROM            [CompetencyAssessmentQuestions]
                                 WHERE        ([CompetencyId] = fc.CompetencyID)), 0)+1
                        FROM FrameworkCompetencies AS fc
                        WHERE Id = @frameworkCompetencyId
                        AND NOT EXISTS (
                            SELECT 1
                            FROM CompetencyAssessmentQuestions AS caq
                            WHERE caq.CompetencyId = fc.CompetencyID
                            AND caq.AssessmentQuestionID = @assessmentQuestionId
                        );",
                new { frameworkCompetencyId, assessmentQuestionId }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not inserting competency assessment question as db update failed. " +
                    $"frameworkCompetencyId: {frameworkCompetencyId}, assessmentQuestionId: {assessmentQuestionId}"
                );
            }
        }

        public void DeleteCompetencyAssessmentQuestion(int frameworkCompetencyId, int assessmentQuestionId, int adminId)
        {
            if ((frameworkCompetencyId < 1) | (adminId < 1) | (assessmentQuestionId < 1))
            {
                logger.LogWarning(
                    $"Not deleting competency assessment question as it failed server side validation. AdminId: {adminId}, frameworkCompetencyId: {frameworkCompetencyId}, assessmentQuestionId: {assessmentQuestionId}"
                );
                return;
            }

            var numberOfAffectedRows = connection.Execute(
                @"DELETE FROM CompetencyAssessmentQuestions
                    FROM   CompetencyAssessmentQuestions INNER JOIN
                        FrameworkCompetencies AS FC ON CompetencyAssessmentQuestions.CompetencyID = FC.CompetencyID
                    WHERE (FC.ID = @frameworkCompetencyId AND CompetencyAssessmentQuestions.AssessmentQuestionID = @assessmentQuestionId)",
                new { frameworkCompetencyId, assessmentQuestionId }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not deleting competency assessment question as db update failed. " +
                    $"frameworkCompetencyId: {frameworkCompetencyId}, assessmentQuestionId: {assessmentQuestionId}"
                );
            }
        }

        public AssessmentQuestionDetail GetAssessmentQuestionDetailById(int assessmentQuestionId, int adminId)
        {
            return connection.QueryFirstOrDefault<AssessmentQuestionDetail>(
                $@"{AssessmentQuestionFields}{AssessmentQuestionDetailFields}
                    {AssessmentQuestionTables}
                    WHERE AQ.ID = @assessmentQuestionId",
                new { adminId, assessmentQuestionId }
            );
        }

        public LevelDescriptor GetLevelDescriptorForAssessmentQuestionId(
            int assessmentQuestionId,
            int adminId,
            int level
        )
        {
            return connection.QueryFirstOrDefault<LevelDescriptor>(
                @"SELECT COALESCE(ID,0) AS ID, @assessmentQuestionId AS AssessmentQuestionID, n AS LevelValue, LevelLabel, LevelDescription, COALESCE(UpdatedByAdminID, @adminId) AS UpdatedByAdminID
                    FROM
                    (SELECT TOP (@level) n = ROW_NUMBER() OVER (ORDER BY number)
                    FROM [master]..spt_values) AS q1
                    LEFT OUTER JOIN AssessmentQuestionLevels AS AQL ON q1.n = AQL.LevelValue AND AQL.AssessmentQuestionID = @assessmentQuestionId
                    WHERE (q1.n = @level)",
                new { assessmentQuestionId, adminId, level }
            );
        }

        public IEnumerable<LevelDescriptor> GetLevelDescriptorsForAssessmentQuestionId(
            int assessmentQuestionId,
            int adminId,
            int minValue,
            int maxValue,
            bool zeroBased
        )
        {
            var adjustBy = zeroBased ? 1 : 0;
            return connection.Query<LevelDescriptor>(
                @"SELECT COALESCE(ID,0) AS ID, @assessmentQuestionId AS AssessmentQuestionID, n AS LevelValue, LevelLabel, LevelDescription, COALESCE(UpdatedByAdminID, @adminId) AS UpdatedByAdminID
                    FROM
                    (SELECT TOP (@maxValue + @adjustBy) n = ROW_NUMBER() OVER (ORDER BY number) - @adjustBy
                    FROM [master]..spt_values) AS q1
                    LEFT OUTER JOIN AssessmentQuestionLevels AS AQL ON q1.n = AQL.LevelValue AND AQL.AssessmentQuestionID = @assessmentQuestionId
                    WHERE (q1.n BETWEEN @minValue AND @maxValue)",
                new { assessmentQuestionId, adminId, minValue, maxValue, adjustBy }
            );
        }

        public int InsertAssessmentQuestion(
            string question,
            int assessmentQuestionInputTypeId,
            string? maxValueDescription,
            string? minValueDescription,
            string? scoringInstructions,
            int minValue,
            int maxValue,
            bool includeComments,
            int adminId,
            string? commentsPrompt,
            string? commentsHint
        )
        {
            if ((question == null) | (adminId < 1))
            {
                logger.LogWarning(
                    $"Not inserting assessment question as it failed server side validation. AdminId: {adminId}, question: {question}"
                );
                return 0;
            }

            var id = connection.QuerySingle<int>(
                @"INSERT INTO AssessmentQuestions (Question, AssessmentQuestionInputTypeID, MaxValueDescription, MinValueDescription, ScoringInstructions, MinValue, MaxValue, IncludeComments, AddedByAdminId, CommentsPrompt, CommentsHint)
                      OUTPUT INSERTED.Id
                      VALUES (@question, @assessmentQuestionInputTypeId, @maxValueDescription, @minValueDescription, @scoringInstructions, @minValue, @maxValue, @includeComments, @adminId, @commentsPrompt, @commentsHint)",
                new
                {
                    question,
                    assessmentQuestionInputTypeId,
                    maxValueDescription,
                    minValueDescription,
                    scoringInstructions,
                    minValue,
                    maxValue,
                    includeComments,
                    adminId,
                    commentsPrompt,
                    commentsHint,
                }
            );
            if (id < 1)
            {
                logger.LogWarning(
                    "Not inserting assessment question as db update failed. " +
                    $"question: {question}, adminId: {adminId}"
                );
                return 0;
            }

            return id;
        }

        public void InsertLevelDescriptor(
            int assessmentQuestionId,
            int levelValue,
            string levelLabel,
            string? levelDescription,
            int adminId
        )
        {
            if ((assessmentQuestionId < 1) | (adminId < 1) | (levelValue < 0))
            {
                logger.LogWarning(
                    $"Not inserting assessment question level descriptor as it failed server side validation. AdminId: {adminId}, assessmentQuestionId: {assessmentQuestionId}, levelValue: {levelValue}"
                );
            }

            var numberOfAffectedRows = connection.Execute(
                @"INSERT INTO AssessmentQuestionLevels
                    (AssessmentQuestionID
                      ,LevelValue
                      ,LevelLabel
                      ,LevelDescription
                      ,UpdatedByAdminID)
                      VALUES (@assessmentQuestionId, @levelValue, @levelLabel, @levelDescription, @adminId)",
                new { assessmentQuestionId, levelValue, levelLabel, levelDescription, adminId }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not inserting assessment question level descriptor as db update failed. " +
                    $"AdminId: {adminId}, assessmentQuestionId: {assessmentQuestionId}, levelValue: {levelValue}"
                );
            }
        }

        public void UpdateAssessmentQuestion(
            int id,
            string question,
            int assessmentQuestionInputTypeId,
            string? maxValueDescription,
            string? minValueDescription,
            string? scoringInstructions,
            int minValue,
            int maxValue,
            bool includeComments,
            int adminId,
            string? commentsPrompt,
            string? commentsHint
        )
        {
            if ((id < 1) | (question == null) | (adminId < 1))
            {
                logger.LogWarning(
                    $"Not updating assessment question as it failed server side validation. Id: {id}, AdminId: {adminId}, question: {question}"
                );
            }

            var numberOfAffectedRows = connection.Execute(
                @"UPDATE AssessmentQuestions
                    SET Question = @question
                        ,AssessmentQuestionInputTypeID = @assessmentQuestionInputTypeId
                        ,MaxValueDescription = @maxValueDescription
                        ,MinValueDescription = @minValueDescription
                        ,ScoringInstructions = @scoringInstructions
                        ,MinValue = @minValue
                        ,MaxValue = @maxValue
                        ,IncludeComments = @includeComments
                        ,AddedByAdminId = @adminId
                        ,CommentsPrompt = @commentsPrompt
                        ,CommentsHint = @commentsHint
                    WHERE ID = @id",
                new
                {
                    id,
                    question,
                    assessmentQuestionInputTypeId,
                    maxValueDescription,
                    minValueDescription,
                    scoringInstructions,
                    minValue,
                    maxValue,
                    includeComments,
                    adminId,
                    commentsPrompt,
                    commentsHint,
                }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating assessment question as db update failed. " +
                    $"Id: {id}, AdminId: {adminId}, question: {question}"
                );
            }
        }

        public void UpdateLevelDescriptor(
            int id,
            int levelValue,
            string levelLabel,
            string? levelDescription,
            int adminId
        )
        {
            if ((id < 1) | (adminId < 1) | (levelValue < 0))
            {
                logger.LogWarning(
                    $"Not updating assessment question level descriptor as it failed server side validation. Id: {id}, AdminId: {adminId}, levelValue: {levelValue}"
                );
            }

            var numberOfAffectedRows = connection.Execute(
                @"UPDATE AssessmentQuestionLevels
                    SET LevelValue = @levelValue
                        ,LevelLabel = @levelLabel
                        ,LevelDescription = @levelDescription
                        ,UpdatedByAdminID = @adminId
                    WHERE ID = @id",
                new { id, levelValue, levelLabel, levelDescription, adminId }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating assessment question level descriptor as db update failed. " +
                    $"Id: {id}, AdminId: {adminId}, levelValue: {levelValue}"
                );
            }
        }

        public Competency? GetFrameworkCompetencyForPreview(int frameworkCompetencyId)
        {
            Competency? competencyResult = null;
            return connection.Query<Competency, Models.SelfAssessments.AssessmentQuestion, Competency>(
                @"SELECT C.ID       AS Id,
                                                  C.Name AS Name,
                                                  C.Description AS Description,
                                                  CG.Name       AS CompetencyGroup,
                                                  CG.ID AS CompetencyGroupID,
                                                  AQ.ID         AS Id,
                                                  AQ.Question,
                                                  AQ.MaxValueDescription,
                                                  AQ.MinValueDescription,
                                                  AQ.ScoringInstructions,
                                                  AQ.MinValue,
                                                  AQ.MaxValue,
                                                  CAQ.Required,
                                                  AQ.AssessmentQuestionInputTypeID,
                                                  AQ.IncludeComments,
                                                  AQ.MinValue AS Result,
                                                  AQ.CommentsPrompt,
                                                  AQ.CommentsHint
                                                  FROM   Competencies AS C INNER JOIN
             FrameworkCompetencies AS FC ON C.ID = FC.CompetencyID LEFT JOIN
             FrameworkCompetencyGroups AS FCG ON FC.FrameworkCompetencyGroupID = FCG.ID LEFT JOIN
             CompetencyGroups AS CG ON FCG.CompetencyGroupID = CG.ID INNER JOIN
             CompetencyAssessmentQuestions AS CAQ ON C.ID = CAQ.CompetencyID INNER JOIN
             AssessmentQuestions AS AQ ON CAQ.AssessmentQuestionID = AQ.ID
             WHERE (FC.ID = @frameworkCompetencyId)
             ORDER BY CAQ.Ordering",
                (competency, assessmentQuestion) =>
                {
                    if (competencyResult == null)
                    {
                        competencyResult = competency;
                    }

                    competencyResult.AssessmentQuestions.Add(assessmentQuestion);
                    return competencyResult;
                },
                new { frameworkCompetencyId }
            ).FirstOrDefault();
        }

        public int GetAdminUserRoleForFrameworkId(int adminId, int frameworkId)
        {
            return connection.QuerySingle<int>(
                @"SELECT CASE WHEN FW.OwnerAdminID = @adminId THEN 3 WHEN COALESCE (fwc.CanModify, 0) = 1 THEN 2 WHEN COALESCE (fwc.CanModify, 0) = 0 THEN 1 ELSE 0 END AS UserRole
                    FROM   Frameworks AS FW LEFT OUTER JOIN
                                 FrameworkCollaborators AS fwc ON fwc.FrameworkID = FW.ID AND fwc.AdminID = @adminId AND fwc.IsDeleted = 0
                    WHERE (FW.ID = @frameworkId)",
                new { adminId, frameworkId }
            );
        }

        public IEnumerable<CommentReplies> GetCommentsForFrameworkId(int frameworkId, int adminId)
        {
            var result = connection.Query<CommentReplies>(
                @$"SELECT
                    {FrameworksCommentColumns}
                    FROM FrameworkComments
                    WHERE Archived Is NULL AND ReplyToFrameworkCommentID Is NULL AND FrameworkID = @frameworkId",
                new { frameworkId, adminId }
            );
            foreach (var comment in result)
            {
                var replies = GetRepliesForCommentId(comment.ID, adminId);
                foreach (var reply in replies)
                {
                    comment.Replies.Add(reply);
                }
            }

            return result;
        }

        public CommentReplies? GetCommentRepliesById(int commentId, int adminId)
        {
            var result = connection.Query<CommentReplies>(
                @$"SELECT
                        {FrameworksCommentColumns}
                    FROM FrameworkComments
                    WHERE Archived Is NULL AND ReplyToFrameworkCommentID Is NULL AND ID = @commentId",
                new { commentId, adminId }
            ).FirstOrDefault();
            var replies = GetRepliesForCommentId(commentId, adminId);
            foreach (var reply in replies)
            {
                result?.Replies.Add(reply);
            }

            return result;
        }

        public int InsertComment(int frameworkId, int adminId, string comment, int? replyToCommentId)
        {
            if ((frameworkId < 1) | (adminId < 1) | (comment == null))
            {
                logger.LogWarning(
                    $"Not inserting assessment question level descriptor as it failed server side validation. AdminId: {adminId}, frameworkId: {frameworkId}, comment: {comment}"
                );
            }

            var commentId = connection.ExecuteScalar<int>(
                @"INSERT INTO FrameworkComments
                     (AdminID
           ,ReplyToFrameworkCommentID
           ,Comments
           ,FrameworkID)
                      VALUES (@adminId, @replyToCommentId, @comment, @frameworkId);
                      SELECT CAST(SCOPE_IDENTITY() as int)",
                new { adminId, replyToCommentId, comment, frameworkId }
            );
            if (commentId < 1)
            {
                logger.LogWarning(
                    "Not inserting framework comment as db insert failed. " +
                    $"AdminId: {adminId}, frameworkId: {frameworkId}, comment: {comment}."
                );
            }

            return commentId;
        }

        public void ArchiveComment(int commentId)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE FrameworkComments
                    SET Archived = GETUTCDATE()
                    WHERE ID = @commentId",
                new { commentId }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not archiving framework comment as db update failed. " +
                    $"commentId: {commentId}."
                );
            }
        }

        public string? GetFrameworkConfigForFrameworkId(int frameworkId)
        {
            return (string?)connection.ExecuteScalar(
                @"SELECT FrameworkConfig
                FROM Frameworks
                WHERE ID = @frameworkId",
                new { frameworkId }
            );
        }

        public CollaboratorNotification? GetCollaboratorNotification(int id, int invitedByAdminId)
        {
            return connection.Query<CollaboratorNotification>(
                @"SELECT
                        fc.FrameworkID,
                        fc.AdminID,
                        fc.CanModify,
                        fc.UserEmail,
                        au.Active AS UserActive,
                        CASE WHEN fc.CanModify = 1 THEN 'Contributor' ELSE 'Reviewer' END AS FrameworkRole,
                        f.FrameworkName,
                        (SELECT Forename + ' ' + Surname + (CASE WHEN Active = 1 THEN '' ELSE ' (Inactive)' END) AS Expr1 FROM AdminUsers AS au1 WHERE (AdminID = @invitedByAdminId)) AS InvitedByName,
                        (SELECT Email FROM AdminUsers AS au2 WHERE (AdminID = @invitedByAdminId)) AS InvitedByEmail
                    FROM FrameworkCollaborators AS fc
                    INNER JOIN Frameworks AS f ON fc.FrameworkID = f.ID
                    INNER JOIN AdminUsers AS au ON fc.AdminID = au.AdminID
                    WHERE (fc.ID = @id) AND (fc.IsDeleted=0)",
                new { invitedByAdminId, id }
            ).FirstOrDefault();
        }

        public List<Recipient> GetCommentRecipients(int frameworkId, int adminId, int? replyToCommentId)
        {
            return connection.Query<Recipient>(
                @"SELECT au.Email, au.Forename AS FirstName, au.Surname AS LastName, CAST(0 AS bit) AS Owner, CAST(0 AS bit) AS Sender
                    FROM   FrameworkComments AS fwc INNER JOIN
                         AdminUsers AS au ON fwc.AdminID = au.AdminID INNER JOIN
                         Frameworks AS fw1 ON fwc.FrameworkID = fw1.ID AND fwc.AdminID <> fw1.OwnerAdminID
                    WHERE (fwc.FrameworkID = @frameworkId) AND (fwc.AdminID <> @adminID) AND (fwc.ReplyToFrameworkCommentID = @replyToCommentId)
                    GROUP BY fwc.FrameworkID, fwc.AdminID, au.Email, au.Forename, au.Surname
                    UNION
                    SELECT au1.Email, au1.Forename, au1.Surname, CAST(1 AS bit) AS Owner, CAST(0 AS bit) AS Sender
                    FROM   Frameworks AS fw INNER JOIN
                         AdminUsers AS au1 ON fw.OwnerAdminID = au1.AdminID AND au1.AdminID <> @adminId
                    WHERE (fw.ID = @frameworkId)
                    UNION
                    SELECT Email, Forename, Surname, CAST(0 AS bit) AS Owner, CAST(1 AS bit) AS Sender
                    FROM   AdminUsers AS au2
                    WHERE (AdminID = @adminId)
                    ORDER BY Sender Desc",
                new { frameworkId, adminId, replyToCommentId }
            ).ToList();
        }

        public Comment? GetCommentById(int adminId, int commentId)
        {
            return connection.Query<Comment>(
                @"SELECT ID, ReplyToFrameworkCommentID, AdminID, CAST(CASE WHEN AdminID = @adminId THEN 1 ELSE 0 END AS Bit) AS UserIsCommenter, AddedDate, Comments, Archived, LastEdit, FrameworkID
FROM   FrameworkComments
WHERE (ID = @commentId)",
                new { adminId, commentId }
            ).FirstOrDefault();
        }

        public IEnumerable<CollaboratorDetail> GetReviewersForFrameworkId(int frameworkId)
        {
            return connection.Query<CollaboratorDetail>(
                @"SELECT
                        fc.ID,
                        fc.FrameworkID,
                        fc.AdminID,
                        fc.CanModify,
                        fc.UserEmail,
                        au.Active AS UserActive,
                        CASE WHEN CanModify = 1 THEN 'Contributor' ELSE 'Reviewer' END AS FrameworkRole
                    FROM FrameworkCollaborators fc
                    INNER JOIN AdminUsers AS au ON fc.AdminID = au.AdminID
                    LEFT OUTER JOIN FrameworkReviews ON fc.ID = FrameworkReviews.FrameworkCollaboratorID
                    WHERE (fc.FrameworkID = @FrameworkID) AND (FrameworkReviews.ID IS NULL) AND (fc.IsDeleted=0) OR
                            (fc.FrameworkID = @FrameworkID) AND (FrameworkReviews.Archived IS NOT NULL) AND (fc.IsDeleted=0)",
                new { frameworkId }
            );
        }

        public void UpdateFrameworkStatus(int frameworkId, int statusId, int adminId)
        {
            connection.Query(
                @"UPDATE Frameworks
                    SET PublishStatusID = @statusId,
                    UpdatedByAdminID = @adminId
                    WHERE ID = @frameworkId",
                new { frameworkId, statusId, adminId }
            );
        }

        public void InsertFrameworkReview(int frameworkId, int frameworkCollaboratorId, bool required)
        {
            var exists = (int?)connection.ExecuteScalar(
                @"SELECT COUNT(*)
                    FROM FrameworkReviews
                    WHERE FrameworkID = @frameworkId
                    AND FrameworkCollaboratorId = @frameworkCollaboratorId AND Archived IS NULL",
                new { frameworkId, frameworkCollaboratorId }
            );
            if (exists == 0)
            {
                connection.Query(
                    @"INSERT INTO FrameworkReviews
                    (FrameworkID, FrameworkCollaboratorId, SignOffRequired)
                    VALUES
                    (@frameworkId, @frameworkCollaboratorId, @required)",
                    new { frameworkId, frameworkCollaboratorId, required }
                );
            }
        }

        public IEnumerable<FrameworkReview> GetFrameworkReviewsForFrameworkId(int frameworkId)
        {
            return connection.Query<FrameworkReview>(
                @"SELECT FR.ID, FR.FrameworkID, FR.FrameworkCollaboratorID, FC.UserEmail, CAST(CASE WHEN FC.AdminID IS NULL THEN 0 ELSE 1 END AS bit) AS IsRegistered, FR.ReviewRequested, FR.ReviewComplete, FR.SignedOff, FR.FrameworkCommentID, FC1.Comments AS Comment, FR.SignOffRequired
                    FROM   FrameworkReviews AS FR INNER JOIN
                         FrameworkCollaborators AS FC ON FR.FrameworkCollaboratorID = FC.ID LEFT OUTER JOIN
                         FrameworkComments AS FC1 ON FR.FrameworkCommentID = FC1.ID
                    WHERE FR.FrameworkID = @frameworkId AND FR.Archived IS NULL  AND (FC.IsDeleted = 0)",
                new { frameworkId }
            );
        }

        public FrameworkReview? GetFrameworkReview(int frameworkId, int adminId, int reviewId)
        {
            return connection.Query<FrameworkReview>(
                @"SELECT FR.ID, FR.FrameworkID, FR.FrameworkCollaboratorID, FC.UserEmail, CAST(CASE WHEN FC.AdminID IS NULL THEN 0 ELSE 1 END AS bit) AS IsRegistered, FR.ReviewRequested, FR.ReviewComplete, FR.SignedOff, FR.FrameworkCommentID, FC1.Comments AS Comment, FR.SignOffRequired
                    FROM   FrameworkReviews AS FR INNER JOIN
                         FrameworkCollaborators AS FC ON FR.FrameworkCollaboratorID = FC.ID LEFT OUTER JOIN
                         FrameworkComments AS FC1 ON FR.FrameworkCommentID = FC1.ID
                    WHERE FR.ID = @reviewId AND FR.FrameworkID = @frameworkId AND FC.AdminID = @adminId AND FR.Archived IS NULL AND IsDeleted = 0",
                new { frameworkId, adminId, reviewId }
            ).FirstOrDefault();
        }

        public void SubmitFrameworkReview(int frameworkId, int reviewId, bool signedOff, int? commentId)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE FrameworkReviews
                    SET ReviewComplete = GETUTCDATE(), FrameworkCommentID = @commentId, SignedOff = @signedOff
                    WHERE ID = @reviewId AND FrameworkID = @frameworkId",
                new { reviewId, commentId, signedOff, frameworkId }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not submitting framework review as db update failed. " +
                    $"commentId: {commentId}, frameworkId: {frameworkId}, reviewId: {reviewId}, signedOff: {signedOff} ."
                );
            }
        }

        public FrameworkReviewOutcomeNotification? GetFrameworkReviewNotification(int reviewId)
        {
            return connection.Query<FrameworkReviewOutcomeNotification>(
                @"SELECT
                        FR.ID,
                        FR.FrameworkID,
                        FR.FrameworkCollaboratorID,
                        FC.UserEmail,
                        CAST(CASE WHEN FC.AdminID IS NULL THEN 0 ELSE 1 END AS bit) AS IsRegistered,
                        FR.ReviewRequested,
                        FR.ReviewComplete,
                        FR.SignedOff,
                        FR.FrameworkCommentID,
                        FC1.Comments AS Comment,
                        FR.SignOffRequired,
                        AU.Forename AS ReviewerFirstName,
                        AU.Surname AS ReviewerLastName,
                        AU.Active AS ReviewerActive,
                        AU1.Forename AS OwnerFirstName,
                        AU1.Email AS OwnerEmail,
                        FW.FrameworkName
                    FROM FrameworkReviews AS FR
                    INNER JOIN FrameworkCollaborators AS FC ON FR.FrameworkCollaboratorID = FC.ID  AND FWC.IsDeleted = 0
                    INNER JOIN AdminUsers AS AU ON FC.AdminID = AU.AdminID
                    INNER JOIN Frameworks AS FW ON FR.FrameworkID = FW.ID
                    INNER JOIN AdminUsers AS AU1 ON FW.OwnerAdminID = AU1.AdminID
                    LEFT OUTER JOIN FrameworkComments AS FC1 ON FR.FrameworkCommentID = FC1.ID
                    WHERE (FR.ID = @reviewId) AND (FR.ReviewComplete IS NOT NULL)",
                new { reviewId }
            ).FirstOrDefault();
        }

        public void UpdateReviewRequestedDate(int reviewId)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE FrameworkReviews
                    SET ReviewRequested = GETUTCDATE()
                    WHERE ID = @reviewId",
                new { reviewId }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating framework review requested date as db update failed. " +
                    $"reviewId: {reviewId}."
                );
            }
        }

        public int InsertFrameworkReReview(int reviewId)
        {
            ArchiveReviewRequest(reviewId);
            var id = connection.QuerySingle<int>(
                @"INSERT INTO FrameworkReviews
                    (FrameworkID, FrameworkCollaboratorId, SignOffRequired)
                      OUTPUT INSERTED.ID
                      SELECT FR1.FrameworkID, FR1.FrameworkCollaboratorId, FR1.SignOffRequired FROM FrameworkReviews AS FR1 WHERE FR1.ID = @reviewId",
                new { reviewId }
            );
            if (id < 1)
            {
                logger.LogWarning(
                    "Not inserting assessment question as db update failed. " +
                    $"reviewId: {reviewId}"
                );
                return 0;
            }

            return id;
        }

        public void ArchiveReviewRequest(int reviewId)
        {
            var numberOfAffectedRows = connection.Execute(
                @"UPDATE FrameworkReviews
                    SET Archived = GETUTCDATE()
                    WHERE ID = @reviewId",
                new { reviewId }
            );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not archiving framework review as db update failed. " +
                    $"reviewId: {reviewId}."
                );
            }
        }

        public DashboardData? GetDashboardDataForAdminID(int adminId)
        {
            return connection.Query<DashboardData>(
                $@"SELECT (SELECT COUNT(*)
  FROM [dbo].[Frameworks]) AS FrameworksCount,

  (SELECT COUNT(*) FROM {FrameworkTables}
WHERE (OwnerAdminID = @adminId) OR
             (@adminId IN
                 (SELECT AdminID
                 FROM    FrameworkCollaborators
                 WHERE (FrameworkID = FW.ID) AND (IsDeleted=0)))) AS MyFrameworksCount,

                 (SELECT COUNT(*) FROM SelfAssessments) AS RoleProfileCount,

                 (SELECT COUNT(*) FROM SelfAssessments AS RP LEFT OUTER JOIN
             SelfAssessmentCollaborators AS RPC ON RPC.SelfAssessmentID = RP.ID AND RPC.AdminID = @adminId
WHERE (RP.CreatedByAdminID = @adminId) OR
             (@adminId IN
                 (SELECT AdminID
                 FROM    SelfAssessmentCollaborators
                 WHERE (SelfAssessmentID = RP.ID)))) AS MyRoleProfileCount",
                new { adminId }
            ).FirstOrDefault();
        }

        public IEnumerable<DashboardToDoItem> GetDashboardToDoItems(int adminId)
        {
            return connection.Query<DashboardToDoItem>(
                @"SELECT
                        FW.ID AS FrameworkID,
                        0 AS RoleProfileID,
                        FW.FrameworkName AS ItemName,
                        AU.Forename + ' ' + AU.Surname + (CASE WHEN AU.Active = 1 THEN '' ELSE ' (Inactive)' END) AS RequestorName,
                        FWR.SignOffRequired,
                        FWR.ReviewRequested AS Requested
                    FROM FrameworkReviews AS FWR
                    INNER JOIN Frameworks AS FW ON FWR.FrameworkID = FW.ID
                    INNER JOIN FrameworkCollaborators AS FWC ON FWR.FrameworkCollaboratorID = FWC.ID AND FWC.IsDeleted = 0
                    INNER JOIN AdminUsers AS AU ON FW.OwnerAdminID = AU.AdminID
                    WHERE (FWC.AdminID = @adminId) AND (FWR.ReviewComplete IS NULL) AND (FWR.Archived IS NULL)
                    UNION ALL
                    SELECT
                        0 AS SelfAssessmentID,
                        RP.ID AS SelfAssessmentID,
                        RP.Name AS ItemName,
                        AU.Forename + ' ' + AU.Surname + (CASE WHEN AU.Active = 1 THEN '' ELSE ' (Inactive)' END) AS RequestorName,
                        RPR.SignOffRequired,
                        RPR.ReviewRequested AS Requested
                    FROM SelfAssessmentReviews AS RPR
                    INNER JOIN SelfAssessments AS RP ON RPR.SelfAssessmentID = RP.ID
                    INNER JOIN SelfAssessmentCollaborators AS RPC ON RPR.SelfAssessmentCollaboratorID = RPC.ID
                    INNER JOIN AdminUsers AS AU ON RP.CreatedByAdminID = AU.AdminID
                    WHERE (RPC.AdminID = @adminId) AND (RPR.ReviewComplete IS NULL) AND (RPR.Archived IS NULL)",
                new { adminId }
            );
        }

        public void MoveCompetencyAssessmentQuestion(
            int competencyId,
            int assessmentQuestionId,
            bool singleStep,
            string direction
        )
        {
            connection.Execute(
                "ReorderCompetencyAssessmentQuestion",
                new { competencyId, assessmentQuestionId, direction, singleStep },
                commandType: CommandType.StoredProcedure
            );
        }

        public int GetMaxFrameworkCompetencyID()
        {
            return connection.Query<int>(
                "SELECT MAX(ID) FROM FrameworkCompetencies"
            ).Single();
        }

        public int GetMaxFrameworkCompetencyGroupID()
        {
            return connection.Query<int>(
                "SELECT MAX(ID) FROM FrameworkCompetencyGroups"
            ).Single();
        }

        public CompetencyResourceAssessmentQuestionParameter?
            GetCompetencyResourceAssessmentQuestionParameterByCompetencyLearningResourceId(
                int competencyLearningResourceId
            )
        {
            var resource = connection.Query<CompetencyResourceAssessmentQuestionParameter>(
                @"SELECT p.AssessmentQuestionId, clr.ID AS CompetencyLearningResourceId, p.MinResultMatch, p.MaxResultMatch, p.Essential,
                          p.RelevanceAssessmentQuestionId, p.CompareToRoleRequirements, lrr.OriginalResourceName,
                    CASE
                        WHEN p.CompetencyLearningResourceId IS NULL THEN 1
                        ELSE 0
                    END AS IsNew
                    FROM CompetencyLearningResources AS clr
                    INNER JOIN LearningResourceReferences AS lrr ON clr.LearningResourceReferenceID = lrr.ID
                    LEFT OUTER JOIN CompetencyResourceAssessmentQuestionParameters AS p ON p.CompetencyLearningResourceID = clr.ID
                    WHERE clr.ID = @competencyLearningResourceId",
                new { competencyLearningResourceId }
            ).FirstOrDefault();
            var questions = connection.Query<AssessmentQuestion>(
                $@"SELECT * FROM AssessmentQuestions
                    WHERE ID IN ({resource?.AssessmentQuestionId ?? 0}, {resource?.RelevanceAssessmentQuestionId ?? 0})"
            );
            resource!.AssessmentQuestion = questions.FirstOrDefault(q => q.ID == resource.AssessmentQuestionId)!;
            resource.RelevanceAssessmentQuestion =
                questions.FirstOrDefault(q => q.ID == resource.RelevanceAssessmentQuestionId)!;
            return resource;
        }

        public IEnumerable<CompetencyResourceAssessmentQuestionParameter>
            GetSignpostingResourceParametersByFrameworkAndCompetencyId(int frameworkId, int competencyId)
        {
            return connection.Query<CompetencyResourceAssessmentQuestionParameter>(
                @"SELECT clr.ID AS CompetencyLearningResourceID, lrr.ResourceRefID AS ResourceRefId, lrr.OriginalResourceName, lrr.OriginalResourceType, lrr.OriginalRating,
                          q.ID AS AssessmentQuestionId, q.Question, q.AssessmentQuestionInputTypeID, q.MinValue AS AssessmentQuestionMinValue, q.MaxValue AS AssessmentQuestionMaxValue,
                          p.Essential, p.MinResultMatch, p.MaxResultMatch,
                    CASE
                        WHEN p.CompareToRoleRequirements = 1 THEN 'Role requirements'
                        WHEN p.RelevanceAssessmentQuestionID IS NOT NULL THEN raq.Question
                        ELSE 'Don''t compare result'
                    END AS CompareResultTo,
                    CASE
                        WHEN p.CompetencyLearningResourceId IS NULL THEN 1
                        ELSE 0
                    END AS IsNew
                    FROM FrameworkCompetencies AS fc
                    INNER JOIN Competencies AS c ON fc.CompetencyID = c.ID
                    INNER JOIN CompetencyLearningResources AS clr ON clr.CompetencyID = c.ID
                    INNER JOIN LearningResourceReferences AS lrr ON clr.LearningResourceReferenceID = lrr.ID
                    LEFT JOIN CompetencyResourceAssessmentQuestionParameters AS p ON p.CompetencyLearningResourceID = clr.ID
                    LEFT JOIN AssessmentQuestions AS q ON p.AssessmentQuestionID = q.ID
                    LEFT JOIN AssessmentQuestions AS raq ON p.RelevanceAssessmentQuestionID = raq.ID
                    WHERE fc.FrameworkID = @FrameworkId AND clr.CompetencyID = @CompetencyId AND clr.RemovedDate IS NULL",
                new { frameworkId, competencyId }
            );
        }

        public LearningResourceReference? GetLearningResourceReferenceByCompetencyLearningResouceId(
            int competencyLearningResouceId
        )
        {
            return connection.Query<LearningResourceReference>(
                @"SELECT * FROM LearningResourceReferences lrr
                    INNER JOIN CompetencyLearningResources clr ON clr.LearningResourceReferenceID = lrr.ID
                    WHERE clr.ID = @competencyLearningResouceId",
                new { competencyLearningResouceId }
            ).FirstOrDefault();
        }

        public int GetCompetencyAssessmentQuestionRoleRequirementsCount(int assessmentQuestionId, int competencyId)
        {
            var count = connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM CompetencyAssessmentQuestionRoleRequirements
                    WHERE AssessmentQuestionID = @assessmentQuestionId AND CompetencyID = @competencyId",
                new { assessmentQuestionId, competencyId }
            );
            return Convert.ToInt32(count);
        }

        public int EditCompetencyResourceAssessmentQuestionParameter(
            CompetencyResourceAssessmentQuestionParameter parameter
        )
        {
            int rowsAffected;
            if (parameter.IsNew)
            {
                rowsAffected = connection.Execute(
                    $@"INSERT INTO CompetencyResourceAssessmentQuestionParameters(
                        CompetencyLearningResourceID,
                        AssessmentQuestionID,
                        MinResultMatch,
                        MaxResultMatch,
                        Essential,
                        RelevanceAssessmentQuestionID,
                        CompareToRoleRequirements)
                        VALUES(
                            {parameter.CompetencyLearningResourceId},
                            {parameter.AssessmentQuestion?.ID.ToString() ?? "null"},
                            {parameter.MinResultMatch},
                            {parameter.MaxResultMatch},
                            {Convert.ToInt32(parameter.Essential)},
                            {parameter.RelevanceAssessmentQuestion?.ID.ToString() ?? "null"},
                            {Convert.ToInt32(parameter.CompareToRoleRequirements)})"
                );
            }
            else
            {
                rowsAffected = connection.Execute(
                    $@"UPDATE CompetencyResourceAssessmentQuestionParameters
                    SET AssessmentQuestionID = {parameter.AssessmentQuestion?.ID.ToString() ?? "null"},
                        MinResultMatch = {parameter.MinResultMatch},
                        MaxResultMatch = {parameter.MaxResultMatch},
                        Essential = {Convert.ToInt32(parameter.Essential)},
                        RelevanceAssessmentQuestionID = {parameter.RelevanceAssessmentQuestion?.ID.ToString() ?? "null"},
                        CompareToRoleRequirements = {Convert.ToInt32(parameter.CompareToRoleRequirements)}
                    WHERE CompetencyLearningResourceID = {parameter.CompetencyLearningResourceId}"
                );
            }

            return rowsAffected;
        }

        public List<Comment> GetRepliesForCommentId(int commentId, int adminId)
        {
            return connection.Query<Comment>(
                @$"SELECT
                        {FrameworksCommentColumns},
                        ReplyToFrameworkCommentID
                    FROM FrameworkComments
                    WHERE Archived Is NULL AND ReplyToFrameworkCommentID = @commentId
                    ORDER BY AddedDate ASC",
                new { commentId, adminId }
            ).ToList();
        }

        public void AddDefaultQuestionsToCompetency(int competencyId, int frameworkId)
        {
            connection.Execute(
                @"INSERT INTO CompetencyAssessmentQuestions (CompetencyID, AssessmentQuestionID, Ordering)
                        SELECT @competencyId AS CompetencyID, AssessmentQuestionId, COALESCE
                             ((SELECT        MAX(Ordering)
                                 FROM            [CompetencyAssessmentQuestions]
                                 WHERE        ([CompetencyId] = @competencyId)), 0)+1 As Ordering
                    FROM   FrameworkDefaultQuestions
                    WHERE (FrameworkId = @frameworkId) AND (NOT EXISTS  (SELECT * FROM CompetencyAssessmentQuestions WHERE AssessmentQuestionID = FrameworkDefaultQuestions.AssessmentQuestionID AND CompetencyID = @competencyId))",
                new { competencyId, frameworkId }
            );
        }

        public IEnumerable<BulkCompetency> GetBulkCompetenciesForFramework(int frameworkId)
        {
            if (frameworkId < 1)
            {
                return connection.Query<BulkCompetency>(
                @"SELECT NULL AS ID, '' AS CompetencyGroup, '' AS GroupDescription, '' AS Competency, '' AS CompetencyDescription, 0 AS AlwaysShowDescription, '' AS FlagsCsv"
            );
            }
            else
            {
                return connection.Query<BulkCompetency>(
                @"SELECT fc.ID, cg.Name AS CompetencyGroup, cg.Description AS GroupDescription, c.Name AS Competency, c.Description AS CompetencyDescription, c.AlwaysShowDescription, STUFF((
                        SELECT ', ' + f.FlagName
                        FROM Flags AS f
                        INNER JOIN CompetencyFlags AS cf ON f.ID = cf.FlagID
                        WHERE cf.CompetencyID = c.ID
                        FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS FlagsCsv
                    FROM 
                         Competencies AS c INNER JOIN
                         FrameworkCompetencies AS fc ON c.ID = fc.CompetencyID INNER JOIN
                         FrameworkCompetencyGroups AS fcg ON fc.FrameworkCompetencyGroupID = fcg.ID INNER JOIN
                         CompetencyGroups AS cg ON fcg.CompetencyGroupID = cg.ID
                    WHERE (fc.FrameworkID = @frameworkId)
                    GROUP BY fc.ID, c.ID, cg.Name, cg.Description, c.Name, c.Description, c.AlwaysShowDescription, fcg.Ordering, fc.Ordering
                    ORDER BY fcg.Ordering, fc.Ordering",
                                new { frameworkId }
                            );
            }

        }
        public List<int> GetFrameworkCompetencyOrder(int frameworkId, List<int> frameworkCompetencyIds)
        {
            return connection.Query<int>(
                @"SELECT fc.ID
                    FROM   FrameworkCompetencies AS fc INNER JOIN
                                 FrameworkCompetencyGroups AS fcg ON fc.FrameworkCompetencyGroupID = fcg.ID
                    WHERE (fc.FrameworkID = @frameworkId) AND (fc.ID IN @frameworkCompetencyIds)
                    ORDER BY fcg.Ordering, fc.Ordering",
                                new { frameworkId, frameworkCompetencyIds }
                            ).ToList();
        }
    }
}
