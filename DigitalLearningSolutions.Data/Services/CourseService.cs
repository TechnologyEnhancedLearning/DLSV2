﻿namespace DigitalLearningSolutions.Data.Services
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

        public bool DelegateHasCurrentProgress(int delegateId, int customisationId);

        public void RemoveDelegateFromCourse(
            int delegateId,
            int customisationId,
            RemovalMethod removalMethod
        );

        public IEnumerable<DelegateCourseDetails> GetAllCoursesInCategoryForDelegate(
            int delegateId,
            int centreId,
            int? courseCategoryId
        );

        public DelegateCourseDetails? GetDelegateCourseProgress(int progressId, int centreId);

        public bool? VerifyAdminUserCanAccessCourse(int customisationId, int centreId, int categoryId);

        public CourseDetails? GetCourseDetailsForAdminCategoryId(int customisationId, int centreId, int categoryId);

        public void UpdateLearningPathwayDefaultsForCourse(
            int customisationId,
            int completeWithinMonths,
            int validityMonths,
            bool mandatory,
            bool autoRefresh
        );

        void UpdateCourseOptions(CourseOptions courseOptions, int customisationId);

        CourseOptions? GetCourseOptionsForAdminCategoryId(int customisationId, int centreId, int categoryId);
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

        public CourseDetails? GetCourseDetailsForAdminCategoryId(int customisationId, int centreId, int categoryId)
        {
            return courseDataService.GetCourseDetailsForAdminCategoryId(customisationId, centreId, categoryId);
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

        public CourseOptions? GetCourseOptionsForAdminCategoryId(int customisationId, int centreId, int categoryId)
        {
            return courseDataService.GetCourseOptionsForAdminCategoryId(
                customisationId,
                centreId,
                categoryId
            );
        }

        public bool DelegateHasCurrentProgress(int delegateId, int customisationId)
        {
            return progressDataService
                .GetDelegateProgressForCourse(delegateId, customisationId)
                .Any(p => p.Completed == null && p.RemovedDate == null);
        }

        public void RemoveDelegateFromCourse(
            int delegateId,
            int customisationId,
            RemovalMethod removalMethod
        )
        {
            var currentProgressIds = progressDataService.GetDelegateProgressForCourse(delegateId, customisationId)
                .Where(p => p.Completed == null && p.RemovedDate == null)
                .Select(p => p.ProgressId)
                .ToList();

            foreach (var progressId in currentProgressIds)
            {
                courseDataService.RemoveCurrentCourse(progressId, delegateId, removalMethod);
            }
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
