namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Common;
    using Microsoft.Extensions.Logging;

    public interface ICourseCategoriesDataService
    {
        IEnumerable<Category> GetCategoriesForCentreAndCentrallyManagedCourses(int centreId);
        string? GetCourseCategoryName(int categoryId);
    }

    public class CourseCategoriesDataService : ICourseCategoriesDataService
    {
        private readonly IDbConnection connection;
        private readonly ILogger<CourseCategoriesDataService> logger;

        public CourseCategoriesDataService(IDbConnection connection, ILogger<CourseCategoriesDataService> logger)
        {
            this.connection = connection;
            this.logger = logger;
        }

        public IEnumerable<Category> GetCategoriesForCentreAndCentrallyManagedCourses(int centreId)
        {
            return connection.Query<Category>(
                @"SELECT CourseCategoryID, CategoryName
                    FROM CourseCategories
                    WHERE ((CentreID = @CentreID) OR (CentreID = 0)) AND (Active = 1)
                    ORDER BY CategoryName",
                new { centreId }
            );
        }

        public string? GetCourseCategoryName(int categoryId)
        {
            var name = connection.QueryFirstOrDefault<string?>(
                @"SELECT CategoryName
                        FROM CourseCategories
                        WHERE CourseCategoryID = @categoryId",
                new { categoryId }
            );
            if (name == null)
            {
                logger.LogWarning(
                    $"No course category found for course category id {categoryId}"
                );
            }

            return name;
        }
    }
}
