namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("TrackingSystem/Delegates/{accessedVia}/DelegateProgress/{progressId:int}")]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [ServiceFilter(typeof(VerifyDelegateAccessedViaValidRoute))]
    [ServiceFilter(typeof(VerifyAdminUserCanAccessProgress))]
    public class DelegateProgressController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly ICourseAdminFieldsService courseAdminFieldsService;
        private readonly ICourseService courseService;
        private readonly IProgressService progressService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly IUserService userService;

        public DelegateProgressController(
            ICourseService courseService,
            ICourseAdminFieldsService courseAdminFieldsService,
            IUserService userService,
            IProgressService progressService,
            IConfiguration configuration,
            ISearchSortFilterPaginateService searchSortFilterPaginateService
        )
        {
            this.courseService = courseService;
            this.courseAdminFieldsService = courseAdminFieldsService;
            this.userService = userService;
            this.progressService = progressService;
            this.configuration = configuration;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
        }

        public IActionResult Index(int progressId, DelegateAccessRoute accessedVia, int? returnPage)
        {
            var courseDelegatesData =
                courseService.GetDelegateCourseProgress(progressId);

            var model = new DelegateProgressViewModel(
                accessedVia,
                courseDelegatesData!,
                returnPage
            );
            return View(model);
        }

        [HttpGet]
        [Route("EditSupervisor")]
        public IActionResult EditSupervisor(int progressId, DelegateAccessRoute accessedVia, int? returnPage)
        {
            var centreId = User.GetCentreId();
            var delegateCourseProgress =
                courseService.GetDelegateCourseProgress(progressId);
            var supervisors = userService.GetSupervisorsAtCentreForCategory(
                centreId,
                delegateCourseProgress!.DelegateCourseInfo.CourseCategoryId
            );

            var model = new EditSupervisorViewModel(
                progressId,
                accessedVia,
                supervisors,
                delegateCourseProgress!.DelegateCourseInfo,
                returnPage
            );
            return View(model);
        }

        [HttpPost]
        [Route("EditSupervisor")]
        public IActionResult EditSupervisor(
            EditSupervisorFormData formData,
            int progressId,
            DelegateAccessRoute accessedVia
        )
        {
            if (!ModelState.IsValid)
            {
                var supervisors = userService.GetSupervisorsAtCentre(User.GetCentreId());
                var model = new EditSupervisorViewModel(formData, progressId, accessedVia, supervisors);
                return View(model);
            }

            progressService.UpdateSupervisor(progressId, formData.SupervisorId);

            return RedirectToPreviousPage(formData.DelegateId, progressId, accessedVia, formData.ReturnPage);
        }

        [HttpGet]
        [Route("EditCompleteByDate")]
        public IActionResult EditCompleteByDate(
            int progressId,
            DelegateAccessRoute accessedVia,
            int? returnPage
        )
        {
            var delegateCourseProgress =
                courseService.GetDelegateCourseProgress(progressId);

            var model = new EditCompleteByDateViewModel(
                progressId,
                accessedVia,
                delegateCourseProgress!.DelegateCourseInfo,
                returnPage
            );
            return View(model);
        }

        [HttpPost]
        [Route("EditCompleteByDate")]
        public IActionResult EditCompleteByDate(
            EditCompleteByDateFormData formData,
            int progressId,
            DelegateAccessRoute accessedVia
        )
        {
            if (!ModelState.IsValid)
            {
                var model = new EditCompleteByDateViewModel(formData, progressId, accessedVia);
                return View(model);
            }

            var completeByDate = formData.Year != null
                ? new DateTime(formData.Year.Value, formData.Month!.Value, formData.Day!.Value)
                : (DateTime?)null;

            progressService.UpdateCompleteByDate(progressId, completeByDate);

            return RedirectToPreviousPage(formData.DelegateId, progressId, accessedVia, formData.ReturnPage);
        }

        [HttpGet]
        [Route("EditCompletionDate")]
        public IActionResult EditCompletionDate(
            int progressId,
            DelegateAccessRoute accessedVia,
            int? returnPage
        )
        {
            var delegateCourseProgress =
                courseService.GetDelegateCourseProgress(progressId);

            var model = new EditCompletionDateViewModel(
                progressId,
                accessedVia,
                delegateCourseProgress!.DelegateCourseInfo,
                returnPage
            );
            return View(model);
        }

        [HttpPost]
        [Route("EditCompletionDate")]
        public IActionResult EditCompletionDate(
            EditCompletionDateFormData formData,
            int progressId,
            DelegateAccessRoute accessedVia
        )
        {
            if (!ModelState.IsValid)
            {
                var model = new EditCompletionDateViewModel(formData, progressId, accessedVia);
                return View(model);
            }

            var completionDate = formData.Year != null
                ? new DateTime(formData.Year.Value, formData.Month!.Value, formData.Day!.Value)
                : (DateTime?)null;

            progressService.UpdateCompletionDate(progressId, completionDate);
            return RedirectToPreviousPage(formData.DelegateId, progressId, accessedVia, formData.ReturnPage);
        }

        [HttpGet]
        [Route("EditAdminField/{promptNumber:int}")]
        public IActionResult EditDelegateCourseAdminField(
            int progressId,
            int promptNumber,
            DelegateAccessRoute accessedVia,
            int? returnPage
        )
        {
            var delegateCourseProgress =
                courseService.GetDelegateCourseProgress(progressId);

            var courseAdminField = courseAdminFieldsService.GetCourseAdminFieldsForCourse(
                delegateCourseProgress!.DelegateCourseInfo.CustomisationId
            ).AdminFields.Find(caf => caf.PromptNumber == promptNumber);

            if (courseAdminField == null)
            {
                return new NotFoundResult();
            }

            var model = new EditDelegateCourseAdminFieldViewModel(
                progressId,
                promptNumber,
                delegateCourseProgress!,
                accessedVia,
                returnPage
            );
            return View(model);
        }

        [HttpPost]
        [Route("EditAdminField/{promptNumber:int}")]
        public IActionResult EditDelegateCourseAdminField(
            EditDelegateCourseAdminFieldFormData formData,
            int promptNumber,
            int progressId,
            DelegateAccessRoute accessedVia
        )
        {
            if (!ModelState.IsValid)
            {
                var delegateCourseProgress =
                    courseService.GetDelegateCourseProgress(progressId);

                var model = new EditDelegateCourseAdminFieldViewModel(
                    formData,
                    delegateCourseProgress!,
                    progressId,
                    promptNumber,
                    accessedVia
                );
                return View(model);
            }

            progressService.UpdateCourseAdminFieldForDelegate(progressId, promptNumber, formData.Answer?.Trim());
            // TODO: HEEDLS-862 Account for all possible access routes in redirection
            return RedirectToPreviousPage(formData.DelegateId, progressId, accessedVia, formData.ReturnPage);
        }

        private IActionResult RedirectToPreviousPage(
            int delegateId,
            int progressId,
            DelegateAccessRoute accessedVia,
            int? returnPage = 1
        )
        {
            if (accessedVia.Equals(DelegateAccessRoute.CourseDelegates))
            {
                return RedirectToAction("Index", new { progressId, accessedVia, returnPage });
            }

            return RedirectToAction("Index", "ViewDelegate", new { delegateId });
        }

        [HttpGet]
        [Route("UnlockProgress")]
        public IActionResult UnlockProgress(
            int progressId,
            int customisationId,
            int delegateId,
            DelegateAccessRoute accessedVia
        )
        {
            progressService.UnlockProgress(progressId);

            if (accessedVia.Equals(DelegateAccessRoute.CourseDelegates))
            {
                return RedirectToAction("Index", "CourseDelegates", new { customisationId });
            }

            return RedirectToAction("Index", "ViewDelegate", new { delegateId });
        }

        [Route("LearningLog")]
        public IActionResult LearningLog(
            int progressId,
            DelegateAccessRoute accessedVia,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Descending
        )
        {
            sortBy ??= LearningLogSortByOptions.When.PropertyName;
            var learningLog = courseService.GetLearningLogDetails(progressId);

            if (learningLog == null)
            {
                return NotFound();
            }

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                null,
                new SortOptions(sortBy, sortDirection),
                null,
                null
            );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                learningLog.Entries,
                searchSortPaginationOptions
            );

            var model = new LearningLogViewModel(accessedVia, learningLog, result);
            return View(model);
        }

        [Route("AllLearningLogEntries")]
        public IActionResult AllLearningLogEntries(
            int progressId,
            DelegateAccessRoute accessedVia
        )
        {
            var learningLog = courseService.GetLearningLogDetails(progressId);

            if (learningLog == null)
            {
                return NotFound();
            }

            var model = new AllLearningLogEntriesViewModel(learningLog.Entries);
            return View(model);
        }

        [HttpGet("DetailedProgress")]
        public IActionResult DetailedProgress(int progressId, DelegateAccessRoute accessedVia)
        {
            var progressData = progressService.GetDetailedCourseProgress(progressId);
            if (progressData == null)
            {
                return NotFound();
            }

            var model = new DetailedCourseProgressViewModel(
                progressData,
                accessedVia,
                configuration.GetCurrentSystemBaseUrl()
            );
            return View(model);
        }
    }
}
