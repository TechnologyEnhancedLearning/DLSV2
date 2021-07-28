namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;

    public interface ICourseService
    {
        public IEnumerable<(DelegateCourseInfo info, List<CustomPromptWithAnswer> prompts, (int, int) stats)>
            GetAllCoursesForDelegate(
                int delegateId,
                int centreId
            );

        public IEnumerable<CourseStatistics> GetTopCourseStatistics(int centreId, int categoryId);
        public IEnumerable<CourseStatistics> GetCentreSpecificCourseStatistics(int centreId, int categoryId);
    }

    public class CourseService : ICourseService
    {
        private readonly ICourseDataService courseDataService;
        private readonly ICustomPromptsService customPromptsService;

        public CourseService(ICourseDataService courseDataService, ICustomPromptsService customPromptsService)
        {
            this.courseDataService = courseDataService;
            this.customPromptsService = customPromptsService;
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

        public IEnumerable<(DelegateCourseInfo info, List<CustomPromptWithAnswer> prompts, (int, int) stats)>
            GetAllCoursesForDelegate(int delegateId, int centreId)
        {
            return courseDataService.GetDelegateCoursesInfo(delegateId).Select(
                info =>
                {
                    var courseCustomPromptsWithAnswers = customPromptsService.GetCustomPromptsWithAnswersForCourse(
                        info,
                        info.CustomisationId,
                        centreId
                    );
                    var attemptStats = info.IsAssessed
                        ? courseDataService.GetDelegateCourseAttemptStats(delegateId, info.CustomisationId)
                        : (0, 0);
                    return (info, courseCustomPromptsWithAnswers, attemptStats);
                }
            );
        }
    }
}
