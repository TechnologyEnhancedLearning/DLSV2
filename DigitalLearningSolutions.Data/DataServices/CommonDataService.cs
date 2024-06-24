namespace DigitalLearningSolutions.Data.DataServices
{
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Common;
    using Microsoft.Extensions.Logging;
    using System.Collections.Generic;
    using System.Data;

    public interface ICommonDataService
    {
        //GET DATA
        IEnumerable<Brand> GetBrandListForCentre(int centreId);
        IEnumerable<Category> GetCategoryListForCentre(int centreId);
        IEnumerable<Topic> GetTopicListForCentre(int centreId);
        IEnumerable<Brand> GetAllBrands();
        IEnumerable<Category> GetAllCategories();
        IEnumerable<Topic> GetAllTopics();
        IEnumerable<(int, string)> GetCoreCourseCategories();
        IEnumerable<(int, string)> GetCentreTypes();
        IEnumerable<(int, string)> GetSelfAssessmentBrands(bool supervised);
        IEnumerable<(int, string)> GetSelfAssessmentCategories(bool supervised);
        IEnumerable<(int, string)> GetSelfAssessmentCentreTypes(bool supervised);
        IEnumerable<(int, string)> GetSelfAssessmentRegions(bool supervised);
        IEnumerable<(int, string)> GetAllRegions();
        IEnumerable<(int, string)> GetSelfAssessments(bool supervised);
        IEnumerable<(int, string)> GetSelfAssessmentCentres(bool supervised);
        IEnumerable<(int, string)> GetCourseCentres();
        IEnumerable<(int, string)> GetCoreCourseBrands();
        IEnumerable<(int, string)> GetCoreCourses();
        string? GetBrandNameById(int brandId);
        string? GetApplicationNameById(int applicationId);
        string? GetCategoryNameById(int categoryId);
        string? GetTopicNameById(int topicId);
        string? GenerateCandidateNumber(string firstName, string lastName);
        string? GetCentreTypeNameById(int centreTypeId);
        //INSERT DATA
        int InsertBrandAndReturnId(string brandName, int centreId);
        int InsertCategoryAndReturnId(string categoryName, int centreId);
        int InsertTopicAndReturnId(string topicName, int centreId);

    }
    public class CommonDataService : ICommonDataService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<CommonDataService> logger;
        private string GetSelfAssessmentWhereClause(bool supervised)
        {
            return supervised ? " (sa.SupervisorResultsReview = 1 OR SupervisorSelfAssessmentReview = 1)" : " (sa.SupervisorResultsReview = 0 AND SupervisorSelfAssessmentReview = 0)";
        }
        public CommonDataService(IDbConnection connection, ILogger<CommonDataService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }
        public IEnumerable<Brand> GetBrandListForCentre(int centreId)
        {
            return connection.Query<Brand>(
                @"SELECT        BrandID, BrandName
                    FROM            Brands WITH (NOLOCK)
                    WHERE        (Active = 1) AND (IncludeOnLanding = 1) OR
                         (Active = 1) AND ((OwnerOrganisationID = @centreId) OR (BrandID = 6))
                    ORDER BY BrandName",
               new { centreId }
           );
        }
        public IEnumerable<Brand> GetAllBrands()
        {
            return connection.Query<Brand>(
                @"SELECT        BrandID, BrandName
                    FROM            Brands WITH (NOLOCK)
                    WHERE        
                         (Active = 1)
                    ORDER BY BrandName"
           );
        }
        public IEnumerable<Category> GetCategoryListForCentre(int centreId)
        {
            return connection.Query<Category>(
                @"SELECT        CourseCategoryID, CategoryName
                    FROM            CourseCategories WITH (NOLOCK)
                    WHERE        ((CentreID = @CentreID) OR (CourseCategoryID = 1)) AND (Active = 1)
                    ORDER BY CategoryName",
               new { centreId }
           );
        }
        public IEnumerable<Category> GetAllCategories()
        {
            return connection.Query<Category>(
                @"SELECT        CourseCategoryID, CategoryName
                    FROM            CourseCategories WITH (NOLOCK)
                    WHERE        (Active = 1)
                    ORDER BY CategoryName"
           );
        }
        public IEnumerable<Topic> GetTopicListForCentre(int centreId)
        {
            return connection.Query<Topic>(
                @"SELECT        CourseTopicID, CourseTopic
                    FROM            CourseTopics WITH (NOLOCK)
                    WHERE        ((CentreID = @CentreID) OR (CourseTopicID = 1)) AND (Active = 1)
                    ORDER BY CourseTopic",
               new { centreId }
           );
        }
        public IEnumerable<Topic> GetAllTopics()
        {
            return connection.Query<Topic>(
                @"SELECT        CourseTopicID, CourseTopic
                    FROM            CourseTopics WITH (NOLOCK)
                    WHERE        (Active = 1)
                    ORDER BY CourseTopic"
                       );
        }

        public IEnumerable<(int, string)> GetCentreTypes()
        {
            return connection.Query<(int, string)>(
                @"SELECT CentreTypeID, CentreType
                    FROM   CentreTypes WITH (NOLOCK)
                    ORDER BY CentreType"
                      );
        }
        public IEnumerable<(int, string)> GetSelfAssessmentBrands(bool supervised)
        {
            var whereClause = GetSelfAssessmentWhereClause(supervised);
            return connection.Query<(int, string)>(
                $@"SELECT b.BrandID, b.BrandName
                    FROM   Brands AS b WITH (NOLOCK) INNER JOIN
                                 SelfAssessments AS sa WITH (NOLOCK) ON b.BrandID = sa.BrandID
                    WHERE (b.Active = 1) AND
                                 (sa.ArchivedDate IS NULL) AND (sa.[National] = 1) AND {whereClause}
                    GROUP BY b.BrandID, b.BrandName
                    ORDER BY b.BrandName"
           );
        }

        public IEnumerable<(int, string)> GetSelfAssessmentCategories(bool supervised)
        {
            var whereClause = GetSelfAssessmentWhereClause(supervised);
            return connection.Query<(int, string)>(
                $@"SELECT cc.CourseCategoryID, cc.CategoryName
                    FROM   CourseCategories AS cc WITH (NOLOCK) INNER JOIN
                                 SelfAssessments AS sa ON cc.CourseCategoryID = sa.CategoryID
                    WHERE (cc.Active = 1) AND (sa.ArchivedDate IS NULL) AND (sa.[National] = 1) AND {whereClause}
                    GROUP BY cc.CourseCategoryID, cc.CategoryName
                    ORDER BY cc.CategoryName"
           );
        }

        public IEnumerable<(int, string)> GetSelfAssessmentCentreTypes(bool supervised)
        {
            var whereClause = GetSelfAssessmentWhereClause(supervised);
            return connection.Query<(int, string)>(
                $@"SELECT ct.CentreTypeID, ct.CentreType AS CentreTypeName
                    FROM   Centres AS c WITH (NOLOCK) INNER JOIN
                                CentreSelfAssessments AS csa WITH (NOLOCK) ON c.CentreID = csa.CentreID INNER JOIN
                                SelfAssessments AS sa WITH (NOLOCK) ON csa.SelfAssessmentID = sa.ID INNER JOIN
                                CentreTypes AS ct WITH (NOLOCK) ON c.CentreTypeID = ct.CentreTypeID
                    WHERE (sa.[National] = 1) AND (sa.ArchivedDate IS NULL) AND {whereClause}
                    GROUP BY ct.CentreTypeID, ct.CentreType
                    ORDER BY CentreTypeName"
                      );
        }
        public IEnumerable<(int, string)> GetSelfAssessmentRegions(bool supervised)
        {
            var whereClause = GetSelfAssessmentWhereClause(supervised);
            return connection.Query<(int, string)>(
                $@"SELECT r.RegionID AS ID, r.RegionName AS Label
                    FROM   Regions AS r WITH (NOLOCK) INNER JOIN
                                 Centres AS c WITH (NOLOCK) ON r.RegionID = c.RegionID INNER JOIN
                                 CentreSelfAssessments AS csa WITH (NOLOCK) ON c.CentreID = csa.CentreID INNER JOIN
                                 SelfAssessments AS sa WITH (NOLOCK) ON csa.SelfAssessmentID = sa.ID
                    WHERE (sa.[National] = 1) AND (sa.ArchivedDate IS NULL) AND {whereClause}
                    GROUP BY r.RegionID, r.RegionName
                    ORDER BY Label"
                      );
        }

        public IEnumerable<(int, string)> GetSelfAssessments(bool supervised)
        {
            var whereClause = GetSelfAssessmentWhereClause(supervised);
            return connection.Query<(int, string)>(
                $@"SELECT ID, Name AS Label
                    FROM   SelfAssessments AS sa WITH (NOLOCK)
                    WHERE ([National] = 1) AND (ArchivedDate IS NULL) AND {whereClause}
                    GROUP BY ID, Name
                    ORDER BY Label"
                      );
        }

        public IEnumerable<(int, string)> GetSelfAssessmentCentres(bool supervised)
        {
            var whereClause = GetSelfAssessmentWhereClause(supervised);
            return connection.Query<(int, string)>(
                $@"SELECT c.CentreID AS ID, c.CentreName AS Label
                    FROM   Centres AS c WITH (NOLOCK) INNER JOIN
                                 CentreSelfAssessments AS csa WITH (NOLOCK) ON c.CentreID = csa.CentreID INNER JOIN
                                 SelfAssessments AS sa WITH (NOLOCK) ON csa.SelfAssessmentID = sa.ID
                    WHERE (sa.[National] = 1) AND (sa.ArchivedDate IS NULL) AND {whereClause}
                    GROUP BY c.CentreID, c.CentreName
                    ORDER BY Label"
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
                    FROM            Brands WITH (NOLOCK)
                    WHERE        BrandID = @brandId",
              new { brandId }
          );
        }
        public string? GetCategoryNameById(int categoryId)
        {
            return (string?)connection.ExecuteScalar(
                @"SELECT         CategoryName
                    FROM            CourseCategories WITH (NOLOCK)
                    WHERE        CourseCategoryID = @categoryId",
               new { categoryId }
           );
        }
        public string? GetTopicNameById(int topicId)
        {
            return (string?)connection.ExecuteScalar(
                @"SELECT        CourseTopic
                    FROM            CourseTopics WITH (NOLOCK)
                    WHERE        CourseTopicID = @topicId",
               new { topicId }
           );
        }
        public string? GetCentreTypeNameById(int centreTypeId)
        {
            return (string?)connection.ExecuteScalar(
                @"SELECT        CentreType
                    FROM            CentreTypes WITH (NOLOCK)
                    WHERE        CentreTypeID = @centreTypeId",
               new { centreTypeId }
           );
        }
        public string? GenerateCandidateNumber(string firstName, string lastName)
        {
            string initials = "";
            if (firstName != null) initials = (firstName.Substring(0, 1)).ToUpper();
            if (lastName != null) initials += (lastName.Substring(0, 1)).ToUpper();


            var candidateNumber = connection.QueryFirst<string>(
                @"DECLARE @_MaxCandidateNumber AS integer
                        SET @_MaxCandidateNumber = (SELECT TOP (1) CONVERT(int, SUBSTRING(CandidateNumber, 3, 250)) AS nCandidateNumber
                        FROM DelegateAccounts
                        WHERE (LEFT(CandidateNumber, 2) = @initials)
                        ORDER BY nCandidateNumber DESC)
                        IF @_MaxCandidateNumber IS Null
                            BEGIN
                            SET @_MaxCandidateNumber = 0
                            END
                        SELECT @initials + CONVERT(varchar(100), @_MaxCandidateNumber + 1)",
                new { initials });
            return candidateNumber;
        }

        public IEnumerable<(int, string)> GetAllRegions()
        {
            return connection.Query<(int, string)>(
                $@"SELECT r.RegionID AS ID, r.RegionName AS Label
                    FROM   Regions AS r WITH (NOLOCK)
                    ORDER BY Label"
                      );
        }

        public IEnumerable<(int, string)> GetCourseCentres()
        {
            return connection.Query<(int, string)>(
               $@"SELECT c.CentreID AS ID, c.CentreName AS Label
                    FROM   Centres AS c WITH (NOLOCK) INNER JOIN
                    CentreApplications AS ca WITH (NOLOCK) ON c.CentreID = ca.CentreID
                    WHERE c.Active = 1
                    GROUP BY c.CentreID, c.CentreName
                    ORDER BY Label"
                     );
        }

        public IEnumerable<(int, string)> GetCoreCourses()
        {
            return connection.Query<(int, string)>(
                $@"SELECT a.ApplicationID AS ID, a.ApplicationName AS Label
                    FROM   Applications AS a WITH (NOLOCK) INNER JOIN
                    Customisations AS cu WITH (NOLOCK) ON a.ApplicationID = cu.ApplicationID
                    WHERE (a.ASPMenu = 1) AND (a.ArchivedDate IS NULL) AND (CoreContent = 1 OR cu.AllCentres = 1) 
                    GROUP BY a.ApplicationID, a.ApplicationName
                    ORDER BY Label"
                      );
        }

        public string? GetApplicationNameById(int applicationId)
        {
            return (string?)connection.ExecuteScalar(
                @"SELECT        ApplicationName
                    FROM            Applications WITH (NOLOCK)
                    WHERE        ApplicationID = @applicationId",
               new { applicationId }
           );
        }

        public IEnumerable<(int, string)> GetCoreCourseCategories()
        {
            return connection.Query<(int, string)>(
                @"SELECT        cc.CourseCategoryID AS ID, cc.CategoryName AS Label
                    FROM            CourseCategories AS cc WITH (NOLOCK) INNER JOIN
                    Applications AS a WITH (NOLOCK) ON a.CourseCategoryID = cc.CourseCategoryID INNER JOIN
                    Customisations AS cu WITH (NOLOCK) ON a.ApplicationID = cu.ApplicationID
                    WHERE        (cc.Active = 1) AND (a.CoreContent = 1 OR cu.AllCentres = 1)
                    GROUP BY cc.CourseCategoryID, cc.CategoryName
                    ORDER BY cc.CategoryName"
           );
        }
        public IEnumerable<(int, string)> GetCoreCourseBrands()
        {
            return connection.Query<(int, string)>(
                @"SELECT        b.BrandID, b.BrandName
                    FROM            Brands AS b WITH (NOLOCK) INNER JOIN
                    Applications AS a WITH (NOLOCK) ON b.BrandID = a.BrandID INNER JOIN
                    Customisations AS cu WITH (NOLOCK) ON a.ApplicationID = cu.ApplicationID
                    WHERE        (b.Active = 1) AND (a.CoreContent = 1 OR cu.AllCentres = 1)
                    GROUP BY b.BrandID, b.BrandName
                    ORDER BY b.BrandName"
           );
        }
    }
}
