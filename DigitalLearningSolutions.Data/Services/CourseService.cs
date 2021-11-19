﻿namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.Courses;

    public interface ICourseService
    {
        IEnumerable<CourseStatistics> GetTopCourseStatistics(int centreId, int categoryId);

        IEnumerable<CourseStatistics> GetCentreSpecificCourseStatistics(int centreId, int categoryId);

        bool RemoveDelegateFromCourseIfDelegateHasCurrentProgress(
            int delegateId,
            int customisationId,
            RemovalMethod removalMethod
        );

        IEnumerable<DelegateCourseDetails> GetAllCoursesInCategoryForDelegate(
            int delegateId,
            int centreId,
            int? courseCategoryId
        );

        DelegateCourseDetails? GetDelegateCourseProgress(int progressId, int centreId);

        bool? VerifyAdminUserCanManageCourse(int customisationId, int centreId, int categoryId);

        bool? VerifyAdminUserCanViewCourse(int customisationId, int centreId, int adminCategoryIdClaim);

        CourseDetails? GetCourseDetailsForAdminCategoryId(int customisationId, int centreId, int categoryId);

        void UpdateLearningPathwayDefaultsForCourse(
            int customisationId,
            int completeWithinMonths,
            int validityMonths,
            bool mandatory,
            bool autoRefresh,
            int refreshToCustomisationId = 0,
            int autoRefreshMonths = 0,
            bool applyLpDefaultsToSelfEnrol = false
        );

        public IEnumerable<(int id, string name)> GetCourseOptionsAlphabeticalListForCentre(
            int centreId,
            int? categoryId
        );

        public bool DoesCourseNameExistAtCentre(
            int customisationId,
            string customisationName,
            int centreId,
            int applicationId
        );

        public void UpdateCourseDetails(
            int customisationId,
            string customisationName,
            string password,
            string notificationEmails,
            bool isAssessed,
            int tutCompletionThreshold,
            int diagCompletionThreshold
        );

        void UpdateCourseOptions(CourseOptions courseOptions, int customisationId);

        CourseOptions? GetCourseOptionsForAdminCategoryId(int customisationId, int centreId, int categoryId);

        IEnumerable<CourseAssessmentDetails> GetEligibleCoursesToAddToGroup(int centreId, int? categoryId, int groupId);
    }

    public class CourseService : ICourseService
    {
        private readonly ICourseAdminFieldsService courseAdminFieldsService;
        private readonly ICourseDataService courseDataService;
        private readonly IGroupsDataService groupsDataService;
        private readonly IProgressDataService progressDataService;

        public CourseService(
            ICourseDataService courseDataService,
            ICourseAdminFieldsService courseAdminFieldsService,
            IProgressDataService progressDataService,
            IGroupsDataService groupsDataService
        )
        {
            this.courseDataService = courseDataService;
            this.courseAdminFieldsService = courseAdminFieldsService;
            this.progressDataService = progressDataService;
            this.groupsDataService = groupsDataService;
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

        public bool? VerifyAdminUserCanManageCourse(int customisationId, int centreId, int adminCategoryIdClaim)
        {
            var (courseCentreId, courseCategoryId, _) = courseDataService.GetCourseValidationDetails(customisationId);

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

        public bool? VerifyAdminUserCanViewCourse(int customisationId, int centreId, int adminCategoryIdClaim)
        {
            var (courseCentreId, courseCategoryId, allCentres) = courseDataService.GetCourseValidationDetails(customisationId);

            if (courseCentreId == null || courseCategoryId == null || allCentres == null)
            {
                return null;
            }

            if (courseCentreId != centreId && !allCentres.Value)
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
            bool autoRefresh,
            int refreshToCustomisationId = 0,
            int autoRefreshMonths = 0,
            bool applyLpDefaultsToSelfEnrol = false
        )
        {
            courseDataService.UpdateLearningPathwayDefaultsForCourse(
                customisationId,
                completeWithinMonths,
                validityMonths,
                mandatory,
                autoRefresh,
                refreshToCustomisationId,
                autoRefreshMonths,
                applyLpDefaultsToSelfEnrol
            );
        }

        public IEnumerable<(int id, string name)> GetCourseOptionsAlphabeticalListForCentre(
            int centreId,
            int? categoryId
        )
        {
            var activeCourses = courseDataService.GetCoursesAvailableToCentreByCategory(centreId, categoryId)
                .Where(c => c.Active = true);
            var orderedCourses = activeCourses.OrderBy(c => c.ApplicationName);
            return orderedCourses.Select(c => (c.CustomisationId, c.CourseName));
        }

        public bool DoesCourseNameExistAtCentre(
            int customisationId,
            string customisationName,
            int centreId,
            int applicationId
        )
        {
            return courseDataService.DoesCourseNameExistAtCentre(
                customisationId,
                customisationName,
                centreId,
                applicationId
            );
        }

        public void UpdateCourseDetails(
            int customisationId,
            string customisationName,
            string? password,
            string? notificationEmails,
            bool isAssessed,
            int tutCompletionThreshold,
            int diagCompletionThreshold
        )
        {
            courseDataService.UpdateCourseDetails(
                customisationId,
                customisationName,
                password!,
                notificationEmails!,
                isAssessed,
                tutCompletionThreshold,
                diagCompletionThreshold
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

        public IEnumerable<CourseAssessmentDetails> GetEligibleCoursesToAddToGroup(
            int centreId,
            int? categoryId,
            int groupId
        )
        {
            var allPossibleCourses = courseDataService.GetCoursesAvailableToCentreByCategory(centreId, categoryId)
                .Where(c => c.Active && c.AspMenu);

            var groupCourseIds = groupsDataService.GetGroupCourses(centreId, groupId).Select(gc => gc.CustomisationId);

            var coursesNotAlreadyInGroup = allPossibleCourses.Where(c => !groupCourseIds.Contains(c.CustomisationId));

            return coursesNotAlreadyInGroup;
        }

        public bool RemoveDelegateFromCourseIfDelegateHasCurrentProgress(
            int delegateId,
            int customisationId,
            RemovalMethod removalMethod
        )
        {
            var currentProgressIds = progressDataService.GetDelegateProgressForCourse(delegateId, customisationId)
                .Where(p => p.Completed == null && p.RemovedDate == null)
                .Select(p => p.ProgressId)
                .ToList();

            if (!currentProgressIds.Any())
            {
                return false;
            }

            foreach (var progressId in currentProgressIds)
            {
                courseDataService.RemoveCurrentCourse(progressId, delegateId, removalMethod);
            }

            return true;
        }

        public void UpdateCourseOptions(CourseOptions courseOptions, int customisationId)
        {
            courseDataService.UpdateCourseOptions(
                courseOptions,
                customisationId
            );
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
