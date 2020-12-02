namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Frameworks;
    using Microsoft.Extensions.Logging;

    public interface IFrameworkService
    {
        DetailFramework? GetFrameworkDetailByFrameworkId(int frameworkId, int adminId);
        IEnumerable<BrandedFramework> GetFrameworkDetailByFrameworkName(string frameworkName, int adminId);
        IEnumerable<BrandedFramework> GetFrameworksForAdminId(int adminId);
        IEnumerable<BrandedFramework> GetAllFrameworks(int adminId);
        Int32 CreateFramework(string frameworkName, int adminId);
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
               new { frameworkId }
           );
        }
        public IEnumerable<BrandedFramework> GetFrameworkDetailByFrameworkName(string frameworkName, int adminId)
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

        public Int32 CreateFramework(string frameworkName, int adminId)
        {
            if (frameworkName.Length == 0 )
            {
                logger.LogWarning(
                    "Not inserting framework as it frameworkname. AdminId: " + adminId
                ) ;
                return -2;
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
                return -1;
            }
            var frameworkId = connection.ExecuteScalar(
                @"SELECT ID FROM Frameworks WHERE FrameworkName = @frameworkName AND OwnerAdminID = @adminId",
                new { frameworkName, adminId }
                );
            return Convert.ToInt32(frameworkId);
        }
    }
}
