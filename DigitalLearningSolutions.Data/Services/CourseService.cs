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
        public IEnumerable<DelegateCourseDetails> GetAllCoursesForDelegate(int delegateId, int centreId);
    }

    public class CourseService : ICourseService
    {
        private readonly ICourseDataService courseDataService;
        private readonly ICourseAdminFieldsService courseAdminFieldsService;

        public CourseService(ICourseDataService courseDataService, ICourseAdminFieldsService courseAdminFieldsService)
        {
            this.courseDataService = courseDataService;
            this.courseAdminFieldsService = courseAdminFieldsService;
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

        public IEnumerable<DelegateCourseDetails> GetAllCoursesForDelegate(int delegateId, int centreId)
        {
            return courseDataService.GetDelegateCoursesInfo(delegateId).Select(
                info =>
                {
                    var customPrompts = courseAdminFieldsService.GetCustomPromptsWithAnswersForCourse(
                        info,
                        info.CustomisationId,
                        centreId
                    );
                    var attemptStats = info.IsAssessed
                        ? courseDataService.GetDelegateCourseAttemptStats(delegateId, info.CustomisationId)
                        : (0, 0);
                    return new DelegateCourseDetails(info, customPrompts, attemptStats);
                }
            );
        }
    }
}
