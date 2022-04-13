namespace DigitalLearningSolutions.Data.Services
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;

    public interface ICourseService
    {
        public IEnumerable<CourseStatistics> GetTopCourseStatistics(int centreId, int? categoryId);

        public IEnumerable<CourseStatisticsWithAdminFieldResponseCounts>
            GetCentreSpecificCourseStatisticsWithAdminFieldResponseCounts(
                int centreId,
                int? categoryId,
                bool includeAllCentreCourses = false
            );

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

        DelegateCourseDetails? GetDelegateCourseProgress(int progressId);

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

        public IEnumerable<ApplicationDetails> GetApplicationOptionsAlphabeticalListForCentre(
            int centreId,
            int? categoryId,
            int? topicId = null
        );

        public CentreCourseDetails GetCentreCourseDetails(int centreId, int? categoryId);

        public CentreCourseDetails GetCentreCourseDetailsWithAllCentreCourses(int centreId, int? categoryId);

        public bool DoesCourseNameExistAtCentre(
            string customisationName,
            int centreId,
            int applicationId,
            int customisationId = 0
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

        IEnumerable<string> GetCategoriesForCentreAndCentrallyManagedCourses(int centreId);

        IEnumerable<string> GetTopicsForCentreAndCentrallyManagedCourses(int centreId);

        IEnumerable<ApplicationWithSections> GetApplicationsByBrandId(int brandId);

        int CreateNewCentreCourse(Customisation customisation);

        LearningLog? GetLearningLogDetails(int progressId);
    }

    public class CourseService : ICourseService
    {
        private readonly IClockService clockService;
        private readonly ICourseAdminFieldsService courseAdminFieldsService;
        private readonly ICourseCategoriesDataService courseCategoriesDataService;
        private readonly ICourseDataService courseDataService;
        private readonly ICourseTopicsDataService courseTopicsDataService;
        private readonly IGroupsDataService groupsDataService;
        private readonly IProgressDataService progressDataService;
        private readonly ISectionService sectionService;

        public CourseService(
            IClockService clockService,
            ICourseDataService courseDataService,
            ICourseAdminFieldsService courseAdminFieldsService,
            IProgressDataService progressDataService,
            IGroupsDataService groupsDataService,
            ICourseCategoriesDataService courseCategoriesDataService,
            ICourseTopicsDataService courseTopicsDataService,
            ISectionService sectionService
        )
        {
            this.clockService = clockService;
            this.courseDataService = courseDataService;
            this.courseAdminFieldsService = courseAdminFieldsService;
            this.progressDataService = progressDataService;
            this.groupsDataService = groupsDataService;
            this.courseCategoriesDataService = courseCategoriesDataService;
            this.courseTopicsDataService = courseTopicsDataService;
            this.sectionService = sectionService;
        }

        public IEnumerable<CourseStatistics> GetTopCourseStatistics(int centreId, int? categoryId)
        {
            var allCourses = courseDataService.GetCourseStatisticsAtCentreFilteredByCategory(centreId, categoryId);
            return allCourses.Where(c => c.Active).OrderByDescending(c => c.InProgressCount);
        }

        public IEnumerable<CourseStatisticsWithAdminFieldResponseCounts>
            GetCentreSpecificCourseStatisticsWithAdminFieldResponseCounts(
                int centreId,
                int? categoryId,
                bool includeAllCentreCourses = false
            )
        {
            var allCourses = courseDataService.GetCourseStatisticsAtCentreFilteredByCategory(centreId, categoryId);
            return allCourses.Where(c => c.CentreId == centreId || c.AllCentres && includeAllCentreCourses).Select(
                c => new CourseStatisticsWithAdminFieldResponseCounts(
                    c,
                    courseAdminFieldsService.GetCourseAdminFieldsWithAnswerCountsForCourse(c.CustomisationId, centreId)
                )
            );
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
                .Select(GetDelegateAttemptsAndCourseAdminFields)
                .Where(info => info.DelegateCourseInfo.RemovedDate == null);
        }

        public DelegateCourseDetails? GetDelegateCourseProgress(int progressId)
        {
            var info = courseDataService.GetDelegateCourseInfoByProgressId(progressId);

            return info == null ? null : GetDelegateAttemptsAndCourseAdminFields(info);
        }

        public bool? VerifyAdminUserCanManageCourse(int customisationId, int centreId, int? categoryId)
        {
            var viewVerificationResult = VerifyAdminUserCanViewCourse(customisationId, centreId, categoryId);
            if (viewVerificationResult != true)
            {
                return viewVerificationResult;
            }

            var courseValidationDetails = courseDataService.GetCourseValidationDetails(customisationId, centreId);

            if (courseValidationDetails!.AllCentres && courseValidationDetails.CentreId != centreId)
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

            if (categoryId != null && courseValidationDetails.CourseCategoryId != categoryId)
            {
                return false;
            }

            var centreIdMatches = courseValidationDetails.CentreId == centreId;
            var allCentresCourseWithApplication =
                courseValidationDetails.AllCentres && courseValidationDetails.CentreHasApplication;

            return centreIdMatches || allCentresCourseWithApplication;
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
            string customisationName,
            int centreId,
            int applicationId,
            int customisationId = 0
        )
        {
            return courseDataService.DoesCourseNameExistAtCentre(
                customisationName,
                centreId,
                applicationId,
                customisationId
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

            var groupCourseIds = groupsDataService.GetGroupCoursesVisibleToCentre(centreId)
                .Where(gc => gc.IsUsable && gc.GroupId == groupId)
                .Select(gc => gc.CustomisationId);

            var coursesNotAlreadyInGroup = allPossibleCourses.Where(c => !groupCourseIds.Contains(c.CustomisationId));

            return coursesNotAlreadyInGroup;
        }

        public CourseNameInfo? GetCourseNameAndApplication(int customisationId)
        {
            return courseDataService.GetCourseNameAndApplication(customisationId);
        }

        public IEnumerable<string> GetCategoriesForCentreAndCentrallyManagedCourses(int centreId)
        {
            return courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId)
                .Select(c => c.CategoryName);
        }

        public IEnumerable<string> GetTopicsForCentreAndCentrallyManagedCourses(int centreId)
        {
            return courseTopicsDataService.GetCourseTopicsAvailableAtCentre(centreId).Select(c => c.CourseTopic);
        }

        public CentreCourseDetails GetCentreCourseDetails(int centreId, int? categoryId)
        {
            var (courses, categories, topics) = (
                GetCentreSpecificCourseStatisticsWithAdminFieldResponseCounts(centreId, categoryId),
                courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId)
                    .Select(c => c.CategoryName),
                courseTopicsDataService.GetCourseTopicsAvailableAtCentre(centreId).Select(c => c.CourseTopic));

            return new CentreCourseDetails(courses, categories, topics);
        }

        public CentreCourseDetails GetCentreCourseDetailsWithAllCentreCourses(int centreId, int? categoryId)
        {
            var (courses, categories, topics) = (
                GetCentreSpecificCourseStatisticsWithAdminFieldResponseCounts(centreId, categoryId, true),
                courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId)
                    .Select(c => c.CategoryName),
                courseTopicsDataService.GetCourseTopicsAvailableAtCentre(centreId).Select(c => c.CourseTopic));

            return new CentreCourseDetails(courses, categories, topics);
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

        public int CreateNewCentreCourse(
            Customisation customisation
        )
        {
            return courseDataService.CreateNewCentreCourse(customisation);
        }

        public IEnumerable<ApplicationDetails>
            GetApplicationOptionsAlphabeticalListForCentre(
                int centreId,
                int? categoryId,
                int? topicId = null
            )
        {
            var activeApplications = courseDataService.GetApplicationsAvailableToCentreByCategory(centreId, categoryId);
            var filteredApplications = activeApplications.Where(c => c.CourseTopicId == topicId || topicId == null);
            return filteredApplications.OrderBy(a => a.ApplicationName);
        }

        public IEnumerable<ApplicationWithSections> GetApplicationsByBrandId(int brandId)
        {
            var numRecordsByApplicationId =
                courseDataService.GetNumsOfRecentProgressRecordsForBrand(brandId, clockService.UtcNow.AddMonths(-3));
            var applications = courseDataService.GetApplicationsByBrandId(brandId);
            double maxPopularity = numRecordsByApplicationId.Any() ? numRecordsByApplicationId.First().Value : 0;
            var applicationsWithSections = applications.Select(
                application => new ApplicationWithSections(
                    application,
                    sectionService.GetSectionsThatHaveTutorialsAndPopulateTutorialsForApplication(application.ApplicationId),
                    maxPopularity == 0
                        ? 0
                        : (numRecordsByApplicationId.ContainsKey(application.ApplicationId)
                            ? numRecordsByApplicationId[application.ApplicationId] / maxPopularity
                            : 0)
                )
            );
            return applicationsWithSections;
        }

        public LearningLog? GetLearningLogDetails(int progressId)
        {
            var delegateCourseInfo = courseDataService.GetDelegateCourseInfoByProgressId(progressId);

            if (delegateCourseInfo == null)
            {
                return null;
            }

            var learningLogEntries = progressDataService.GetLearningLogEntries(progressId);

            return new LearningLog(delegateCourseInfo, learningLogEntries);
        }

        public DelegateCourseDetails GetDelegateAttemptsAndCourseAdminFields(
            DelegateCourseInfo info
        )
        {
            var coursePrompts = courseAdminFieldsService.GetCourseAdminFieldsWithAnswersForCourse(
                info,
                info.CustomisationId
            );

            var attemptStats = info.IsAssessed
                ? courseDataService.GetDelegateCourseAttemptStats(info.DelegateId, info.CustomisationId)
                : new AttemptStats(0, 0);

            return new DelegateCourseDetails(info, coursePrompts, attemptStats);
        }
    }
}
