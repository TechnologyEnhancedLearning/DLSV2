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
        DetailFramework? GetFrameworkDetailByFrameworkId(int frameworkId);
        IEnumerable<BaseFramework> GetFrameworksForAdminId(int adminId);
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
        private const string DetailFrameworkFields =
            @",FW.Description
            ,(SELECT BrandName
                                 FROM    Brands
                                 WHERE (BrandID = FW.BrandID)) AS Brand,
                                 (SELECT CategoryName
                                 FROM    CourseCategories
                                 WHERE (CourseCategoryID = FW.CategoryID)) AS Category,
                                 (SELECT CourseTopic
                 FROM    CourseTopics
                                 WHERE (CourseTopicID = FW.TopicID)) AS Topic
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
        public DetailFramework? GetFrameworkDetailByFrameworkId(int frameworkId)
        {
            return connection.QueryFirstOrDefault<DetailFramework>(
               $@"SELECT {BaseFrameworkFields} {DetailFrameworkFields}
                      FROM {FrameworkTables}
                      WHERE FW.ID = @frameworkId",
               new { frameworkId }
           );
        }

        public IEnumerable<BaseFramework> GetFrameworksForAdminId(int adminId)
        {
            return connection.Query<BaseFramework>(
                $@"SELECT {BaseFrameworkFields} {DetailFrameworkFields}
                      FROM {FrameworkTables}
                      WHERE FW.OwnerAdminID = @AdminID OR FWC.AdminID = @AdminID",
               new { adminId }
           );
        }
    }
}
