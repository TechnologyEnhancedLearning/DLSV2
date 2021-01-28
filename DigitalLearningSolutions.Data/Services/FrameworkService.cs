namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using DigitalLearningSolutions.Data.Models.Common;
    using Microsoft.Extensions.Logging;

    public interface IFrameworkService
    {
        //GET DATA
        //  Frameworks:
        DetailFramework? GetFrameworkDetailByFrameworkId(int frameworkId, int adminId);
        BaseFramework? GetBaseFrameworkByFrameworkId(int frameworkId, int adminId);
        BrandedFramework? GetBrandedFrameworkByFrameworkId(int frameworkId, int adminId);
        IEnumerable<BrandedFramework> GetFrameworkByFrameworkName(string frameworkName, int adminId);
        IEnumerable<BrandedFramework> GetFrameworksForAdminId(int adminId);
        IEnumerable<BrandedFramework> GetAllFrameworks(int adminId);
        //  Collaborators:
        IEnumerable<CollaboratorDetail> GetCollaboratorsForFrameworkId(int frameworkId);
        //  Competencies/groups:
        IEnumerable<FrameworkCompetencyGroup> GetFrameworkCompetencyGroups(int frameworkId);
        IEnumerable<FrameworkCompetency> GetFrameworkCompetenciesUngrouped(int frameworkId);
        CompetencyGroupBase GetCompetencyGroupBaseById(int Id);
        FrameworkCompetency GetFrameworkCompetencyById(int Id);
        //  Assessment questions:
        IEnumerable<AssessmentQuestion> GetAllCompetencyQuestions(int adminId);
        IEnumerable<AssessmentQuestion> GetFrameworkDefaultQuestionsById(int frameworkId, int adminId);
        IEnumerable<AssessmentQuestion> GetCompetencyAssessmentQuestionsById(int competencyId, int adminId);
        IEnumerable<AssessmentQuestionInputType> GetAssessmentQuestionInputTypes();
        IEnumerable<GenericSelectList> GetAssessmentQuestions(int frameworkId, int adminId);
        //INSERT DATA
        BrandedFramework CreateFramework(string frameworkName, int adminId);
        int InsertCompetencyGroup(string groupName, int adminId);
        int InsertFrameworkCompetencyGroup(int groupId, int frameworkID, int adminId);
        int InsertCompetency(string name, string description, int adminId);
        int InsertFrameworkCompetency(int competencyId, int? frameworkCompetencyGroupID, int adminId, int frameworkId);
        int AddCollaboratorToFramework(int frameworkId, int adminId, bool canModify);
        void AddFrameworkDefaultQuestion(int frameworkId, int assessmentQuestionId, int adminId, bool addToExisting);
        //UPDATE DATA
        BrandedFramework? UpdateFrameworkBranding(int frameworkId, int brandId, int categoryId, int topicId, int adminId);
        bool UpdateFrameworkName(int frameworkId, int adminId, string frameworkName);
        void UpdateFrameworkCompetencyGroup(int frameworkCompetencyGroupId, int competencyGroupId, string name, int adminId);
        void UpdateFrameworkCompetency(int frameworkCompetencyId, string name, string description, int adminId);
        void MoveFrameworkCompetencyGroup(int frameworkCompetencyGroupId, bool singleStep, string direction);
        void MoveFrameworkCompetency(int frameworkCompetencyId, bool singleStep, string direction);
        //Delete data
        void RemoveCollaboratorFromFramework(int frameworkId, int adminId);
        void DeleteFrameworkCompetencyGroup(int frameworkCompetencyGroupId, int competencyGroupId, int adminId);
        void DeleteFrameworkCompetency(int frameworkCompetencyId, int adminId);
        void DeleteFrameworkDefaultQuestion(int frameworkId, int assessmentQuestionId, int adminId, bool deleteFromExisting);
    }
    public class FrameworkService : IFrameworkService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<FrameworkService> logger;
        private const string BaseFrameworkFields =
            @"ID, FrameworkName, FrameworkConfig, OwnerAdminID,
                 (SELECT Forename + ' ' + Surname AS Expr1
                 FROM    AdminUsers
                 WHERE (AdminID = FW.OwnerAdminID)) AS Owner, BrandID, CategoryID, TopicID, CreatedDate, PublishStatusID,
                 (SELECT Status
                 FROM    PublishStatus
                 WHERE (ID = FW.PublishStatusID)) AS PublishStatus, UpdatedByAdminID,
                 (SELECT Forename + ' ' + Surname AS Expr1
                 FROM    AdminUsers AS AdminUsers_1
                 WHERE (AdminID = FW.UpdatedByAdminID)) AS UpdatedBy, CASE WHEN FW.OwnerAdminID = 1 THEN 1 ELSE COALESCE
                 ((SELECT CanModify
                  FROM    FrameworkCollaborators
                  WHERE FrameworkID = FW.ID AND AdminID = @AdminID), 0) END AS UserCanModify";
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
            @"Frameworks AS FW";
        private const string AssessmentQuestionFields =
            @"SELECT AQ.ID, AQ.Question, AQ.MinValue, AQ.MaxValue, AQ.AssessmentQuestionInputTypeID, AQI.InputTypeName, AQ.AddedByAdminId, CASE WHEN AQ.AddedByAdminId = @adminId THEN 1 ELSE 0 END AS UserIsOwner";
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
                      WHERE (OwnerAdminID = @AdminID) OR
             (@AdminID IN
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

        public BrandedFramework CreateFramework(string frameworkName, int adminId)
        {
            if (frameworkName.Length == 0 | adminId < 1)
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
                @"INSERT INTO Frameworks (FrameworkName, OwnerAdminID, PublishStatusID, UpdatedByAdminID)
                    VALUES (@frameworkName, @adminId, 1, @adminId)",
               new { frameworkName, adminId }
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
        public int InsertCompetency(string name, string description, int adminId)
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

                return existingId;
            }
        }

        public IEnumerable<CollaboratorDetail> GetCollaboratorsForFrameworkId(int frameworkId)
        {
            return connection.Query<CollaboratorDetail>(
                $@"SELECT fw.ID AS FrameworkID, au1.AdminID AS AdminID, 1 AS CanModify, au1.Email, au1.Forename, au1.Surname, 'Owner' AS FrameworkRole
                    FROM   Frameworks AS fw INNER JOIN
                        AdminUsers AS au1 ON fw.OwnerAdminID = au1.AdminID
                    WHERE (fw.ID = @FrameworkID)
                    UNION ALL
                   SELECT fwc.FrameworkID, fwc.AdminID, fwc.CanModify, au.Email, au.Forename, au.Surname, CASE WHEN fwc.CanModify = 1 THEN 'Contributor' ELSE 'Reviewer' END AS FrameworkRole
                    FROM   FrameworkCollaborators AS fwc INNER JOIN
                        AdminUsers AS au ON fwc.AdminID = au.AdminID
                    WHERE (fwc.FrameworkID = @FrameworkID) AND (au.Active=1)", new { frameworkId });
        }

        public int AddCollaboratorToFramework(int frameworkId, int adminId, bool canModify)
        {
            int existingId = (int)connection.ExecuteScalar(
               @"SELECT COALESCE
                 ((SELECT AdminID
                  FROM    FrameworkCollaborators
                  WHERE (FrameworkID = @frameworkId) AND (AdminID = @adminId)), 0) AS AdminID",
               new { frameworkId, adminId });
            if (existingId > 0)
            {
                return -2;
            }
            else
            {
                var numberOfAffectedRows = connection.Execute(
                             @"INSERT INTO FrameworkCollaborators (FrameworkID, AdminID, CanModify)
                    VALUES (@frameworkId, @adminId, @canModify)",
                            new { frameworkId, adminId, canModify });
                if (numberOfAffectedRows < 1)
                {
                    logger.LogWarning(
                        $"Not inserting framework collaborator as db insert failed. AdminId: {adminId}, frameworkId: {frameworkId}, canModify: {canModify}"
                    );
                    return -1;
                }
                existingId = (int)connection.ExecuteScalar(
                 @"SELECT COALESCE
                 ((SELECT AdminID
                  FROM    FrameworkCollaborators
                  WHERE (FrameworkID = @frameworkId) AND (AdminID = @adminId)), 0) AS AdminID",
               new { frameworkId, adminId });
                return existingId;
            }
        }
        public void RemoveCollaboratorFromFramework(int frameworkId, int adminId)
        {
            connection.Execute(
                             @"DELETE FROM  FrameworkCollaborators WHERE (FrameworkID = @frameworkId) AND (AdminID = @adminId)",
                            new { frameworkId, adminId });
        }

        public IEnumerable<FrameworkCompetencyGroup> GetFrameworkCompetencyGroups(int frameworkId)
        {
            var result = connection.Query<FrameworkCompetencyGroup, FrameworkCompetency, FrameworkCompetencyGroup>(
                @"SELECT fcg.ID, fcg.CompetencyGroupID, cg.Name, fcg.Ordering, fc.ID, c.Name, c.Description, fc.Ordering
                    FROM   FrameworkCompetencyGroups AS fcg INNER JOIN
                        CompetencyGroups AS cg ON fcg.CompetencyGroupID = cg.ID LEFT OUTER JOIN
                        FrameworkCompetencies AS fc ON fcg.ID = fc.FrameworkCompetencyGroupID LEFT OUTER JOIN
                        Competencies AS c ON fc.CompetencyID = c.ID
                    WHERE (fcg.FrameworkID = @frameworkId)
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
                @"SELECT fc.ID, c.Name, c.Description, fc.Ordering
                	FROM FrameworkCompetencies AS fc 
                		INNER JOIN Competencies AS c ON fc.CompetencyID = c.ID
                	WHERE fc.FrameworkID = @frameworkId 
                		AND fc.FrameworkCompetencyGroupID IS NULL
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
        public CompetencyGroupBase GetCompetencyGroupBaseById(int Id)
        {
            return connection.QueryFirstOrDefault<CompetencyGroupBase>(
                @"SELECT fcg.ID, fcg.CompetencyGroupID, cg.Name
                    FROM   FrameworkCompetencyGroups AS fcg
                        INNER JOIN CompetencyGroups AS cg ON fcg.CompetencyGroupID = cg.ID
                    WHERE (fcg.ID = @Id)", new { Id }
                );
        }
        public FrameworkCompetency GetFrameworkCompetencyById(int Id)
        {
            return connection.QueryFirstOrDefault<FrameworkCompetency>(
                 @"SELECT fc.ID, c.Name, c.Description, fc.Ordering
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
        public void UpdateFrameworkCompetency(int frameworkCompetencyId, string name, string description, int adminId)
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
                    @"DELETE FROM Competencies WHERE ID = @competencyId", new { competencyId }
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
                    WHERE FDQ.FrameworkId = @frameworkId
                     ORDER BY [Question]"
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
            }
            else if(addToExisting)
            {
                numberOfAffectedRows = connection.Execute(
                    @"INSERT INTO CompetencyAssessmentQuestions (CompetencyID, AssessmentQuestionID)
                        SELECT CompetencyID, @assessmentQuestionId
                        FROM FrameworkCompetencies
                        WHERE FrameworkID = @frameworkId
                        EXCEPT
                        SELECT CompetencyID, AssessmentQuestionID
                        FROM CompetencyAssessmentQuestions",
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
            }
            else if(deleteFromExisting)
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
                    WHERE AQ.ID NOT IN (SELECT AssessmentQuestionID FROM FrameworkDefaultQuestions WHERE FrameworkId = @frameworkId)", new {frameworkId, adminId}
                );
        }
        public IEnumerable<AssessmentQuestionInputType> GetAssessmentQuestionInputTypes()
        {
            return connection.Query<AssessmentQuestionInputType>(
                @"SELECT ID, InputTypeName
                    FROM AssessmentQuestionInputTypes"
                );
        }
    }
}
