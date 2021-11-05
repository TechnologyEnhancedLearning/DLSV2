namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.Courses;

    public interface ICourseService
    {
        public IEnumerable<CourseStatistics> GetTopCourseStatistics(int centreId, int? categoryId);

        public IEnumerable<CourseStatistics> GetCentreSpecificCourseStatistics(int centreId, int? categoryId);

        public bool DelegateHasCurrentProgress(int delegateId, int customisationId);

        public void RemoveDelegateFromCourse(
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

        bool? VerifyAdminUserCanManageCourse(int customisationId, int centreId, int? categoryId);

        bool? VerifyAdminUserCanViewCourse(int customisationId, int centreId, int? categoryId);

        public CourseDetails? GetCourseDetailsFilteredByCategory(int customisationId, int centreId, int? categoryId);

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

        CourseOptions? GetCourseOptionsFilteredByCategory(int customisationId, int centreId, int? categoryId);

        int? GetCourseCategoryId(int customisationId, int centreId);

        IEnumerable<CourseAssessmentDetails> GetEligibleCoursesToAddToGroup(int centreId, int? categoryId, int groupId);

        CourseNameInfo? GetCourseNameAndApplication(int customisationId);

        void CreateNewCentreCourse(CourseDetails courseDetails);
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
                .Where(info => info.CustomisationCentreId == centreId || info.AllCentresCourse)
                .Where(info => courseCategoryId == null || info.CourseCategoryId == courseCategoryId)
                .Select(GetDelegateAttemptsAndCourseCustomPrompts)
                .Where(info => info.DelegateCourseInfo.RemovedDate == null);
        }

        public DelegateCourseDetails? GetDelegateCourseProgress(int progressId, int centreId)
        {
            var info = courseDataService.GetDelegateCourseInfoByProgressId(progressId);

            return info == null ? null : GetDelegateAttemptsAndCourseCustomPrompts(info);
        }

        public bool? VerifyAdminUserCanManageCourse(int customisationId, int centreId, int? categoryId)
        {
            var courseValidationDetails = courseDataService.GetCourseValidationDetails(customisationId, centreId);

            if (courseValidationDetails == null)
            {
                return null;
            }

            if (courseValidationDetails.CentreId != centreId)
            {
                return false;
            }

            if (categoryId != null && courseValidationDetails.CourseCategoryId != categoryId)
            {
                return false;
            }

            return true;
        }

        public bool? VerifyAdminUserCanViewCourse(int customisationId, int centreId, int? categoryId)
        {
            var courseValidationDetails = courseDataService.GetCourseValidationDetails(customisationId, centreId);

            if (courseValidationDetails == null)
            {
                return null;
            }

            if (courseValidationDetails.CentreId != centreId && !courseValidationDetails.AllCentres)
            {
                return false;
            }

            if (categoryId != null && courseValidationDetails.CourseCategoryId != categoryId)
            {
                return false;
            }

            if (courseValidationDetails.AllCentres && !courseValidationDetails.CentreHasApplication)
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

        public CourseOptions? GetCourseOptionsFilteredByCategory(int customisationId, int centreId, int? categoryId)
        {
            return courseDataService.GetCourseOptionsFilteredByCategory(
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

        public IEnumerable<CourseAssessmentDetails> GetEligibleCoursesToAddToGroup(
            int centreId,
            int? categoryId,
            int groupId
        )
        {
            var allPossibleCourses = courseDataService.GetCoursesAvailableToCentreByCategory(centreId, categoryId)
                .Where(c => c.Active);

            var groupCourseIds = groupsDataService.GetGroupCoursesForCentre(centreId)
                .Where(gc => gc.IsUsable && gc.GroupId == groupId)
                .Select(gc => gc.CustomisationId);

            var coursesNotAlreadyInGroup = allPossibleCourses.Where(c => !groupCourseIds.Contains(c.CustomisationId));

            return coursesNotAlreadyInGroup;
        }

        public CourseNameInfo? GetCourseNameAndApplication(int customisationId)
        {
            return courseDataService.GetCourseNameAndApplication(customisationId);
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

        public void UpdateCourseOptions(CourseOptions courseOptions, int customisationId)
        {
            courseDataService.UpdateCourseOptions(
                courseOptions,
                customisationId
            );
        }

        public int? GetCourseCategoryId(int customisationId, int centreId)
        {
            return courseDataService.GetCourseValidationDetails(customisationId, centreId)?.CourseCategoryId;
        }

        public DelegateCourseDetails GetDelegateAttemptsAndCourseCustomPrompts(
            DelegateCourseInfo info
        )
        {
            var customPrompts = courseAdminFieldsService.GetCustomPromptsWithAnswersForCourse(
                info,
                info.CustomisationId
            );

            var attemptStats = info.IsAssessed
                ? courseDataService.GetDelegateCourseAttemptStats(info.DelegateId, info.CustomisationId)
                : new AttemptStats(0, 0);

            return new DelegateCourseDetails(info, customPrompts, attemptStats);
        }

        public void CreateNewCentreCourse(CourseDetails courseDetails)
        {
            courseDataService.CreateNewCentreCourse(
                courseDetails.CentreId,
                courseDetails.ApplicationId,
                courseDetails.CustomisationName,
                courseDetails.Password,
                courseDetails.SelfRegister,
                courseDetails.TutCompletionThreshold,
                courseDetails.IsAssessed,
                courseDetails.DiagCompletionThreshold,
                courseDetails.DiagObjSelect,
                courseDetails.HideInLearnerPortal,
                courseDetails.NotificationEmails);


        }
    }
}
