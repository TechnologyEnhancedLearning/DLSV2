using DigitalLearningSolutions.Data.DataServices;
using DigitalLearningSolutions.Data.Models.Common;
using System.Collections.Generic;

namespace DigitalLearningSolutions.Web.Services
{
    public interface ICourseCategoriesService
    {
        IEnumerable<Category> GetCategoriesForCentreAndCentrallyManagedCourses(int centreId);
        string? GetCourseCategoryName(int categoryId);
    }
    public class CourseCategoriesService : ICourseCategoriesService
    {
        private readonly ICourseCategoriesDataService courseCategoriesDataService;
        public CourseCategoriesService(ICourseCategoriesDataService courseCategoriesDataService)
        {
            this.courseCategoriesDataService = courseCategoriesDataService;
        }

        public IEnumerable<Category> GetCategoriesForCentreAndCentrallyManagedCourses(int centreId)
        {
            return courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId);
        }

        public string? GetCourseCategoryName(int categoryId)
        {
            return courseCategoriesDataService.GetCourseCategoryName(categoryId);
        }
    }
}
