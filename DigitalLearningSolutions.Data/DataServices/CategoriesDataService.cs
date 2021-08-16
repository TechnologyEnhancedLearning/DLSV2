namespace DigitalLearningSolutions.Data.DataServices
{
    using System.Collections.Generic;
    using System.Data;
    using Dapper;
    using DigitalLearningSolutions.Data.Models.Common;

    public interface ICategoriesDataService
    {
        IEnumerable<Category> GetCategoryListForCentre(int centreId);
    }

    public class CategoriesDataService : ICategoriesDataService
    {
        private readonly IDbConnection connection;

        public CategoriesDataService(IDbConnection connection)
        {
            this.connection = connection;
        }

        public IEnumerable<Category> GetCategoryListForCentre(int centreId)
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
