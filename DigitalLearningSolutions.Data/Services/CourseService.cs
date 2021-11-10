namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Courses;

    public interface ICourseService
    {
        public IEnumerable<CourseStatistics> GetTopCourseStatistics(int centreId, int? categoryId);

        public IEnumerable<CourseStatistics> GetCentreSpecificCourseStatistics(int centreId, int? categoryId);

        public IEnumerable<DelegateCourseDetails> GetAllCoursesInCategoryForDelegate(
            int delegateId,
            int centreId,
            int? courseCategoryId
        );

        public DelegateCourseDetails? GetDelegateCourseProgress(int progressId, int centreId);

        public bool? VerifyAdminUserCanAccessCourse(int customisationId, int centreId, int categoryId);

        public CourseDetails? GetCourseDetailsFilteredByCategory(int customisationId, int centreId, int? categoryId);

        public void UpdateLearningPathwayDefaultsForCourse(
            int customisationId,
            int completeWithinMonths,
            int validityMonths,
            bool mandatory,
            bool autoRefresh
        );

        void UpdateCourseOptions(CourseOptions courseOptions, int customisationId);

        CourseOptions? GetCourseOptionsFilteredByCategory(int customisationId, int centreId, int? categoryId);
    }

    public class CourseService : ICourseService
    {
        private readonly ICourseAdminFieldsService courseAdminFieldsService;
        private readonly ICourseDataService courseDataService;

        public CourseService(ICourseDataService courseDataService, ICourseAdminFieldsService courseAdminFieldsService)
        {
            this.courseDataService = courseDataService;
            this.courseAdminFieldsService = courseAdminFieldsService;
        }

        public IEnumerable<CourseStatistics> GetTopCourseStatistics(int centreId, int? categoryId)
        {
            var allCourses = courseDataService.GetCourseStatisticsAtCentreFilteredByCategory(centreId, categoryId);
            return allCourses.Where(c => c.Active).OrderByDescending(c => c.InProgressCount);
        }

        public IEnumerable<CourseStatistics> GetCentreSpecificCourseStatistics(int centreId, int? categoryId)
        {
            var allCourses = courseDataService.GetCourseStatisticsAtCentreFilteredByCategory(centreId, categoryId);
            return allCourses.Where(c => c.CentreId == centreId);
        }

        public IEnumerable<DelegateCourseDetails> GetAllCoursesInCategoryForDelegate(
            int delegateId,
            int centreId,
            int? courseCategoryId
        )
        {
            return courseDataService.GetDelegateCoursesInfo(delegateId)
                .Where(info => courseCategoryId == null || info.CourseCategoryId == courseCategoryId)
                .Select(
                    info => GetDelegateAttemptsAndCourseCustomPrompts(info, centreId)
                ).Where(info => info.DelegateCourseInfo.RemovedDate == null);
        }

        public DelegateCourseDetails? GetDelegateCourseProgress(int progressId, int centreId)
        {
            var info = courseDataService.GetDelegateCourseInfoByProgressId(progressId);

            return info == null ? null : GetDelegateAttemptsAndCourseCustomPrompts(info, centreId, true);
        }

        public bool? VerifyAdminUserCanAccessCourse(int customisationId, int centreId, int adminCategoryIdClaim)
        {
            var (courseCentreId, courseCategoryId) = courseDataService.GetCourseValidationDetails(customisationId);

            if (courseCentreId == null || courseCategoryId == null)
            {
                return null;
            }

            if (courseCentreId != centreId)
            {
                return false;
            }

            if (adminCategoryIdClaim != 0 && courseCategoryId != adminCategoryIdClaim)
            {
                return false;
            }

            return true;
        }

        public CourseDetails? GetCourseDetailsFilteredByCategory(int customisationId, int centreId, int? categoryId)
        {
            return courseDataService.GetCourseDetailsFilteredByCategory(customisationId, centreId, categoryId);
        }

        public void UpdateLearningPathwayDefaultsForCourse(
            int customisationId,
            int completeWithinMonths,
            int validityMonths,
            bool mandatory,
            bool autoRefresh
        )
        {
            courseDataService.UpdateLearningPathwayDefaultsForCourse(
                customisationId,
                completeWithinMonths,
                validityMonths,
                mandatory,
                autoRefresh
            );
        }

        public void UpdateCourseOptions(CourseOptions courseOptions, int customisationId)
        {
            courseDataService.UpdateCourseOptions(
                courseOptions,
                customisationId
            );
        }

        public CourseOptions? GetCourseOptionsFilteredByCategory(int customisationId, int centreId, int? categoryId)
        {
            return courseDataService.GetCourseOptionsFilteredByCategory(
                customisationId,
                centreId,
                categoryId);
        }

        public DelegateCourseDetails GetDelegateAttemptsAndCourseCustomPrompts(
            DelegateCourseInfo info,
            int centreId,
            bool allowAllCentreCourses = false
        )
        {
            var customPrompts = courseAdminFieldsService.GetCustomPromptsWithAnswersForCourse(
                info,
                info.CustomisationId,
                centreId,
                allowAllCentreCourses
            );

            var attemptStats = info.IsAssessed
                ? courseDataService.GetDelegateCourseAttemptStats(info.DelegateId, info.CustomisationId)
                : new AttemptStats(0, 0);

            return new DelegateCourseDetails(info, customPrompts, attemptStats);
        }
    }
}
