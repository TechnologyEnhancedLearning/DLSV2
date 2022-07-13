namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Common;
    using Microsoft.Extensions.Logging;

    public interface ICommonService
    {
        //GET DATA
        IEnumerable<Brand> GetBrandListForCentre(int centreId);
        IEnumerable<Category> GetCategoryListForCentre(int centreId);
        IEnumerable<Topic> GetTopicListForCentre(int centreId);
        string? GetBrandNameById(int brandId);
        string? GetCategoryNameById(int categoryId);
        string? GetTopicNameById(int topicId);

        //INSERT DATA
        int InsertBrandAndReturnId(string brandName, int centreId);
        int InsertCategoryAndReturnId(string categoryName, int centreId);
        int InsertTopicAndReturnId(string topicName, int centreId);

    }
    public class CommonService : ICommonService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<CommonService> logger;
        public CommonService(IDbConnection connection, ILogger<CommonService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }
        public IEnumerable<Brand> GetBrandListForCentre(int centreId)
        {
            return connection.Query<Brand>(
                @"SELECT        BrandID, BrandName
                    FROM            Brands
                    WHERE        (Active = 1) AND (IncludeOnLanding = 1) OR
                         (Active = 1) AND ((OwnerOrganisationID = @centreId) OR (BrandID = 6))
                    ORDER BY BrandName",
               new { centreId }
           );
        }
        public IEnumerable<Category> GetCategoryListForCentre(int centreId)
        {
            return connection.Query<Category>(
                @"SELECT        CourseCategoryID, CategoryName
                    FROM            CourseCategories
                    WHERE        ((CentreID = @CentreID) OR (CourseCategoryID = 1)) AND (Active = 1)
                    ORDER BY CategoryName",
               new { centreId }
           );
        }
        public IEnumerable<Topic> GetTopicListForCentre(int centreId)
        {
            return connection.Query<Topic>(
                @"SELECT        CourseTopicID, CourseTopic
                    FROM            CourseTopics
                    WHERE        ((CentreID = @CentreID) OR (CourseTopicID = 1)) AND (Active = 1)
                    ORDER BY CourseTopic",
               new { centreId }
           );
        }
        private const string GetBrandID = @"SELECT COALESCE ((SELECT BrandID FROM Brands WHERE [BrandName] = @brandName), 0) AS BrandID";
        public int InsertBrandAndReturnId(string brandName, int centreId)
        {
            if (brandName.Length == 0 | centreId < 1)
            {
                logger.LogWarning(
                    $"Not inserting brand as it failed server side validation. centreId: {centreId}, brandName: {brandName}"
                );
                return -2;
            }
            int existingId = (int)connection.ExecuteScalar(GetBrandID,
                new { brandName });
            if (existingId > 0)
            {
                return existingId;
            }
            else
            {
                var numberOfAffectedRows = connection.Execute(
                              @"INSERT INTO Brands ([BrandName], OwnerOrganisationID)
                    VALUES (@brandName, @centreId)",
                             new { brandName, centreId });
                if (numberOfAffectedRows < 1)
                {
                    logger.LogWarning(
                        "Not inserting brand as db insert failed. " +
                        $"centreId: {centreId}, brandName: {brandName}"
                    );
                    return -1;
                }
                int newBrandId = (int)connection.ExecuteScalar(
                GetBrandID,
                 new { brandName });
                return newBrandId;
            }
        }
        private const string GetCategoryID = @"SELECT COALESCE ((SELECT CourseCategoryID FROM CourseCategories WHERE [CategoryName] = @categoryName), 0) AS CategoryID";
        public int InsertCategoryAndReturnId(string categoryName, int centreId)
        {
            if (categoryName.Length == 0 | centreId < 1)
            {
                logger.LogWarning(
                    $"Not inserting category as it failed server side validation. centreId: {centreId}, categoryName: {categoryName}"
                );
                return -2;
            }

            int existingId = (int)connection.ExecuteScalar(GetCategoryID,
                new { categoryName });
            if (existingId > 0)
            {
                return existingId;
            }
            else
            {
                var numberOfAffectedRows = connection.Execute(
                              @"INSERT INTO CourseCategories ([CategoryName], CentreID)
                    VALUES (@categoryName, @centreId)",
                             new { categoryName, centreId });
                if (numberOfAffectedRows < 1)
                {
                    logger.LogWarning(
                        $"Not inserting category as db insert failed. centreId: {centreId}, categoryName: {categoryName}"
                    );
                    return -1;
                }
                int newCategoryId = (int)connection.ExecuteScalar(GetCategoryID,
                 new { categoryName });
                return newCategoryId;
            }
        }
        private const string GetTopicID = @"SELECT COALESCE ((SELECT CourseTopicID FROM CourseTopics WHERE [CourseTopic] = @topicName), 0) AS TopicID";
        public int InsertTopicAndReturnId(string topicName, int centreId)
        {
            if (topicName.Length == 0 | centreId < 1)
            {
                logger.LogWarning(
                    $"Not inserting topic as it failed server side validation. centreId: {centreId}, topicName: {topicName}"
                );
                return -2;
            }
            int existingId = (int)connection.ExecuteScalar(GetTopicID,
                new { topicName });
            if (existingId > 0)
            {
                return existingId;
            }
            else
            {
                var numberOfAffectedRows = connection.Execute(
                              @"INSERT INTO CourseTopics ([CourseTopic], CentreID)
                    VALUES (@topicName, @centreId)",
                             new { topicName, centreId });
                if (numberOfAffectedRows < 1)
                {
                    logger.LogWarning(
                        "Not inserting brand as db insert failed. " +
                        $"centreId: {centreId}, topicName: {topicName}"
                    );
                    return -1;
                }
                int newTopicId = (int)connection.ExecuteScalar(
                GetTopicID,
                 new { topicName });
                return newTopicId;
            }
        }

        public string? GetBrandNameById(int brandId)
        {
            return (string?)connection.ExecuteScalar(
               @"SELECT       BrandName
                    FROM            Brands
                    WHERE        BrandID = @brandId",
              new { brandId }
          );
        }
        public string? GetCategoryNameById(int categoryId)
        {
            return (string?)connection.ExecuteScalar(
                @"SELECT         CategoryName
                    FROM            CourseCategories
                    WHERE        CourseCategoryID = @categoryId",
               new { categoryId }
           );
        }
        public string? GetTopicNameById(int topicId)
        {
            return (string?)connection.ExecuteScalar(
                @"SELECT        CourseTopic
                    FROM            CourseTopics
                    WHERE        CourseTopicID = @topicId",
               new { topicId }
           );
        }
    }
}
