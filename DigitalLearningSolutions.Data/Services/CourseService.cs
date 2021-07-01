namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Courses;

    public interface ICourseService
    {
        IEnumerable<CourseStatistics> GetTopCourseStatisticsAtCentreForCategoryId(int centreId, int categoryId);
    }

    public class CourseService : ICourseService
    {
        private readonly ICourseDataService courseDataService;

        public CourseService(ICourseDataService courseDataService)
        {
            this.courseDataService = courseDataService;
        }

        public IEnumerable<CourseStatistics> GetTopCourseStatisticsAtCentreForCategoryId(int centreId, int categoryId)
        {
            var allCourses = courseDataService.GetCourseStatisticsAtCentreForCategoryId(centreId, categoryId);
            return allCourses.Where(c => c.Active).OrderByDescending(c => c.DelegateCount);
        }
    }
}
