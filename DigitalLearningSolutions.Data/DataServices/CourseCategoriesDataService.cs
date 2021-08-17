namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Common;

    public interface ICourseCategoriesDataService
    {
        IEnumerable<Category> GetCategoriesForCentreAndCentrallyManagedCourses(int centreId);
    }

    public class CourseCategoriesDataService : ICourseCategoriesDataService
    {
        private readonly IDbConnection connection;

        public CourseCategoriesDataService(IDbConnection connection)
        {
            this.connection = connection;
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
    }
}
