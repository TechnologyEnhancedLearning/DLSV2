namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Courses;

    public interface ICourseService
    {
        public IEnumerable<CourseStatistics> GetTopCourseStatistics(int centreId, int categoryId);
        public IEnumerable<CourseStatistics> GetCentreSpecificCourseStatistics(int centreId, int categoryId);
    }

    public class CourseService : ICourseService
    {
        private readonly ICourseDataService courseDataService;

        public CourseService(ICourseDataService courseDataService)
        {
            this.courseDataService = courseDataService;
        }

        public IEnumerable<CourseStatistics> GetTopCourseStatistics(int centreId, int categoryId)
        {
            var allCourses = courseDataService.GetCourseStatisticsAtCentreForCategoryId(centreId, categoryId);
            return allCourses.Where(c => c.Active).OrderByDescending(c => c.InProgressCount);
        }

        public IEnumerable<CourseStatistics> GetCentreSpecificCourseStatistics(int centreId, int categoryId)
        {
            var allCourses = courseDataService.GetCourseStatisticsAtCentreForCategoryId(centreId, categoryId);
            return allCourses.Where(c => c.CentreId == centreId);
        }
    }
}
