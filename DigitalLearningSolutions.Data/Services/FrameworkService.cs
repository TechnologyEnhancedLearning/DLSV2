namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using Microsoft.Extensions.Logging;

    public interface IFrameworkService
    {
        //Retrieve data
        DetailFramework? GetFrameworkDetailByFrameworkId(int frameworkId, int adminId);
        BaseFramework? GetBaseFrameworkByFrameworkId(int frameworkId, int adminId);
        BrandedFramework? GetBrandedFrameworkByFrameworkId(int frameworkId, int adminId);
        IEnumerable<BrandedFramework> GetFrameworkByFrameworkName(string frameworkName, int adminId);
        IEnumerable<BrandedFramework> GetFrameworksForAdminId(int adminId);
        IEnumerable<BrandedFramework> GetAllFrameworks(int adminId);
        BrandedFramework CreateFramework(string frameworkName, int adminId);
        IEnumerable<CollaboratorDetail> GetCollaboratorsForFrameworkId(int frameworkId);
        IEnumerable<FrameworkCompetencyGroup> GetFrameworkCompetencyGroups(int frameworkId);
        IEnumerable<FrameworkCompetency> GetFrameworkCompetenciesUngrouped(int frameworkId);
        //Insert data
        int InsertCompetencyGroup(string groupName, int adminId);
        int InsertFrameworkCompetencyGroup(int groupId, int frameworkID, int adminId);
        int InsertCompetency(string description, int competencyGroupId, int adminId);
        int InsertFrameworkCompetency(int competencyId, int frameworkCompetencyGroupID, int adminId);
        int AddCollaboratorToFramework(int frameworkId, int adminId, bool canModify);
        //Update data
        BrandedFramework? UpdateFrameworkBranding(int frameworkId, int brandId, int categoryId, int topicId, int adminId);
        bool UpdateFrameworkName(int frameworkId, int adminId, string frameworkName);
        //Delete data
        void RemoveCollaboratorFromFramework(int frameworkId, int adminId);
    }
    public class FrameworkService : IFrameworkService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<FrameworkService> logger;
        private const string BaseFrameworkFields =
            @"ID
            ,FW.FrameworkName
            ,FW.FrameworkConfig
            ,FW.OwnerAdminID
	        ,(SELECT Forename + ' ' + Surname FROM AdminUsers WHERE AdminID = FW.OwnerAdminID) AS Owner
            ,FW.BrandID
            ,FW.CategoryID
            ,FW.TopicID
            ,FW.CreatedDate
            ,FW.PublishStatusID
            ,(SELECT [Status] FROM PublishStatus WHERE ID = FW.PublishStatusID) AS PublishStatus
            ,FW.UpdatedByAdminID
	        ,(SELECT Forename + ' ' + Surname FROM AdminUsers WHERE AdminID = FW.UpdatedByAdminID) AS UpdatedBy
	        , CASE WHEN FW.OwnerAdminID = @AdminID THEN 1 ELSE FWC.CanModify END AS UserCanModify";
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
            FrameworkCollaborators AS FWC
            ON FWC.FrameworkID = FW.ID";
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
                      WHERE FW.OwnerAdminID = @AdminID OR FWC.AdminID = @AdminID",
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
                @"SELECT COALESCE (SELECT CompetencyGroupID FROM CompetencyGroups WHERE [Name] = @groupName), 0) AS CompetencyGroupID",
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
                 @"SELECT COALESCE (SELECT CompetencyGroupID FROM CompetencyGroups WHERE [Name] = @groupName), 0) AS CompetencyGroupID",
                 new { groupName });
                return existingId;
            }
        }
        public int InsertFrameworkCompetencyGroup(int groupId, int frameworkID, int adminId)
        {
            if (groupId < 1 | frameworkID < 1 | adminId < 1)
            {
                logger.LogWarning(
                    $"Not inserting framework competency group as it failed server side validation. AdminId: {adminId}, frameworkId: {frameworkID}, groupId: {groupId}"
                );
                return -2;
            }
            int existingId = (int)connection.ExecuteScalar(
                 @"SELECT COALESCE (SELECT CompetencyGroupID FROM FrameworkCompetencyGroups WHERE CompetencyGroupID = @groupID AND FrameworkID = @frameworkID), 0) AS FrameworkCompetencyGroupID",
                 new { groupId, frameworkID });
            if (existingId > 0)
            {
                return existingId;
            }
            else
            {
                var numberOfAffectedRows = connection.Execute(
                              @"INSERT INTO FrameworkCompetencyGroups (CompetencyGroupID, UpdatedByAdminID, Ordering)
                    VALUES (@groupName, @adminId, COALESCE
                             ((SELECT        MAX(Ordering) AS OrderNum
                                 FROM            [FrameworkCompetencyGroups]
                                 WHERE        ([CompetencyGroupID] = @groupID)) + 1, 1)))",
                             new { groupId, adminId });
                if (numberOfAffectedRows < 1)
                {
                    logger.LogWarning(
                        "Not inserting framework competency group as db insert failed. " +
                        $"Group id: {groupId}, admin id: {adminId}"
                    );
                    return -1;
                }
                existingId = (int)connection.ExecuteScalar(
                 @"SELECT COALESCE (SELECT CompetencyGroupID FROM FrameworkCompetencyGroups WHERE CompetencyGroupID = @groupID AND FrameworkID = @frameworkID), 0) AS FrameworkCompetencyGroupID",
                 new { groupId, frameworkID });
                return existingId;
            }
        }
        public int InsertCompetency(string description, int groupId, int adminId)
        {
            if (description.Length == 0 | groupId < 1 | adminId < 1)
            {
                logger.LogWarning(
                    $"Not inserting competency as it failed server side valiidation. AdminId: {adminId}, groupId: {groupId}, description:{description}"
                );
                return -2;
            }
            int existingId = (int)connection.ExecuteScalar(
                @"SELECT COALESCE (SELECT CompetencyID FROM Competencies WHERE [Description] = @description AND CompetencyGroupID = @groupId), 0) AS CompetencyID",
                new { description, groupId });
            if (existingId > 0)
            {
                return existingId;
            }
            else
            {
                var numberOfAffectedRows = connection.Execute(
                              @"INSERT INTO Competencies ([Description], UpdatedByAdminID, CompetencyGroupID)
                    VALUES (@description, @adminId, @groupId)",
                             new { description, adminId });
                if (numberOfAffectedRows < 1)
                {
                    logger.LogWarning(
                        $"Not inserting competency group as db insert failed. AdminId: {adminId}, groupId: {groupId}, description:{description}"
                    );
                    return -1;
                }
                existingId = (int)connection.ExecuteScalar(
                 @"SELECT COALESCE (SELECT CompetencyID FROM Competencies WHERE [Description] = @description AND CompetencyGroupID = @groupId), 0) AS CompetencyID",
                 new { description, groupId, adminId });
                return existingId;
            }
        }
        public int InsertFrameworkCompetency(int competencyId, int frameworkCompetencyGroupID, int adminId)
        {
            if (frameworkCompetencyGroupID < 1 | competencyId < 1 | adminId < 1)
            {
                logger.LogWarning(
                    $"Not inserting competency as it failed server side valiidation. AdminId: {adminId}, frameworkCompetencyGroupID: {frameworkCompetencyGroupID}, competencyId:{competencyId}"
                );
                return -2;
            }
            int existingId = (int)connection.ExecuteScalar(
               @"SELECT COALESCE (SELECT FrameworkCompetencyID FROM FrameworkCompetencies WHERE [CompetencyID] = @competencyId AND FrameworkCompetencyGroupID = @frameworkCompetencyGroupID), 0) AS FrameworkCompetencyID",
               new { competencyId, frameworkCompetencyGroupID });
            if (existingId > 0)
            {
                return existingId;
            }
            else
            {
                var numberOfAffectedRows = connection.Execute(
                             @"INSERT INTO FrameworkCompetencies ([CompetencyID], FrameworkCompetencyGroupID, AdminID, Ordering)
                    VALUES (@competencyId, @frameworkCompetencyGroupID, @adminId, , COALESCE
                             ((SELECT        MAX(Ordering) AS OrderNum
                                 FROM            [FrameworkCompetencies]
                                 WHERE        ([FrameworkCompetencyGroupID] = @frameworkCompetencyGroupID)) + 1, 1)))",
                            new { competencyId, frameworkCompetencyGroupID, adminId });
                if (numberOfAffectedRows < 1)
                {
                    logger.LogWarning(
                        $"Not inserting framework competency as db insert failed. AdminId: {adminId}, frameworkCompetencyGroupID: {frameworkCompetencyGroupID}, competencyId: {competencyId}"
                    );
                    return -1;
                }
                existingId = (int)connection.ExecuteScalar(
                 @"SELECT COALESCE (SELECT FrameworkCompetencyID FROM FrameworkCompetencies WHERE [CompetencyID] = @competencyId AND FrameworkCompetencyGroupID = @frameworkCompetencyGroupID), 0) AS FrameworkCompetencyID",
               new { competencyId, frameworkCompetencyGroupID });
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
            return connection.Query<FrameworkCompetencyGroup, FrameworkCompetency, FrameworkCompetencyGroup>(
                @"SELECT fcg.ID, fcg.CompetencyGroupID, cg.Name, fcg.Ordering, fc.ID, c.Description, fc.Ordering
                    FROM FrameworkCompetencyGroups AS fcg 
                        INNER JOIN CompetencyGroups AS cg ON fcg.CompetencyGroupID = cg.ID 
                        LEFT OUTER JOIN FrameworkCompetencies AS fc ON fcg.ID = fc.FrameworkCompetencyGroupID 
                        INNER JOIN Competencies AS c ON fc.CompetencyID = c.ID
                    WHERE fcg.FrameworkID = @frameworkId",
                (frameworkCompetencyGroup, frameworkCompetency) =>
                {
                    frameworkCompetencyGroup.FrameworkCompetencies.Add(frameworkCompetency);
                    return frameworkCompetencyGroup;
                   },
              param: new { frameworkId }
           );
        }
        public IEnumerable<FrameworkCompetency> GetFrameworkCompetenciesUngrouped(int frameworkId)
        {
            return connection.Query<FrameworkCompetency>(
                @"SELECT fc.ID, c.Description, fc.Ordering
                	FROM FrameworkCompetencies AS fc 
                		INNER JOIN Competencies AS c ON fc.CompetencyID = c.ID
                	WHERE fc.FrameworkID = @frameworkId 
                		AND fc.FrameworkCompetencyGroupID IS NULL",
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
    }
}
