﻿namespace DigitalLearningSolutions.Data.Services
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
        public DelegateCourseDetails? GetDelegateCourseProgress(int progressId, int centreId);
        public bool? VerifyAdminUserCanAccessCourse(int customisationId, int centreId, int categoryId);
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
