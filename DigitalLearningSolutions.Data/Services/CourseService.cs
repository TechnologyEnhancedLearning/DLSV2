namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.Courses;

    public interface ICourseService
    {
        public IEnumerable<CourseStatistics> GetTopCourseStatistics(int centreId, int categoryId);
        public IEnumerable<CourseStatistics> GetCentreSpecificCourseStatistics(int centreId, int categoryId);
        public IEnumerable<DelegateCourseDetails> GetAllCoursesForDelegate(int delegateId, int centreId);
        public bool RemoveDelegateFromCourse(int delegateId, int customisationId, RemovalMethod removalMethod);
    }

    public class CourseService : ICourseService
    {
        private readonly ICourseAdminFieldsService courseAdminFieldsService;
        private readonly ICourseDataService courseDataService;
        private readonly IProgressDataService progressDataService;

        public CourseService(
            ICourseDataService courseDataService,
            ICourseAdminFieldsService courseAdminFieldsService,
            IProgressDataService progressDataService
        )
        {
            this.courseDataService = courseDataService;
            this.courseAdminFieldsService = courseAdminFieldsService;
            this.progressDataService = progressDataService;
        }

        public IEnumerable<CourseStatistics> GetTopCourseStatistics(int centreId, int categoryId)
        {
            var allCourses = courseDataService.GetCourseStatisticsAtCentreForAdminCategoryId(centreId, categoryId);
            return allCourses.Where(c => c.Active).OrderByDescending(c => c.InProgressCount);
        }

        public IEnumerable<CourseStatistics> GetCentreSpecificCourseStatistics(int centreId, int categoryId)
        {
            var allCourses = courseDataService.GetCourseStatisticsAtCentreForAdminCategoryId(centreId, categoryId);
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

        public bool RemoveDelegateFromCourse(int delegateId, int customisationId, RemovalMethod removalMethod)
        {
            var currentProgress = progressDataService.GetDelegateProgressForCourse(delegateId, customisationId)
                .FirstOrDefault(p => p.Completed == null && p.RemovedDate == null);
            if (currentProgress == null)
            {
                return false;
            }

            courseDataService.RemoveCurrentCourse(currentProgress.ProgressId, delegateId, removalMethod);
            return true;
        }
    }
}
