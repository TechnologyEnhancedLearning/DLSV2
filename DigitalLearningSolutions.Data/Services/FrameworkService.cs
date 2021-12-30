namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Data.Models.Common;
    using Microsoft.Extensions.Logging;
    using DigitalLearningSolutions.Data.Models.Email;

    public interface IFrameworkService
    {
        //GET DATA
        //  Frameworks:
        DashboardData GetDashboardDataForAdminID(int adminId);
        IEnumerable<DashboardToDoItem> GetDashboardToDoItems(int adminId);
        DetailFramework? GetFrameworkDetailByFrameworkId(int frameworkId, int adminId);
        BaseFramework? GetBaseFrameworkByFrameworkId(int frameworkId, int adminId);
        BrandedFramework? GetBrandedFrameworkByFrameworkId(int frameworkId, int adminId);
        DetailFramework? GetDetailFrameworkByFrameworkId(int frameworkId, int adminId);
        IEnumerable<BrandedFramework> GetFrameworkByFrameworkName(string frameworkName, int adminId);
        IEnumerable<BrandedFramework> GetFrameworksForAdminId(int adminId);
        IEnumerable<BrandedFramework> GetAllFrameworks(int adminId);
        int GetAdminUserRoleForFrameworkId(int adminId, int frameworkId);
        string? GetFrameworkConfigForFrameworkId(int frameworkId);
        //  Collaborators:
        IEnumerable<CollaboratorDetail> GetCollaboratorsForFrameworkId(int frameworkId);
        CollaboratorNotification GetCollaboratorNotification(int id, int invitedByAdminId);
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
        IEnumerable<AssessmentQuestion> GetCompetencyAssessmentQuestionsByFrameworkCompetencyId(int frameworkCompetencyId, int adminId);
        IEnumerable<AssessmentQuestion> GetCompetencyAssessmentQuestionsById(int competencyId, int adminId);
        IEnumerable<GenericSelectList> GetAssessmentQuestionInputTypes();
        IEnumerable<GenericSelectList> GetAssessmentQuestions(int frameworkId, int adminId);
        FrameworkDefaultQuestionUsage GetFrameworkDefaultQuestionUsage(int frameworkId, int assessmentQuestionId);
        IEnumerable<GenericSelectList> GetAssessmentQuestionsForCompetency(int frameworkCompetencyId, int adminId);
        AssessmentQuestionDetail GetAssessmentQuestionDetailById(int assessmentQuestionId, int adminId);
        LevelDescriptor GetLevelDescriptorForAssessmentQuestionId(int assessmentQuestionId, int adminId, int level);
        IEnumerable<LevelDescriptor> GetLevelDescriptorsForAssessmentQuestionId(int assessmentQuestionId, int adminId, int minValue, int maxValue, bool zeroBased);
        Models.SelfAssessments.Competency? GetFrameworkCompetencyForPreview(int frameworkCompetencyId);
        //  Comments:
        IEnumerable<CommentReplies> GetCommentsForFrameworkId(int frameworkId, int adminId);
        CommentReplies GetCommentRepliesById(int commentId, int adminId);
        Comment GetCommentById(int adminId, int commentId);
        List<Recipient> GetCommentRecipients(int frameworkId, int adminId, int? replyToCommentId);
        // Reviews:
        IEnumerable<CollaboratorDetail> GetReviewersForFrameworkId(int frameworkId);
        IEnumerable<FrameworkReview> GetFrameworkReviewsForFrameworkId(int frameworkId);
        FrameworkReview? GetFrameworkReview(int frameworkId, int adminId, int reviewId);
        FrameworkReviewOutcomeNotification? GetFrameworkReviewNotification(int reviewId);
        //INSERT DATA
        BrandedFramework CreateFramework(DetailFramework detailFramework, int adminId);
        int InsertCompetencyGroup(string groupName, int adminId);
        int InsertFrameworkCompetencyGroup(int groupId, int frameworkID, int adminId);
        int InsertCompetency(string name, string? description, int adminId);
        int InsertFrameworkCompetency(int competencyId, int? frameworkCompetencyGroupID, int adminId, int frameworkId);
        int AddCollaboratorToFramework(int frameworkId, string userEmail, bool canModify);
        void AddFrameworkDefaultQuestion(int frameworkId, int assessmentQuestionId, int adminId, bool addToExisting);
        CompetencyResourceAssessmentQuestionParameter? GetCompetencyResourceAssessmentQuestionParameterById(int competencyResourceAssessmentQuestionParameterId);
        LearningResourceReference GetLearningResourceReferenceByCompetencyLearningResouceId(int competencyLearningResourceID);
        void AddCompetencyAssessmentQuestion(int frameworkCompetencyId, int assessmentQuestionId, int adminId);
        int InsertAssessmentQuestion(string question, int assessmentQuestionInputTypeId, string? maxValueDescription, string? minValueDescription, string? scoringInstructions, int minValue, int maxValue, bool includeComments, int adminId, string? commentsPrompt, string? commentsHint);
        void InsertLevelDescriptor(int assessmentQuestionId, int levelValue, string levelLabel, string? levelDescription, int adminId);
        int InsertComment(int frameworkId, int adminId, string comment, int? replyToCommentId);
        void InsertFrameworkReview(int frameworkId, int frameworkCollaboratorId, bool required);
        int InsertFrameworkReReview(int reviewId);
        //UPDATE DATA
        BrandedFramework? UpdateFrameworkBranding(int frameworkId, int brandId, int categoryId, int topicId, int adminId);
        bool UpdateFrameworkName(int frameworkId, int adminId, string frameworkName);
        void UpdateFrameworkDescription(int frameworkId, int adminId, string? frameworkDescription);
        void UpdateFrameworkConfig(int frameworkId, int adminId, string? frameworkConfig);
        void UpdateFrameworkCompetencyGroup(int frameworkCompetencyGroupId, int competencyGroupId, string name, int adminId);
        void UpdateFrameworkCompetency(int frameworkCompetencyId, string name, string? description, int adminId);
        void MoveFrameworkCompetencyGroup(int frameworkCompetencyGroupId, bool singleStep, string direction);
        void MoveFrameworkCompetency(int frameworkCompetencyId, bool singleStep, string direction);
        void UpdateAssessmentQuestion(int id, string question, int assessmentQuestionInputTypeId, string? maxValueDescription, string? minValueDescription, string? scoringInstructions, int minValue, int maxValue, bool includeComments, int adminId, string? commentsPrompt, string? commentsHint);
        void UpdateLevelDescriptor(int id, int levelValue, string levelLabel, string? levelDescription, int adminId);
        void ArchiveComment(int commentId);
        void UpdateFrameworkStatus(int frameworkId, int statusId, int adminId);
        void SubmitFrameworkReview(int frameworkId, int reviewId, bool signedOff, int? commentId);
        void UpdateReviewRequestedDate(int reviewId);
        void ArchiveReviewRequest(int reviewId);
        void MoveCompetencyAssessmentQuestion(int competencyId, int assessmentQuestionId, bool singleStep, string direction);
        //Delete data
        void RemoveCollaboratorFromFramework(int frameworkId, int id);
        void DeleteFrameworkCompetencyGroup(int frameworkCompetencyGroupId, int competencyGroupId, int adminId);
        void DeleteFrameworkCompetency(int frameworkCompetencyId, int adminId);
        void DeleteFrameworkDefaultQuestion(int frameworkId, int assessmentQuestionId, int adminId, bool deleteFromExisting);
        void DeleteCompetencyAssessmentQuestion(int frameworkCompetencyId, int assessmentQuestionId, int adminId);
        IEnumerable<SignpostingResourceParameter> GetSignpostingResourceParametersByFrameworkAndCompetencyId(int frameworkId, int competencyId);
    }
    public class FrameworkService : IFrameworkService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<FrameworkService> logger;
        private const string BaseFrameworkFields =
            @"FW.ID, FrameworkName, OwnerAdminID,
                 (SELECT Forename + ' ' + Surname AS Expr1
                 FROM    AdminUsers
                 WHERE (AdminID = FW.OwnerAdminID)) AS Owner, BrandID, CategoryID, TopicID, CreatedDate, PublishStatusID, UpdatedByAdminID,
                 (SELECT Forename + ' ' + Surname AS Expr1
                 FROM    AdminUsers AS AdminUsers_1
                 WHERE (AdminID = FW.UpdatedByAdminID)) AS UpdatedBy, CASE WHEN FW.OwnerAdminID = @adminId THEN 3 WHEN fwc.CanModify = 1 THEN 2 WHEN fwc.CanModify = 0 THEN 1 ELSE 0 END AS UserRole,
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
        private const string FrameworkTables =
            @"Frameworks AS FW LEFT OUTER JOIN
             FrameworkCollaborators AS fwc ON fwc.FrameworkID = FW.ID AND fwc.AdminID = @adminId
LEFT OUTER JOIN FrameworkReviews AS fwr ON fwc.ID = fwr.FrameworkCollaboratorID AND fwr.Archived IS NULL AND fwr.ReviewComplete IS NULL";
        private const string AssessmentQuestionFields =
            @"SELECT AQ.ID, AQ.Question, AQ.MinValue, AQ.MaxValue, AQ.AssessmentQuestionInputTypeID, AQI.InputTypeName, AQ.AddedByAdminId, CASE WHEN AQ.AddedByAdminId = @adminId THEN 1 ELSE 0 END AS UserIsOwner, AQ.CommentsPrompt, AQ.CommentsHint";
        private const string AssessmentQuestionDetailFields =
            @", AQ.MinValueDescription, AQ.MaxValueDescription, AQ.IncludeComments, AQ.ScoringInstructions ";
        private const string AssessmentQuestionTables =
            @"FROM AssessmentQuestions AS AQ LEFT OUTER JOIN AssessmentQuestionInputTypes AS AQI ON AQ.AssessmentQuestionInputTypeID = AQI.ID ";
        public FrameworkService(IDbConnection connection, ILogger<FrameworkService> logger)
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
                 WHERE (FrameworkID = FW.ID)))",
               new { adminId }
           );
        }
        public IEnumerable<BrandedFramework> GetAllFrameworks(int adminId)
        {
            return connection.Query<BrandedFramework>(
                $@"SELECT {BaseFrameworkFields} {BrandedFrameworkFields}
                      FROM {FrameworkTables}", new { adminId }
           );
        }

        public BrandedFramework CreateFramework(DetailFramework detailFramework, int adminId)
        {
            string frameworkName = detailFramework.FrameworkName;
            string? description = detailFramework.Description;
            string? frameworkConfig = detailFramework.FrameworkConfig;
            int? brandId = detailFramework.BrandID;
            int? categoryId = detailFramework.CategoryID;
            int? topicId = detailFramework.TopicID;
            if (detailFramework.FrameworkName.Length == 0 | adminId < 1)
            {
                logger.LogWarning(
                    $"Not inserting framework as it failed server side validation. AdminId: {adminId}, frameworkName: {frameworkName}"
                );
                return new BrandedFramework();
            }
            int existingFrameworks = (int)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM Frameworks WHERE FrameworkName = @frameworkName",
                new { frameworkName });
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
        public BrandedFramework? UpdateFrameworkBranding(int frameworkId, int brandId, int categoryId, int topicId, int adminId)
        {
            if (frameworkId < 1 | brandId < 1 | categoryId < 1 | topicId < 1 | adminId < 1)
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
        public int InsertCompetencyGroup(string groupName, int adminId)
        {
            if (groupName.Length == 0 | adminId < 1)
            {
                logger.LogWarning(
                    $"Not inserting competency group as it failed server side validation. AdminId: {adminId}, GroupName: {groupName}"
                );
                return -2;
            }
            int existingId = (int)connection.ExecuteScalar(
                @"SELECT COALESCE ((SELECT ID FROM CompetencyGroups WHERE [Name] = @groupName), 0) AS CompetencyGroupID",
                new { groupName });
            if (existingId > 0)
            {
                return existingId;
            }
            else
            {
                var numberOfAffectedRows = connection.Execute(
                              @"INSERT INTO CompetencyGroups ([Name], UpdatedByAdminID)
                    VALUES (@groupName, @adminId)",
                             new { groupName, adminId });
                if (numberOfAffectedRows < 1)
                {
                    logger.LogWarning(
                        "Not inserting competency group as db insert failed. " +
                        $"Group name: {groupName}, admin id: {adminId}"
                    );
                    return -1;
                }
                existingId = (int)connection.ExecuteScalar(
                 @"SELECT COALESCE ((SELECT ID FROM CompetencyGroups WHERE [Name] = @groupName), 0) AS CompetencyGroupID",
                 new { groupName });
                return existingId;
            }
        }
        public int InsertFrameworkCompetencyGroup(int groupId, int frameworkId, int adminId)
        {
            if (groupId < 1 | frameworkId < 1 | adminId < 1)
            {
                logger.LogWarning(
                    $"Not inserting framework competency group as it failed server side validation. AdminId: {adminId}, frameworkId: {frameworkId}, groupId: {groupId}"
                );
                return -2;
            }
            int existingId = (int)connection.ExecuteScalar(
                 @"SELECT COALESCE ((SELECT ID FROM FrameworkCompetencyGroups WHERE CompetencyGroupID = @groupID AND FrameworkID = @frameworkID), 0) AS FrameworkCompetencyGroupID",
                 new { groupId, frameworkId });
            if (existingId > 0)
            {
                return existingId;
            }
            else
            {
                var numberOfAffectedRows = connection.Execute(
                              @"INSERT INTO FrameworkCompetencyGroups (CompetencyGroupID, UpdatedByAdminID, Ordering, FrameworkID)
                    VALUES (@groupId, @adminId, COALESCE
                             ((SELECT        MAX(Ordering)
                                 FROM            [FrameworkCompetencyGroups]
                                 WHERE        ([FrameworkID] = @frameworkId)), 0)+1, @frameworkId)",
                             new { groupId, adminId, frameworkId });
                if (numberOfAffectedRows < 1)
                {
                    logger.LogWarning(
                        "Not inserting framework competency group as db insert failed. " +
                        $"Group id: {groupId}, admin id: {adminId}, frameworkId: {frameworkId}"
                    );
                    return -1;
                }
                existingId = (int)connection.ExecuteScalar(
                 @"SELECT COALESCE ((SELECT ID FROM FrameworkCompetencyGroups WHERE CompetencyGroupID = @groupID AND FrameworkID = @frameworkID), 0) AS FrameworkCompetencyGroupID",
                 new { groupId, frameworkId });
                return existingId;
            }
        }
        public int InsertCompetency(string name, string? description, int adminId)
        {
            if (name.Length == 0 | adminId < 1)
            {
                logger.LogWarning(
                    $"Not inserting competency as it failed server side valiidation. AdminId: {adminId}, name: {name}, description:{description}"
                );
                return -2;
            }
            int existingId = 0;
            if (description == null)
            {
                existingId = (int)connection.ExecuteScalar(
                                @"SELECT COALESCE ((SELECT TOP(1) ID FROM Competencies WHERE [Name] = @name AND [Description] IS NULL), 0) AS CompetencyID",
                                new { name, description });
            }
            else
            {
                existingId = (int)connection.ExecuteScalar(
                                @"SELECT COALESCE ((SELECT TOP(1) ID FROM Competencies WHERE [Name] = @name AND [Description] = @description), 0) AS CompetencyID",
                                new { name, description });
            }
            if (existingId > 0)
            {
                return existingId;
            }
            else
            {
                var numberOfAffectedRows = connection.Execute(
                              @"INSERT INTO Competencies ([Name], [Description], UpdatedByAdminID)
                    VALUES (@name, @description, @adminId)",
                             new { name, description, adminId });
                if (numberOfAffectedRows < 1)
                {
                    logger.LogWarning(
                        $"Not inserting competency as db insert failed. AdminId: {adminId}, name: {name}, description:{description}"
                    );
                    return -1;
                }
                if (description == null)
                {
                    existingId = (int)connection.ExecuteScalar(
                                    @"SELECT COALESCE ((SELECT TOP(1) ID FROM Competencies WHERE [Name] = @name AND [Description] IS NULL), 0) AS CompetencyID",
                                    new { name, description });
                }
                else
                {
                    existingId = (int)connection.ExecuteScalar(
                                    @"SELECT COALESCE ((SELECT TOP(1) ID FROM Competencies WHERE [Name] = @name AND [Description] = @description), 0) AS CompetencyID",
                                    new { name, description });
                }
                return existingId;
            }
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
        public int InsertFrameworkCompetency(int competencyId, int? frameworkCompetencyGroupID, int adminId, int frameworkId)
        {
            if (competencyId < 1 | adminId < 1 | frameworkId < 1)
            {
                logger.LogWarning(
                    $"Not inserting framework competency as it failed server side valiidation. AdminId: {adminId}, frameworkCompetencyGroupID: {frameworkCompetencyGroupID}, competencyId:{competencyId}, frameworkId:{frameworkId}"
                );
                return -2;
            }
            int existingId = 0;
            if (frameworkCompetencyGroupID == null)
            {
                existingId = (int)connection.ExecuteScalar(
             @"SELECT COALESCE ((SELECT ID FROM FrameworkCompetencies WHERE [CompetencyID] = @competencyId AND FrameworkCompetencyGroupID IS NULL), 0) AS FrameworkCompetencyID",
           new { competencyId, frameworkCompetencyGroupID });
            }
            else
            {
                existingId = (int)connection.ExecuteScalar(
                                 @"SELECT COALESCE ((SELECT ID FROM FrameworkCompetencies WHERE [CompetencyID] = @competencyId AND FrameworkCompetencyGroupID = @frameworkCompetencyGroupID), 0) AS FrameworkCompetencyID",
                               new { competencyId, frameworkCompetencyGroupID });
            }
            if (existingId > 0)
            {
                return existingId;
            }
            else
            {
                var numberOfAffectedRows = connection.Execute(
                             @"INSERT INTO FrameworkCompetencies ([CompetencyID], FrameworkCompetencyGroupID, UpdatedByAdminID, Ordering, FrameworkID)
                    VALUES (@competencyId, @frameworkCompetencyGroupID, @adminId, COALESCE
                             ((SELECT        MAX(Ordering) AS OrderNum
                                 FROM            [FrameworkCompetencies]
                                 WHERE        ([FrameworkCompetencyGroupID] = @frameworkCompetencyGroupID)), 0)+1, @frameworkId)",
                            new { competencyId, frameworkCompetencyGroupID, adminId, frameworkId });
                if (numberOfAffectedRows < 1)
                {
                    logger.LogWarning(
                        $"Not inserting framework competency as db insert failed. AdminId: {adminId}, frameworkCompetencyGroupID: {frameworkCompetencyGroupID}, competencyId: {competencyId}"
                    );
                    return -1;
                }
                if (frameworkCompetencyGroupID == null)
                {
                    existingId = (int)connection.ExecuteScalar(
                 @"SELECT COALESCE ((SELECT ID FROM FrameworkCompetencies WHERE [CompetencyID] = @competencyId AND FrameworkCompetencyGroupID IS NULL), 0) AS FrameworkCompetencyID",
               new { competencyId, frameworkCompetencyGroupID });
                }
                else
                {
                    existingId = (int)connection.ExecuteScalar(
                                     @"SELECT COALESCE ((SELECT ID FROM FrameworkCompetencies WHERE [CompetencyID] = @competencyId AND FrameworkCompetencyGroupID = @frameworkCompetencyGroupID), 0) AS FrameworkCompetencyID",
                                   new { competencyId, frameworkCompetencyGroupID });
                }
                AddDefaultQuestionsToCompetency(competencyId, frameworkId);
                return existingId;
            }
        }

        public IEnumerable<CollaboratorDetail> GetCollaboratorsForFrameworkId(int frameworkId)
        {
            return connection.Query<CollaboratorDetail>(
                $@"SELECT 0 AS ID, fw.ID AS FrameworkID, au.AdminID AS AdminID, 1 AS CanModify, au.Email AS UserEmail, 'Owner' AS FrameworkRole
                    FROM   Frameworks AS fw INNER JOIN
                        AdminUsers AS au ON fw.OwnerAdminID = au.AdminID
                    WHERE (fw.ID = @FrameworkID)
                    UNION ALL
                   SELECT ID, FrameworkID, AdminID, CanModify, UserEmail, CASE WHEN CanModify = 1 THEN 'Contributor' ELSE 'Reviewer' END AS FrameworkRole
                    FROM   FrameworkCollaborators
                    WHERE (FrameworkID = @FrameworkID)", new { frameworkId });
        }

        public int AddCollaboratorToFramework(int frameworkId, string userEmail, bool canModify)
        {
            if (userEmail.Length == 0)
            {
                logger.LogWarning(
                    $"Not adding collaborator to framework as it failed server side valiidation. frameworkId: {frameworkId}, userEmail: {userEmail}, canModify:{canModify}"
                );
                return -3;
            }
            int existingId = (int)connection.ExecuteScalar(
               @"SELECT COALESCE
                 ((SELECT ID
                  FROM    FrameworkCollaborators
                  WHERE (FrameworkID = @frameworkId) AND (UserEmail = @userEmail)), 0) AS ID",
               new { frameworkId, userEmail });
            if (existingId > 0)
            {
                return -2;
            }
            else
            {
                var adminId = (int?)connection.ExecuteScalar(
                    @"SELECT AdminID FROM AdminUsers WHERE Email = @userEmail AND Active = 1", new { userEmail }
                    );
                
                    var numberOfAffectedRows = connection.Execute(
             @"INSERT INTO FrameworkCollaborators (FrameworkID, AdminID, UserEmail, CanModify)
                    VALUES (@frameworkId, @adminId, @userEmail, @canModify)",
            new { frameworkId, adminId, userEmail, canModify });
                    if (numberOfAffectedRows < 1)
                    {
                        logger.LogWarning(
                            $"Not inserting framework collaborator as db insert failed. AdminId: {adminId}, userEmail: {userEmail}, frameworkId: {frameworkId}, canModify: {canModify}"
                        );
                        return -1;
                    }
                if (adminId > 0)
                {
                    connection.Execute(@"UPDATE AdminUsers SET IsFrameworkContributor = 1 WHERE AdminId = @adminId AND IsFrameworkContributor = 0", new { adminId });
                }
                existingId = (int)connection.ExecuteScalar(
                 @"SELECT COALESCE
                 ((SELECT ID
                  FROM    FrameworkCollaborators
                  WHERE (FrameworkID = @frameworkId) AND (UserEmail = @userEmail)), 0) AS AdminID",
               new { frameworkId, adminId, userEmail });
                return existingId;
            }
        }
        public void RemoveCollaboratorFromFramework(int frameworkId, int id)
        {
            var adminId = (int?)connection.ExecuteScalar(
                    @"SELECT AdminID FROM FrameworkCollaborators WHERE  (FrameworkID = @frameworkId) AND (ID = @id)", new { frameworkId, id }
                    );
            connection.Execute(
                             @"DELETE FROM  FrameworkCollaborators WHERE (FrameworkID = @frameworkId) AND (ID = @id);UPDATE AdminUsers SET IsFrameworkContributor = 0 WHERE AdminID = @adminId AND AdminID NOT IN (SELECT DISTINCT AdminID FROM FrameworkCollaborators);",
                            new { frameworkId, id, adminId });

        }

        public IEnumerable<FrameworkCompetencyGroup> GetFrameworkCompetencyGroups(int frameworkId)
        {
            var result = connection.Query<FrameworkCompetencyGroup, FrameworkCompetency, FrameworkCompetencyGroup>(
                @"SELECT fcg.ID, fcg.CompetencyGroupID, cg.Name, fcg.Ordering, fc.ID, c.ID AS CompetencyID, c.Name, c.Description, fc.Ordering, COUNT(caq.AssessmentQuestionID) AS AssessmentQuestions
                    ,(SELECT COUNT(*) FROM CompetencyLearningResources clr WHERE clr.CompetencyID = c.ID) AS CompetencyLearningResourcesCount
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
                    frameworkCompetencyGroup.FrameworkCompetencies.Add(frameworkCompetency);
                    return frameworkCompetencyGroup;
                },
              param: new { frameworkId }
           );
            return result.GroupBy(frameworkCompetencyGroup => frameworkCompetencyGroup.CompetencyGroupID).Select(group =>
            {
                var groupedFrameworkCompetencyGroup = group.First();
                groupedFrameworkCompetencyGroup.FrameworkCompetencies = group.Select(frameworkCompetencyGroup => frameworkCompetencyGroup.FrameworkCompetencies.Single()).ToList();
                return groupedFrameworkCompetencyGroup;
            });
        }
        public IEnumerable<FrameworkCompetency> GetFrameworkCompetenciesUngrouped(int frameworkId)
        {
            return connection.Query<FrameworkCompetency>(
                @"SELECT fc.ID, c.ID AS CompetencyID, c.Name, c.Description, fc.Ordering, COUNT(caq.AssessmentQuestionID) AS AssessmentQuestions
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
            if (frameworkName.Length == 0 | adminId < 1 | frameworkId < 1)
            {
                logger.LogWarning(
                    $"Not updating framework name as it failed server side validation. AdminId: {adminId}, frameworkName: {frameworkName}, frameworkId: {frameworkId}"
                );
                return false;
            }
            int existingFrameworks = (int)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM Frameworks WHERE FrameworkName = @frameworkName AND ID <> @frameworkId",
                new { frameworkName, frameworkId });
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
            else
            {
                return true;
            }
        }
        public void UpdateFrameworkDescription(int frameworkId, int adminId, string? frameworkDescription)
        {
            if (adminId < 1 | frameworkId < 1)
            {
                logger.LogWarning(
                    $"Not updating framework description as it failed server side validation. AdminId: {adminId}, frameworkDescription: {frameworkDescription}, frameworkId: {frameworkId}"
                );
            }
            var numberOfAffectedRows = connection.Execute(
               @"UPDATE Frameworks SET Description = @frameworkDescription, UpdatedByAdminID = @adminId
                    WHERE ID = @frameworkId",
               new { frameworkDescription, adminId, frameworkId });
        }
        public void UpdateFrameworkConfig(int frameworkId, int adminId, string? frameworkConfig)
        {
            if (adminId < 1 | frameworkId < 1)
            {
                logger.LogWarning(
                    $"Not updating framework config as it failed server side validation. AdminId: {adminId}, frameworkConfig: {frameworkConfig}, frameworkId: {frameworkId}"
                );
            }
            var numberOfAffectedRows = connection.Execute(
               @"UPDATE Frameworks SET FrameworkConfig = @frameworkConfig, UpdatedByAdminID = @adminId
                    WHERE ID = @frameworkId",
               new { frameworkConfig, adminId, frameworkId });
        }
        public CompetencyGroupBase? GetCompetencyGroupBaseById(int Id)
        {
            return connection.QueryFirstOrDefault<CompetencyGroupBase>(
                @"SELECT fcg.ID, fcg.CompetencyGroupID, cg.Name
                    FROM   FrameworkCompetencyGroups AS fcg
                        INNER JOIN CompetencyGroups AS cg ON fcg.CompetencyGroupID = cg.ID
                    WHERE (fcg.ID = @Id)", new { Id }
                );
        }
        public FrameworkCompetency? GetFrameworkCompetencyById(int Id)
        {
            return connection.QueryFirstOrDefault<FrameworkCompetency>(
                 @"SELECT fc.ID, c.ID AS CompetencyID, c.Name, c.Description, fc.Ordering
                	FROM FrameworkCompetencies AS fc 
                		INNER JOIN Competencies AS c ON fc.CompetencyID = c.ID
                	WHERE fc.ID = @Id",
                new { Id }
                );
        }
        public void UpdateFrameworkCompetencyGroup(int frameworkCompetencyGroupId, int competencyGroupId, string name, int adminId)
        {
            if (frameworkCompetencyGroupId < 1 | adminId < 1 | competencyGroupId < 1 | name.Length < 3)
            {
                logger.LogWarning(
                    $"Not updating framework competency group as it failed server side validation. AdminId: {adminId}, frameworkCompetencyGroupId: {frameworkCompetencyGroupId}, competencyGroupId: {competencyGroupId}, name: {name}"
                );
                return;
            }
            int usedElsewhere = (int)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM FrameworkCompetencyGroups
                    WHERE CompetencyGroupId = @competencyGroupId
                    AND ID <> @frameworkCompetencyGroupId",
                new { frameworkCompetencyGroupId, competencyGroupId }
                );
            if (usedElsewhere > 0)
            {
                var newCompetencyGroupId = InsertCompetencyGroup(name, adminId);
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
               @"UPDATE CompetencyGroups SET Name = @name, UpdatedByAdminID = @adminId
                    WHERE ID = @competencyGroupId",
              new { name, adminId, competencyGroupId }
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
        public void UpdateFrameworkCompetency(int frameworkCompetencyId, string name, string? description, int adminId)
        {
            if (frameworkCompetencyId < 1 | adminId < 1 | name.Length < 3)
            {
                logger.LogWarning(
                    $"Not updating framework competency as it failed server side validation. AdminId: {adminId}, frameworkCompetencyId: {frameworkCompetencyId}, name: {name}, description: {description}"
                );
                return;
            }
            //DO WE NEED SOMETHING IN HERE TO CHECK WHETHER IT IS USED ELSEWHERE AND WARN THE USER?
            var numberOfAffectedRows = connection.Execute(
           @"UPDATE Competencies SET Name = @name, Description = @description, UpdatedByAdminID = @adminId
                    FROM   Competencies INNER JOIN
             FrameworkCompetencies AS fc ON Competencies.ID = fc.CompetencyID
WHERE (fc.Id = @frameworkCompetencyId)",
          new { name, description, adminId, frameworkCompetencyId }
      );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating competency group name as db update failed. " +
                    $"Name: {name}, admin id: {adminId}, frameworkCompetencyId: {frameworkCompetencyId}"
                );
            }
        }
        public void MoveFrameworkCompetencyGroup(int frameworkCompetencyGroupId, bool singleStep, string direction)
        {
            connection.Execute("ReorderFrameworkCompetencyGroup", new { frameworkCompetencyGroupId, direction, singleStep }, commandType: CommandType.StoredProcedure);
        }
        public void MoveFrameworkCompetency(int frameworkCompetencyId, bool singleStep, string direction)
        {
            connection.Execute("ReorderFrameworkCompetency", new { frameworkCompetencyId, direction, singleStep }, commandType: CommandType.StoredProcedure);
        }
        public void DeleteFrameworkCompetencyGroup(int frameworkCompetencyGroupId, int competencyGroupId, int adminId)
        {
            if (frameworkCompetencyGroupId < 1 | adminId < 1 | competencyGroupId < 1)
            {
                logger.LogWarning(
                    $"Not deleting framework competency group as it failed server side validation. AdminId: {adminId}, frameworkCompetencyGroupId: {frameworkCompetencyGroupId}, competencyGroupId: {competencyGroupId}"
                );
                return;
            }
            connection.Execute(
                @"UPDATE FrameworkCompetencyGroups
                   SET UpdatedByAdminID = @adminId
                    WHERE ID = @frameworkCompetencyGroupId", new { adminId, frameworkCompetencyGroupId }
                );
            var numberOfAffectedRows = connection.Execute(
                @"DELETE FROM FrameworkCompetencyGroups WHERE ID = @frameworkCompetencyGroupId", new { frameworkCompetencyGroupId }
                );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not deleting framework competency group as db update failed. " +
                    $"frameworkCompetencyGroupId: {frameworkCompetencyGroupId}, competencyGroupId: {competencyGroupId}, adminId: {adminId}"
                );
            }
            //Check if used elsewhere and delete competency group if not:
            int usedElsewhere = (int)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM FrameworkCompetencyGroups
                    WHERE CompetencyGroupId = @competencyGroupId",
                new { competencyGroupId }
                );
            if (usedElsewhere == 0)
            {
                usedElsewhere = (int)connection.ExecuteScalar(
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
                    WHERE ID = @competencyGroupId", new { adminId, competencyGroupId }
               );
                numberOfAffectedRows = connection.Execute(
                    @"DELETE FROM CompetencyGroups WHERE ID = @competencyGroupId", new { competencyGroupId }
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
            int competencyId = (int)connection.ExecuteScalar(@"SELECT CompetencyID FROM FrameworkCompetencies WHERE ID = @frameworkCompetencyId", new { frameworkCompetencyId });
            if (frameworkCompetencyId < 1 | adminId < 1 | competencyId < 1)
            {
                logger.LogWarning(
                    $"Not deleting framework competency group as it failed server side validation. AdminId: {adminId}, frameworkCompetencyId: {frameworkCompetencyId}, competencyId: {competencyId}"
                );
                return;
            }
            connection.Execute(
                @"UPDATE FrameworkCompetencies
                   SET UpdatedByAdminID = @adminId
                    WHERE ID = @frameworkCompetencyId", new { adminId, frameworkCompetencyId }
                );
            var numberOfAffectedRows = connection.Execute(
                @"DELETE FROM FrameworkCompetencies WHERE ID = @frameworkCompetencyId", new { frameworkCompetencyId }
                );
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not deleting framework competency as db update failed. " +
                    $"frameworkCompetencyId: {frameworkCompetencyId}, competencyId: {competencyId}, adminId: {adminId}"
                );
            }
            //Check if used elsewhere and delete competency group if not:
            int usedElsewhere = (int)connection.ExecuteScalar(
                @"SELECT COUNT(*) FROM FrameworkCompetencies
                    WHERE CompetencyID = @competencyId",
                new { competencyId }
                );
            if (usedElsewhere == 0)
            {
                usedElsewhere = (int)connection.ExecuteScalar(
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
                    WHERE ID = @competencyId", new { adminId, competencyId }
               );
                numberOfAffectedRows = connection.Execute(
                    @"DELETE FROM CompetencyAssessmentQuestions WHERE CompetencyID = @competencyId;
                        DELETE FROM Competencies WHERE ID = @competencyId", new { competencyId }
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
        public IEnumerable<AssessmentQuestion> GetAllCompetencyQuestions(int adminId)
        {
            return connection.Query<AssessmentQuestion>(
                 $@"{AssessmentQuestionFields}
                    {AssessmentQuestionTables}
                     ORDER BY [Question]",
                 new { adminId });
        }
        public IEnumerable<AssessmentQuestion> GetFrameworkDefaultQuestionsById(int frameworkId, int adminId)
        {
            return connection.Query<AssessmentQuestion>(
                 $@"{AssessmentQuestionFields}
                    {AssessmentQuestionTables}
                    INNER JOIN FrameworkDefaultQuestions AS FDQ ON AQ.ID = FDQ.AssessmentQuestionID
                    WHERE FDQ.FrameworkId = @frameworkId"
                    , new { frameworkId, adminId });
        }
        public IEnumerable<AssessmentQuestion> GetCompetencyAssessmentQuestionsById(int competencyId, int adminId)
        {
            return connection.Query<AssessmentQuestion>(
                 $@"{AssessmentQuestionFields}
                    {AssessmentQuestionTables}
                     INNER JOIN CompetencyAssessmentQuestions AS CAQ ON AQ.ID = CAQ.AssessmentQuestionID
                     WHERE CAQ.CompetencyID = @competencyId
                     ORDER BY [Question]"
                    , new { competencyId, adminId });
        }
        public void AddFrameworkDefaultQuestion(int frameworkId, int assessmentQuestionId, int adminId, bool addToExisting)
        {
            if (frameworkId < 1 | adminId < 1 | assessmentQuestionId < 1)
            {
                logger.LogWarning(
                    $"Not inserting framework default question as it failed server side validation. AdminId: {adminId}, frameworkId: {frameworkId}, assessmentQuestionId: {assessmentQuestionId}"
                );
                return;
            }
            var numberOfAffectedRows = connection.Execute(
                @"INSERT INTO FrameworkDefaultQuestions (FrameworkId, AssessmentQuestionID)
                      VALUES (@frameworkId, @assessmentQuestionId)"
                    , new { frameworkId, assessmentQuestionId });
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not inserting framework default question as db update failed. " +
                    $"frameworkId: {frameworkId}, assessmentQuestionId: {assessmentQuestionId}"
                );
                return;
            }
            else if (addToExisting)
            {
                numberOfAffectedRows = connection.Execute(
                    @"INSERT INTO CompetencyAssessmentQuestions (CompetencyID, AssessmentQuestionID, Ordering)
                        SELECT CompetencyID, @assessmentQuestionId AS AssessmentQuestionID, COALESCE
                             ((SELECT        MAX(Ordering)
                                 FROM            [CompetencyAssessmentQuestions]
                                 WHERE        ([CompetencyId] = fc.CompetencyID)), 0)+1 AS Ordering
                        FROM FrameworkCompetencies AS fc
                        WHERE FrameworkID = @frameworkId AND NOT EXISTS (SELECT * FROM CompetencyAssessmentQuestions WHERE CompetencyID = fc.CompetencyID AND AssessmentQuestionID = @assessmentQuestionId)",
                    new { assessmentQuestionId, frameworkId });
            }
        }
        public void DeleteFrameworkDefaultQuestion(int frameworkId, int assessmentQuestionId, int adminId, bool deleteFromExisting)
        {
            if (frameworkId < 1 | adminId < 1 | assessmentQuestionId < 1)
            {
                logger.LogWarning(
                    $"Not deleting framework default question as it failed server side validation. AdminId: {adminId}, frameworkId: {frameworkId}, assessmentQuestionId: {assessmentQuestionId}"
                );
                return;
            }
            var numberOfAffectedRows = connection.Execute(
                @"DELETE FROM FrameworkDefaultQuestions
                    WHERE FrameworkId = @frameworkId AND AssessmentQuestionID = @assessmentQuestionId",
                new { frameworkId, assessmentQuestionId });
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not deleting framework default question as db update failed. " +
                    $"frameworkId: {frameworkId}, assessmentQuestionId: {assessmentQuestionId}"
                );
                return;
            }
            else if (deleteFromExisting)
            {
                numberOfAffectedRows = connection.Execute(
                    @"DELETE FROM CompetencyAssessmentQuestions
                        WHERE AssessmentQuestionID = @assessmentQuestionId
                        AND CompetencyID IN (
                            SELECT CompetencyID FROM FrameworkCompetencies
                            WHERE FrameworkID = @frameworkId)",
                    new { frameworkId, assessmentQuestionId });
            }
        }
        public IEnumerable<GenericSelectList> GetAssessmentQuestions(int frameworkId, int adminId)
        {
            return connection.Query<GenericSelectList>(
                @"SELECT AQ.ID, CASE WHEN AddedByAdminId = @adminId THEN '* ' ELSE '' END + Question + ' (' + InputTypeName + ' ' + CAST(MinValue AS nvarchar) + ' to ' + CAST(MaxValue As nvarchar) + ')' AS Label
                    FROM AssessmentQuestions AS AQ LEFT OUTER JOIN AssessmentQuestionInputTypes AS AQI ON AQ.AssessmentQuestionInputTypeID = AQI.ID
                    WHERE AQ.ID NOT IN (SELECT AssessmentQuestionID FROM FrameworkDefaultQuestions WHERE FrameworkId = @frameworkId)
					ORDER BY Label", new { frameworkId, adminId }
                );
        }
        public IEnumerable<GenericSelectList> GetAssessmentQuestionsForCompetency(int frameworkCompetencyId, int adminId)
        {
            return connection.Query<GenericSelectList>(
                @"SELECT AQ.ID, CASE WHEN AddedByAdminId = @adminId THEN '* ' ELSE '' END + Question + ' (' + InputTypeName + ' ' + CAST(MinValue AS nvarchar) + ' to ' + CAST(MaxValue As nvarchar) + ')' AS Label
                    FROM AssessmentQuestions AS AQ LEFT OUTER JOIN AssessmentQuestionInputTypes AS AQI ON AQ.AssessmentQuestionInputTypeID = AQI.ID
                    WHERE AQ.ID NOT IN (SELECT AssessmentQuestionID FROM CompetencyAssessmentQuestions AS CAQ INNER JOIN FrameworkCompetencies AS FC ON CAQ.CompetencyID = FC.CompetencyID WHERE FC.ID = @frameworkCompetencyId) ORDER BY Question", new { frameworkCompetencyId, adminId }
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
WHERE (FrameworkID = @frameworkId)", new { frameworkId, assessmentQuestionId }
                );
        }
        public IEnumerable<AssessmentQuestion> GetCompetencyAssessmentQuestionsByFrameworkCompetencyId(int frameworkCompetencyId, int adminId)
        {
            return connection.Query<AssessmentQuestion>(
                 $@"{AssessmentQuestionFields}
                    {AssessmentQuestionTables}
                    INNER JOIN CompetencyAssessmentQuestions AS CAQ ON AQ.ID = CAQ.AssessmentQuestionID
                    INNER JOIN FrameworkCompetencies AS FC ON CAQ.CompetencyId = FC.CompetencyId
                    WHERE FC.Id = @frameworkCompetencyId
                     ORDER BY CAQ.Ordering"
                    , new
                    {
                        frameworkCompetencyId,
                        adminId
                    });
        }
        public void AddCompetencyAssessmentQuestion(int frameworkCompetencyId, int assessmentQuestionId, int adminId)
        {
            if (frameworkCompetencyId < 1 | adminId < 1 | assessmentQuestionId < 1)
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
                        WHERE Id = @frameworkCompetencyId"
                    , new { frameworkCompetencyId, assessmentQuestionId });
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
            if (frameworkCompetencyId < 1 | adminId < 1 | assessmentQuestionId < 1)
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
                new { frameworkCompetencyId, assessmentQuestionId });
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
                    WHERE AQ.ID = @assessmentQuestionId", new { adminId, assessmentQuestionId }
                );
        }
        public LevelDescriptor GetLevelDescriptorForAssessmentQuestionId(int assessmentQuestionId, int adminId, int level)
        {
            return connection.QueryFirstOrDefault<LevelDescriptor>(
                @"SELECT COALESCE(ID,0) AS ID, @assessmentQuestionId AS AssessmentQuestionID, n AS LevelValue, LevelLabel, LevelDescription, COALESCE(UpdatedByAdminID, @adminId) AS UpdatedByAdminID
                    FROM
                    (SELECT TOP (@level) n = ROW_NUMBER() OVER (ORDER BY number)
                    FROM [master]..spt_values) AS q1
                    LEFT OUTER JOIN AssessmentQuestionLevels AS AQL ON q1.n = AQL.LevelValue AND AQL.AssessmentQuestionID = @assessmentQuestionId
                    WHERE (q1.n = @level)", new { assessmentQuestionId, adminId, level }
                );
        }
        public IEnumerable<LevelDescriptor> GetLevelDescriptorsForAssessmentQuestionId(int assessmentQuestionId, int adminId, int minValue, int maxValue, bool zeroBased)
        {
            int adjustBy = zeroBased ? 1 : 0;
            return connection.Query<LevelDescriptor>(
               @"SELECT COALESCE(ID,0) AS ID, @assessmentQuestionId AS AssessmentQuestionID, n AS LevelValue, LevelLabel, LevelDescription, COALESCE(UpdatedByAdminID, @adminId) AS UpdatedByAdminID
                    FROM
                    (SELECT TOP (@maxValue + @adjustBy) n = ROW_NUMBER() OVER (ORDER BY number) - @adjustBy
                    FROM [master]..spt_values) AS q1
                    LEFT OUTER JOIN AssessmentQuestionLevels AS AQL ON q1.n = AQL.LevelValue AND AQL.AssessmentQuestionID = @assessmentQuestionId
                    WHERE (q1.n BETWEEN @minValue AND @maxValue)", new { assessmentQuestionId, adminId, minValue, maxValue, adjustBy }
               );
        }
        public int InsertAssessmentQuestion(string question, int assessmentQuestionInputTypeId, string? maxValueDescription, string? minValueDescription, string? scoringInstructions, int minValue, int maxValue, bool includeComments, int adminId, string? commentsPrompt, string? commentsHint)
        {
            if (question == null | adminId < 1)
            {
                logger.LogWarning(
                    $"Not inserting assessment question as it failed server side validation. AdminId: {adminId}, question: {question}"
                );
                return 0;
            }
            var id = connection.QuerySingle<int>(
               @"INSERT INTO AssessmentQuestions (Question, AssessmentQuestionInputTypeID, MaxValueDescription, MinValueDescription, ScoringInstructions, MinValue, MaxValue, IncludeComments, AddedByAdminId, CommentsPrompt, CommentsHint)
                      OUTPUT INSERTED.Id
                      VALUES (@question, @assessmentQuestionInputTypeId, @maxValueDescription, @minValueDescription, @scoringInstructions, @minValue, @maxValue, @includeComments, @adminId, @commentsPrompt, @commentsHint)"
                   , new { question, assessmentQuestionInputTypeId, maxValueDescription, minValueDescription, scoringInstructions, minValue, maxValue, includeComments, adminId, commentsPrompt, commentsHint });
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
        public void InsertLevelDescriptor(int assessmentQuestionId, int levelValue, string levelLabel, string? levelDescription, int adminId)
        {
            if (assessmentQuestionId < 1 | adminId < 1 | levelValue < 0)
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
                      VALUES (@assessmentQuestionId, @levelValue, @levelLabel, @levelDescription, @adminId)"
                   , new { assessmentQuestionId, levelValue, levelLabel, levelDescription, adminId });
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not inserting assessment question level descriptor as db update failed. " +
                    $"AdminId: {adminId}, assessmentQuestionId: {assessmentQuestionId}, levelValue: {levelValue}"
                );
            }
        }
        public void UpdateAssessmentQuestion(int id, string question, int assessmentQuestionInputTypeId, string? maxValueDescription, string? minValueDescription, string? scoringInstructions, int minValue, int maxValue, bool includeComments, int adminId, string? commentsPrompt, string? commentsHint)
        {
            if (id < 1 | question == null | adminId < 1)
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
                    WHERE ID = @id", new { id, question, assessmentQuestionInputTypeId, maxValueDescription, minValueDescription, scoringInstructions, minValue, maxValue, includeComments, adminId, commentsPrompt, commentsHint });
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating assessment question as db update failed. " +
                    $"Id: {id}, AdminId: {adminId}, question: {question}"
                );
            }
        }
        public void UpdateLevelDescriptor(int id, int levelValue, string levelLabel, string? levelDescription, int adminId)
        {
            if (id < 1 | adminId < 1 | levelValue < 0)
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
                    WHERE ID = @id", new { id, levelValue, levelLabel, levelDescription, adminId });
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not updating assessment question level descriptor as db update failed. " +
                    $"Id: {id}, AdminId: {adminId}, levelValue: {levelValue}"
                );
            }
        }
        public Models.SelfAssessments.Competency GetFrameworkCompetencyForPreview(int frameworkCompetencyId)
        {
            Models.SelfAssessments.Competency? competencyResult = null;
            return connection.Query<Models.SelfAssessments.Competency, Models.SelfAssessments.AssessmentQuestion, Models.SelfAssessments.Competency>(
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
                                                  AQ.AssessmentQuestionInputTypeID,
                                                  AQ.IncludeComments,
                                                  AQ.MinValue AS Result,
                                                  AQ.CommentsPrompt,
                                                  AQ.CommentsHint
												  FROM   Competencies AS C INNER JOIN
             FrameworkCompetencies AS FC ON C.ID = FC.CompetencyID INNER JOIN
             FrameworkCompetencyGroups AS FCG ON FC.FrameworkCompetencyGroupID = FCG.ID INNER JOIN
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
                param: new { frameworkCompetencyId }
            ).FirstOrDefault();
        }
        public int GetAdminUserRoleForFrameworkId(int adminId, int frameworkId)
        {
            return (int)connection.ExecuteScalar(
               @"SELECT CASE WHEN FW.OwnerAdminID = @adminId THEN 3 WHEN fwc.CanModify = 1 THEN 2 WHEN fwc.CanModify = 0 THEN 1 ELSE 0 END AS UserRole
                FROM Frameworks AS FW LEFT OUTER JOIN
                FrameworkCollaborators AS fwc ON fwc.FrameworkID = FW.ID AND fwc.AdminID = @adminId
                WHERE FW.ID = @frameworkId",
              new { adminId, frameworkId }
          );
        }
        public IEnumerable<CommentReplies> GetCommentsForFrameworkId(int frameworkId, int adminId)
        {
            var result = connection.Query<CommentReplies>(
                @"SELECT ID, ReplyToFrameworkCommentID, AdminID, (SELECT Forename + ' ' + Surname FROM AdminUsers WHERE AdminID = FrameworkComments.AdminId) AS Commenter, CAST(CASE WHEN AdminID = @adminId THEN 1 ELSE 0 END AS Bit) AS UserIsCommenter, AddedDate, Comments, LastEdit
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
        public List<Comment> GetRepliesForCommentId(int commentId, int adminId)
        {
            return connection.Query<Comment>(
                @"SELECT ID, ReplyToFrameworkCommentID, AdminID, (SELECT Forename + ' ' + Surname FROM AdminUsers WHERE AdminID = FrameworkComments.AdminId) AS Commenter, CAST(CASE WHEN AdminID IS NULL THEN NULL WHEN AdminID = @adminId THEN 1 ELSE 0 END AS Bit) AS UserIsCommenter, ReplyToFrameworkCommentID, AddedDate, Comments, LastEdit
                    FROM FrameworkComments
                    WHERE Archived Is NULL AND ReplyToFrameworkCommentID = @commentId
                    ORDER BY AddedDate ASC", new { commentId, adminId }
           ).ToList();
        }
        public CommentReplies GetCommentRepliesById(int commentId, int adminId)
        {

            var result = connection.Query<CommentReplies>(
                 @"SELECT ID, ReplyToFrameworkCommentID, AdminID, (SELECT Forename + ' ' + Surname FROM AdminUsers WHERE AdminID = FrameworkComments.AdminId) AS Commenter, CAST(CASE WHEN AdminID = @adminId THEN 1 ELSE 0 END AS Bit) AS UserIsCommenter, AddedDate, Comments, LastEdit
                    FROM FrameworkComments
                    WHERE Archived Is NULL AND ReplyToFrameworkCommentID Is NULL AND ID = @commentId", new { commentId, adminId }
           ).FirstOrDefault();
            var replies = GetRepliesForCommentId(commentId, adminId);
            foreach (var reply in replies)
            {
                result.Replies.Add(reply);
            }
            return result;
        }
        public int InsertComment(int frameworkId, int adminId, string comment, int? replyToCommentId)
        {
            if (frameworkId < 1 | adminId < 1 | comment == null)
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
                      SELECT CAST(SCOPE_IDENTITY() as int)"
                   , new { adminId, replyToCommentId, comment, frameworkId });
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
                    WHERE ID = @commentId", new { commentId });
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
        public CollaboratorNotification GetCollaboratorNotification(int id, int invitedByAdminId)
        {
            return connection.Query<CollaboratorNotification>(

                @"SELECT fc.FrameworkID, fc.AdminID, fc.CanModify, fc.UserEmail, CASE WHEN fc.CanModify = 1 THEN 'Contributor' ELSE 'Reviewer' END AS FrameworkRole, f.FrameworkName,
                    (SELECT Forename + ' ' + Surname AS Expr1
                         FROM    AdminUsers AS au1
                         WHERE (AdminID = @invitedByAdminId)) AS InvitedByName,
                    (SELECT Email
                        FROM    AdminUsers AS au2
                        WHERE (AdminID = @invitedByAdminId)) AS InvitedByEmail
                    FROM   FrameworkCollaborators AS fc INNER JOIN
                        Frameworks AS f ON fc.FrameworkID = f.ID
                    WHERE (fc.ID = @id)",
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

        public Comment GetCommentById(int adminId, int commentId)
        {
            return connection.Query<Comment>(
                @"SELECT ID, ReplyToFrameworkCommentID, AdminID, CAST(CASE WHEN AdminID = @adminId THEN 1 ELSE 0 END AS Bit) AS UserIsCommenter, AddedDate, Comments, Archived, LastEdit, FrameworkID
FROM   FrameworkComments
WHERE (ID = @commentId)", new { adminId, commentId }
                ).FirstOrDefault();
        }

        public IEnumerable<CollaboratorDetail> GetReviewersForFrameworkId(int frameworkId)
        {
            return connection.Query<CollaboratorDetail>(
                $@"SELECT FrameworkCollaborators.ID, FrameworkCollaborators.FrameworkID, FrameworkCollaborators.AdminID, FrameworkCollaborators.CanModify, FrameworkCollaborators.UserEmail, CASE WHEN CanModify = 1 THEN 'Contributor' ELSE 'Reviewer' END AS FrameworkRole
FROM   FrameworkCollaborators LEFT OUTER JOIN
             FrameworkReviews ON FrameworkCollaborators.ID = FrameworkReviews.FrameworkCollaboratorID
WHERE (FrameworkCollaborators.FrameworkID = @FrameworkID) AND (FrameworkReviews.ID IS NULL) OR
             (FrameworkCollaborators.FrameworkID = @FrameworkID) AND (FrameworkReviews.Archived IS NOT NULL)", new { frameworkId });
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
                    WHERE FR.FrameworkID = @frameworkId AND FR.Archived IS NULL",
                new { frameworkId });
        }

        public FrameworkReview? GetFrameworkReview(int frameworkId, int adminId, int reviewId)
        {
            return connection.Query<FrameworkReview>(
                @"SELECT FR.ID, FR.FrameworkID, FR.FrameworkCollaboratorID, FC.UserEmail, CAST(CASE WHEN FC.AdminID IS NULL THEN 0 ELSE 1 END AS bit) AS IsRegistered, FR.ReviewRequested, FR.ReviewComplete, FR.SignedOff, FR.FrameworkCommentID, FC1.Comments AS Comment, FR.SignOffRequired
                    FROM   FrameworkReviews AS FR INNER JOIN
                         FrameworkCollaborators AS FC ON FR.FrameworkCollaboratorID = FC.ID LEFT OUTER JOIN
                         FrameworkComments AS FC1 ON FR.FrameworkCommentID = FC1.ID
                    WHERE FR.ID = @reviewId AND FR.FrameworkID = @frameworkId AND FC.AdminID = @adminId AND FR.Archived IS NULL",
                new { frameworkId, adminId, reviewId }).FirstOrDefault();
        }

        public void SubmitFrameworkReview(int frameworkId, int reviewId, bool signedOff, int? commentId)
        {
            var numberOfAffectedRows = connection.Execute(
             @"UPDATE FrameworkReviews
                    SET ReviewComplete = GETUTCDATE(), FrameworkCommentID = @commentId, SignedOff = @signedOff
                    WHERE ID = @reviewId AND FrameworkID = @frameworkId", new { reviewId, commentId, signedOff, frameworkId });
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
                @"SELECT FR.ID, FR.FrameworkID, FR.FrameworkCollaboratorID, FC.UserEmail, CAST(CASE WHEN FC.AdminID IS NULL THEN 0 ELSE 1 END AS bit) AS IsRegistered, FR.ReviewRequested, FR.ReviewComplete, FR.SignedOff, FR.FrameworkCommentID, FC1.Comments AS Comment, FR.SignOffRequired, 
             AU.Forename AS ReviewerFirstName, AU.Surname AS ReviewerLastName, AU1.Forename AS OwnerFirstName, AU1.Email AS OwnerEmail, FW.FrameworkName
FROM   FrameworkReviews AS FR INNER JOIN
             FrameworkCollaborators AS FC ON FR.FrameworkCollaboratorID = FC.ID INNER JOIN
             AdminUsers AS AU ON FC.AdminID = AU.AdminID INNER JOIN
             Frameworks AS FW ON FR.FrameworkID = FW.ID INNER JOIN
             AdminUsers AS AU1 ON FW.OwnerAdminID = AU1.AdminID LEFT OUTER JOIN
             FrameworkComments AS FC1 ON FR.FrameworkCommentID = FC1.ID
WHERE (FR.ID = @reviewId) AND (FR.ReviewComplete IS NOT NULL)",
                new { reviewId }).FirstOrDefault();
        }

        public void UpdateReviewRequestedDate(int reviewId)
        {
            var numberOfAffectedRows = connection.Execute(
            @"UPDATE FrameworkReviews
                    SET ReviewRequested = GETUTCDATE()
                    WHERE ID = @reviewId", new { reviewId });
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
                      SELECT FR1.FrameworkID, FR1.FrameworkCollaboratorId, FR1.SignOffRequired FROM FrameworkReviews AS FR1 WHERE FR1.ID = @reviewId"
                   , new { reviewId });
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
                    WHERE ID = @reviewId", new { reviewId });
            if (numberOfAffectedRows < 1)
            {
                logger.LogWarning(
                    "Not archiving framework review as db update failed. " +
                    $"reviewId: {reviewId}."
                );
            }
        }

        public DashboardData GetDashboardDataForAdminID(int adminId)
        {
            return connection.Query<DashboardData>(
                $@"SELECT (SELECT COUNT(*) 
  FROM [dbo].[Frameworks]) AS FrameworksCount,

  (SELECT COUNT(*) FROM {FrameworkTables} 
WHERE (OwnerAdminID = @adminId) OR
             (@adminId IN
                 (SELECT AdminID
                 FROM    FrameworkCollaborators
                 WHERE (FrameworkID = FW.ID)))) AS MyFrameworksCount,

				 (SELECT COUNT(*) FROM SelfAssessments) AS RoleProfileCount,

				 (SELECT COUNT(*) FROM SelfAssessments AS RP LEFT OUTER JOIN
             SelfAssessmentCollaborators AS RPC ON RPC.SelfAssessmentID = RP.ID AND RPC.AdminID = @adminId 
WHERE (RP.CreatedByAdminID = @adminId) OR
             (@adminId IN
                 (SELECT AdminID
                 FROM    SelfAssessmentCollaborators
                 WHERE (SelfAssessmentID = RP.ID)))) AS MyRoleProfileCount",
                new { adminId }).FirstOrDefault();
        }
        public IEnumerable<DashboardToDoItem> GetDashboardToDoItems(int adminId)
        {
            return connection.Query<DashboardToDoItem>(
                @"SELECT FW.ID AS FrameworkID, 0 AS RoleProfileID, FW.FrameworkName AS ItemName, AU.Forename + ' ' + AU.Surname AS RequestorName, FWR.SignOffRequired, FWR.ReviewRequested AS Requested
FROM   FrameworkReviews AS FWR INNER JOIN
             Frameworks AS FW ON FWR.FrameworkID = FW.ID INNER JOIN
             FrameworkCollaborators AS FWC ON FWR.FrameworkCollaboratorID = FWC.ID INNER JOIN
             AdminUsers AS AU ON FW.OwnerAdminID = AU.AdminID
WHERE (FWC.AdminID = @adminId) AND (FWR.ReviewComplete IS NULL) AND (FWR.Archived IS NULL)
UNION ALL
SELECT 0 AS SelfAssessmentID, RP.ID AS SelfAssessmentID, RP.Name AS ItemName, AU.Forename + ' ' + AU.Surname AS RequestorName, RPR.SignOffRequired, RPR.ReviewRequested AS Requested
FROM   SelfAssessmentReviews AS RPR INNER JOIN
             SelfAssessments AS RP ON RPR.SelfAssessmentID = RP.ID INNER JOIN
             SelfAssessmentCollaborators AS RPC ON RPR.SelfAssessmentCollaboratorID = RPC.ID INNER JOIN
             AdminUsers AS AU ON RP.CreatedByAdminID = AU.AdminID
WHERE (RPC.AdminID = @adminId) AND (RPR.ReviewComplete IS NULL) AND (RPR.Archived IS NULL)", new { adminId }
                );

        }
        public void MoveCompetencyAssessmentQuestion(int competencyId, int assessmentQuestionId, bool singleStep, string direction)
        {
            connection.Execute("ReorderCompetencyAssessmentQuestion", new { competencyId, assessmentQuestionId, direction, singleStep }, commandType: CommandType.StoredProcedure);
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

        public CompetencyResourceAssessmentQuestionParameter? GetCompetencyResourceAssessmentQuestionParameterById(int competencyResourceAssessmentQuestionParameterId)
        {
            var parameter = connection.Query<CompetencyResourceAssessmentQuestionParameter>(
                $@"SELECT *
                    FROM CompetencyResourceAssessmentQuestionParameters
                    WHERE ID = @competencyResourceAssessmentQuestionParameterId",
                    new { competencyResourceAssessmentQuestionParameterId }).FirstOrDefault();
            if (parameter != null)
            {
                var questions = connection.Query<AssessmentQuestion>(
                    $@"SELECT * FROM AssessmentQuestions
                        WHERE ID IN ({parameter.AssessmentQuestionID}, {parameter.RelevanceAssessmentQuestionID ?? parameter.AssessmentQuestionID})");
                parameter.AssessmentQuestion = questions.FirstOrDefault(q => q.ID == parameter.AssessmentQuestionID);
                parameter.RelevanceAssessmentQuestion = questions.FirstOrDefault(q => q.ID == parameter.RelevanceAssessmentQuestionID);
            }
            return parameter;
        }

        public IEnumerable<SignpostingResourceParameter> GetSignpostingResourceParametersByFrameworkAndCompetencyId(int frameworkId, int competencyId)
        {
            return connection.Query<SignpostingResourceParameter>(
                $@"SELECT p.ID, lrr.OriginalResourceName, p.Essential, q.Question, p.MinResultMatch, p.MaxResultMatch, 
                    CASE 
	                    WHEN p.CompareToRoleRequirements = 1 THEN 'Role requirements'  
	                    WHEN p.RelevanceAssessmentQuestionID IS NOT NULL THEN raq.Question
	                    ELSE 'Don''t compare result'
                    END AS CompareResultTo
                    FROM CompetencyResourceAssessmentQuestionParameters AS p
                    INNER JOIN CompetencyLearningResources AS clr ON p.CompetencyLearningResourceID = clr.ID
                    INNER JOIN FrameworkCompetencies AS fc ON fc.CompetencyID = clr.CompetencyID
                    INNER JOIN LearningResourceReferences AS lrr ON clr.LearningResourceReferenceID = lrr.ID
                    INNER JOIN AssessmentQuestions AS q ON p.AssessmentQuestionID = q.ID
                    LEFT JOIN AssessmentQuestions AS raq ON p.RelevanceAssessmentQuestionID = raq.ID
                    WHERE fc.FrameworkID = @FrameworkId AND clr.CompetencyID = @CompetencyId",
                new { frameworkId, competencyId });
        }

        public LearningResourceReference GetLearningResourceReferenceByCompetencyLearningResouceId(int competencyLearningResouceId)
        {
            return connection.Query<LearningResourceReference>(
                $@"SELECT * FROM LearningResourceReferences lrr
                    INNER JOIN CompetencyLearningResources clr ON clr.LearningResourceReferenceID = lrr.ID
                    WHERE clr.ID = @competencyLearningResouceId",
                new { competencyLearningResouceId }).FirstOrDefault();
        }
    }
}
