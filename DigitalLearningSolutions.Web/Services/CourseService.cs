namespace DigitalLearningSolutions.Web.Services
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Utilities;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ConfigurationExtensions = DigitalLearningSolutions.Data.Extensions.ConfigurationExtensions;
    public interface ICourseService
    {
        public IEnumerable<CourseStatistics> GetTopCourseStatistics(int centreId, int? categoryId);

        public IEnumerable<CourseStatisticsWithAdminFieldResponseCounts>
            GetCentreSpecificCourseStatisticsWithAdminFieldResponseCounts(
                int centreId,
                int? categoryId
            );

        public IEnumerable<CourseStatisticsWithAdminFieldResponseCounts>
            GetCentreSpecificCourseStatisticsWithAdminFieldResponseCountsForReport(
            int centreId,
            int? categoryId,
            string? searchString,
            string? sortBy,
            string? filterString,
            string sortDirection
        );

        public bool DelegateHasCurrentProgress(int progressId);

        public void RemoveDelegateFromCourse(
            int delegateId,
            int customisationId,
            RemovalMethod removalMethod
        );

        IEnumerable<DelegateCourseInfo> GetAllCoursesInCategoryForDelegate(
            int delegateId,
            int centreId,
            int? courseCategoryId
        );

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

        public CentreCourseDetails GetCentreCourseDetailsWithAllCentreCourses(int centreId, int? categoryId, string? searchString,
            string? sortBy,
            string? filterString,
            string sortDirection);

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

        IEnumerable<ApplicationWithSections> GetApplicationsThatHaveSectionsByBrandId(int brandId);

        int CreateNewCentreCourse(Customisation customisation);

        LearningLog? GetLearningLogDetails(int progressId);

        public (IEnumerable<CourseStatisticsWithAdminFieldResponseCounts>, int) GetCentreCourses(string searchString, int offSet, int itemsPerPage, string sortBy, string sortDirection, int centreId, int? categoryId, bool allCentreCourses, bool? hideInLearnerPortal,
           string isActive, string categoryName, string courseTopic, string hasAdminFields);

        public IEnumerable<CourseStatisticsWithAdminFieldResponseCounts> GetDelegateCourses(string searchString, int centreId, int? categoryId, bool allCentreCourses, bool? hideInLearnerPortal, string isActive, string categoryName, string courseTopic, string hasAdminFields);
        public IEnumerable<DelegateAssessmentStatistics> GetDelegateAssessments(string searchString, int centreId, string categoryName, string isActive);
        IEnumerable<AvailableCourse> GetAvailableCourses(int delegateId, int? centreId, int categoryId);
        bool IsCourseCompleted(int candidateId, int customisationId);
        bool GetSelfRegister(int customisationId);
        IEnumerable<CurrentCourse> GetCurrentCourses(int candidateId);
        void SetCompleteByDate(int progressId, int candidateId, DateTime? completeByDate);
        void RemoveCurrentCourse(int progressId, int candidateId, RemovalMethod removalMethod);
        IEnumerable<CompletedCourse> GetCompletedCourses(int candidateId);
        IEnumerable<AvailableCourse> GetAvailableCourses(int candidateId, int? centreId);
        void EnrolOnSelfAssessment(int selfAssessmentId, int delegateUserId, int centreId);
        int GetNumberOfActiveCoursesAtCentreFilteredByCategory(int centreId, int? categoryId);
        public IEnumerable<Course> GetApplicationsAvailableToCentre(int centreId);
    }

    public class CourseService : ICourseService
    {
        private readonly IClockUtility clockUtility;
        private readonly ICourseAdminFieldsService courseAdminFieldsService;
        private readonly ICourseCategoriesDataService courseCategoriesDataService;
        private readonly ICourseDataService courseDataService;
        private readonly ICourseTopicsDataService courseTopicsDataService;
        private readonly IGroupsDataService groupsDataService;
        private readonly IProgressDataService progressDataService;
        private readonly ISectionService sectionService;
        private readonly IConfiguration configuration;

        public CourseService(
            IClockUtility clockUtility,
            ICourseDataService courseDataService,
            ICourseAdminFieldsService courseAdminFieldsService,
            IProgressDataService progressDataService,
            IGroupsDataService groupsDataService,
            ICourseCategoriesDataService courseCategoriesDataService,
            ICourseTopicsDataService courseTopicsDataService,
            ISectionService sectionService,
            IConfiguration configuration
        )
        {
            this.clockUtility = clockUtility;
            this.courseDataService = courseDataService;
            this.courseAdminFieldsService = courseAdminFieldsService;
            this.progressDataService = progressDataService;
            this.groupsDataService = groupsDataService;
            this.courseCategoriesDataService = courseCategoriesDataService;
            this.courseTopicsDataService = courseTopicsDataService;
            this.sectionService = sectionService;
            this.configuration = configuration;
        }

        public IEnumerable<CourseStatistics> GetTopCourseStatistics(int centreId, int? categoryId)
        {
            var allCourses = courseDataService.GetCourseStatisticsAtCentreFilteredByCategory(centreId, categoryId);
            return allCourses.Where(c => c.Active).OrderByDescending(c => c.InProgressCount);
        }

        public IEnumerable<CourseStatisticsWithAdminFieldResponseCounts>
            GetCentreSpecificCourseStatisticsWithAdminFieldResponseCounts(
                int centreId,
                int? categoryId
            )
        {
            var allCourses = courseDataService.GetCourseStatisticsAtCentreFilteredByCategory(centreId, categoryId);
            return allCourses.Where(c => c.CentreId == centreId).Select(
                c => new CourseStatisticsWithAdminFieldResponseCounts(
                    c, courseAdminFieldsService.GetCourseAdminFieldsWithAnswerCountsForCourse(c.CustomisationId, centreId)
                )
            );
        }

        public IEnumerable<CourseStatisticsWithAdminFieldResponseCounts>
        GetCentreSpecificCourseStatisticsWithAdminFieldResponseCountsForReport(
            int centreId,
            int? categoryId,
            string? searchString,
            string? sortBy,
            string? filterString,
            string sortDirection
        )
        {
            var exportQueryRowLimit = ConfigurationExtensions.GetExportQueryRowLimit(configuration);

            int resultCount = courseDataService.GetCourseStatisticsAtCentreFilteredByCategoryResultCount(centreId, categoryId, searchString);

            int totalRun = (int)(resultCount / exportQueryRowLimit) + ((resultCount % exportQueryRowLimit) > 0 ? 1 : 0);
            int currentRun = 1;

            List<CourseStatistics> allCourses = new List<CourseStatistics>();
            while (totalRun >= currentRun)
            {
                allCourses.AddRange(courseDataService.GetCourseStatisticsAtCentreFilteredByCategory(centreId, categoryId, exportQueryRowLimit, currentRun, searchString, sortBy, filterString, sortDirection));
                currentRun++;
            }
            return allCourses.Where(c => c.CentreId == centreId || c.AllCentres).Select(
                c => new CourseStatisticsWithAdminFieldResponseCounts(
                    c, courseAdminFieldsService.GetCourseAdminFieldsWithAnswerCountsForCourse(c.CustomisationId, centreId)
                )
            );
        }

        private IEnumerable<CourseStatisticsWithAdminFieldResponseCounts>
            GetNonArchivedCentreSpecificCourseStatisticsWithAdminFieldResponseCounts(
                int centreId,
                int? categoryId,
                bool includeAllCentreCourses = false
            )
        {
            var allCourses = courseDataService.GetNonArchivedCourseStatisticsAtCentreFilteredByCategory(centreId, categoryId);
            return allCourses.Where(c => c.CentreId == centreId || c.AllCentres && includeAllCentreCourses).Select(
                c => new CourseStatisticsWithAdminFieldResponseCounts(
                    c,
                    courseAdminFieldsService.GetCourseAdminFieldsWithAnswerCountsForCourse(c.CustomisationId, centreId)
                )
            );
        }

        public IEnumerable<DelegateCourseInfo> GetAllCoursesInCategoryForDelegate(
            int delegateId,
            int centreId,
            int? courseCategoryId
        )
        {
            return courseDataService.GetDelegateCoursesInfo(delegateId)
                .Where(info => info.CustomisationCentreId == centreId || info.AllCentresCourse)
                .Where(info => courseCategoryId == null || info.CourseCategoryId == courseCategoryId)
                .Select(PopulateDelegateCourseInfoAdminFields)
                .Where(info => info.RemovedDate == null);
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
            var activeCourses = courseDataService.GetNonArchivedCoursesAvailableToCentreByCategory(centreId, categoryId)
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

        public bool DelegateHasCurrentProgress(int progressId)
        {
            var progress = progressDataService.GetProgressByProgressId(progressId);
            return progress is { RemovedDate: null };
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

        public (IEnumerable<CourseStatisticsWithAdminFieldResponseCounts>, int) GetCentreCourses(string searchString, int offSet, int itemsPerPage, string sortBy, string sortDirection, int centreId, int? categoryId, bool allCentreCourses, bool? hideInLearnerPortal,
           string isActive, string categoryName, string courseTopic, string hasAdminFields)
        {
            var (allCourses, resultCount) = courseDataService.GetCourseStatisticsAtCentre(searchString, offSet, itemsPerPage, sortBy, sortDirection, centreId, categoryId, allCentreCourses, hideInLearnerPortal,
           isActive, categoryName, courseTopic, hasAdminFields);

            return (allCourses.Select(
                c => new CourseStatisticsWithAdminFieldResponseCounts(
                    c,
                    courseAdminFieldsService.GetCourseAdminFieldsWithAnswerCountsForCourse(c.CustomisationId, centreId)
                )
            ), resultCount);
        }

        public CentreCourseDetails GetCentreCourseDetailsWithAllCentreCourses(int centreId, int? categoryId, string? searchString,
            string? sortBy,
            string? filterString,
            string sortDirection)
        {
            var (courses, categories, topics) = (GetCentreSpecificCourseStatisticsWithAdminFieldResponseCountsForReport(centreId, categoryId, searchString, sortBy, filterString, sortDirection), courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId)
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
                .Where(p => p.RemovedDate == null)
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

        public IEnumerable<ApplicationWithSections> GetApplicationsThatHaveSectionsByBrandId(int brandId)
        {
            var numRecordsByApplicationId =
                courseDataService.GetNumsOfRecentProgressRecordsForBrand(brandId, clockUtility.UtcNow.AddMonths(-3));

            var applications = courseDataService.GetApplicationsByBrandId(brandId);

            double maxPopularity = numRecordsByApplicationId.Any() ? numRecordsByApplicationId.Values.Max() : 0;

            var applicationsWithSections = applications.Select(
                application => new ApplicationWithSections(
                    application,
                    sectionService.GetSectionsThatHaveTutorialsAndPopulateTutorialsForApplication(
                        application.ApplicationId
                    ),
                    maxPopularity == 0
                        ? 0
                        : numRecordsByApplicationId.GetValueOrDefault(application.ApplicationId, 0) / maxPopularity
                )
            );

            var applicationsWithPopulatedSections = applicationsWithSections.Where(a => a.Sections.Any());

            return applicationsWithPopulatedSections;
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

        private DelegateCourseInfo PopulateDelegateCourseInfoAdminFields(
            DelegateCourseInfo info
        )
        {
            var coursePrompts = courseAdminFieldsService.GetCourseAdminFieldsWithAnswersForCourse(
                info
            );
            info.CourseAdminFields = coursePrompts;
            return info;
        }

        public IEnumerable<CourseStatisticsWithAdminFieldResponseCounts> GetDelegateCourses(string searchString, int centreId, int? categoryId, bool allCentreCourses, bool? hideInLearnerPortal, string isActive, string categoryName, string courseTopic, string hasAdminFields)
        {
            var allCourses = courseDataService.GetDelegateCourseStatisticsAtCentre(searchString, centreId, categoryId, allCentreCourses, hideInLearnerPortal,
           isActive, categoryName, courseTopic, hasAdminFields);

            return allCourses.Select(
                c => new CourseStatisticsWithAdminFieldResponseCounts(
                    c, courseAdminFieldsService.GetCourseAdminFieldsWithAnswerCountsForCourse(c.CustomisationId, centreId)
                )
            );
        }

        public IEnumerable<DelegateAssessmentStatistics> GetDelegateAssessments(string searchString, int centreId, string categoryName, string isActive)
        {
            return courseDataService.GetDelegateAssessmentStatisticsAtCentre(searchString, centreId, categoryName, isActive);
        }

        public IEnumerable<AvailableCourse> GetAvailableCourses(int delegateId, int? centreId, int categoryId)
        {
            return courseDataService.GetAvailableCourses(delegateId, centreId, categoryId);
        }

        public bool IsCourseCompleted(int candidateId, int customisationId)
        {
            return courseDataService.IsCourseCompleted(candidateId, customisationId);
        }

        public bool GetSelfRegister(int customisationId)
        {
            return courseDataService.GetSelfRegister(customisationId);
        }

        public IEnumerable<CurrentCourse> GetCurrentCourses(int candidateId)
        {
            return courseDataService.GetCurrentCourses(candidateId);
        }

        public void SetCompleteByDate(int progressId, int candidateId, DateTime? completeByDate)
        {
            courseDataService.SetCompleteByDate(progressId, candidateId, completeByDate);
        }

        public void RemoveCurrentCourse(int progressId, int candidateId, RemovalMethod removalMethod)
        {
            courseDataService.RemoveCurrentCourse(progressId, candidateId, removalMethod);
        }

        public IEnumerable<CompletedCourse> GetCompletedCourses(int candidateId)
        {
            return courseDataService.GetCompletedCourses(candidateId);
        }

        public IEnumerable<AvailableCourse> GetAvailableCourses(int candidateId, int? centreId)
        {
            return courseDataService.GetAvailableCourses(candidateId, centreId);
        }

        public void EnrolOnSelfAssessment(int selfAssessmentId, int delegateUserId, int centreId)
        {
            courseDataService.EnrolOnSelfAssessment(selfAssessmentId, delegateUserId, centreId);
        }

        public int GetNumberOfActiveCoursesAtCentreFilteredByCategory(int centreId, int? categoryId)
        {
            return courseDataService.GetNumberOfActiveCoursesAtCentreFilteredByCategory(centreId, categoryId);
        }

        public IEnumerable<Course> GetApplicationsAvailableToCentre(int centreId)
        {
            return courseDataService.GetApplicationsAvailableToCentre(centreId);
        }
    }
}
