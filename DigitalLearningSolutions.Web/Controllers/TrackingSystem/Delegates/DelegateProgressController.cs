namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress;
    using DigitalLearningSolutions.Web.Views.TrackingSystem.Delegates.DelegateProgress;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("TrackingSystem/Delegates/{accessedVia}/DelegateProgress/{progressId:int}")]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [ServiceFilter(typeof(VerifyDelegateProgressAccessedViaValidRoute))]
    [ServiceFilter(typeof(VerifyAdminUserCanAccessProgress))]
    public class DelegateProgressController : Controller
    {
        private readonly ICourseService courseService;
        private readonly IProgressService progressService;
        private readonly IUserService userService;

        public DelegateProgressController(
            ICourseService courseService,
            IUserService userService,
            IProgressService progressService
        )
        {
            this.courseService = courseService;
            this.userService = userService;
            this.progressService = progressService;
        }

        public IActionResult Index(int progressId, DelegateProgressAccessRoute accessedVia)
        {
            var courseDelegatesData =
                courseService.GetDelegateCourseProgress(progressId);

            var model = new DelegateProgressViewModel(
                accessedVia,
                courseDelegatesData!
            );
            return View(model);
        }

        [HttpGet]
        [Route("EditSupervisor")]
        public IActionResult EditSupervisor(int progressId, DelegateProgressAccessRoute accessedVia)
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
                delegateCourseProgress!.DelegateCourseInfo
            );
            return View(model);
        }

        [HttpPost]
        [Route("EditSupervisor")]
        public IActionResult EditSupervisor(
            EditSupervisorFormData formData,
            int progressId,
            DelegateProgressAccessRoute accessedVia
        )
        {
            if (!ModelState.IsValid)
            {
                var supervisors = userService.GetSupervisorsAtCentre(User.GetCentreId());
                var model = new EditSupervisorViewModel(formData, progressId, accessedVia, supervisors);
                return View(model);
            }

            progressService.UpdateSupervisor(progressId, formData.SupervisorId);

            return RedirectToPreviousPage(formData.DelegateId, progressId, accessedVia);
        }

        [HttpGet]
        [Route("EditCompleteByDate")]
        public IActionResult EditCompleteByDate(int progressId, DelegateProgressAccessRoute accessedVia)
        {
            var delegateCourseProgress =
                courseService.GetDelegateCourseProgress(progressId);

            var model = new EditCompleteByDateViewModel(
                progressId,
                accessedVia,
                delegateCourseProgress!.DelegateCourseInfo
            );
            return View(model);
        }

        [HttpPost]
        [Route("EditCompleteByDate")]
        public IActionResult EditCompleteByDate(
            EditCompleteByDateFormData formData,
            int progressId,
            DelegateProgressAccessRoute accessedVia
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

            return RedirectToPreviousPage(formData.DelegateId, progressId, accessedVia);
        }

        [HttpGet]
        [Route("EditCompletionDate")]
        public IActionResult EditCompletionDate(int progressId, DelegateProgressAccessRoute accessedVia)
        {
            var delegateCourseProgress =
                courseService.GetDelegateCourseProgress(progressId);

            var model = new EditCompletionDateViewModel(
                progressId,
                accessedVia,
                delegateCourseProgress!.DelegateCourseInfo
            );
            return View(model);
        }

        [HttpPost]
        [Route("EditCompletionDate")]
        public IActionResult EditCompletionDate(
            EditCompletionDateFormData formData,
            int progressId,
            DelegateProgressAccessRoute accessedVia
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
            return RedirectToPreviousPage(formData.DelegateId, progressId, accessedVia);
        }

        private IActionResult RedirectToPreviousPage(
            int delegateId,
            int progressId,
            DelegateProgressAccessRoute accessedVia
        )
        {
            if (accessedVia.Equals(DelegateProgressAccessRoute.CourseDelegates))
            {
                return RedirectToAction("Index", new { progressId, accessedVia });
            }

            return RedirectToAction("Index", "ViewDelegate", new { delegateId });
        }

        [HttpGet]
        [Route("UnlockProgress")]
        public IActionResult UnlockProgress(
            int progressId,
            int customisationId,
            int delegateId,
            DelegateProgressAccessRoute accessedVia
        )
        {
            progressService.UnlockProgress(progressId);

            if (accessedVia.Equals(DelegateProgressAccessRoute.CourseDelegates))
            {
                return RedirectToAction("Index", "CourseDelegates", new { customisationId });
            }

            return RedirectToAction("Index", "ViewDelegate", new { delegateId });
        }

        [HttpGet("DetailedProgress")]
        public IActionResult DetailedProgress(int progressId, DelegateProgressAccessRoute accessedVia)
        {
            var progressData = progressService.GetDetailedCourseProgress(progressId);
            if (progressData == null)
            {
                return NotFound();
            }

            var candidateData = userService.GetDelegateUserById(progressData.DelegateId);
            if (candidateData == null)
            {
                return NotFound();
            }

            var courseData =
                courseService.GetDelegateCourseProgress(progressId);

            var model = new DetailedCourseProgressViewModel(candidateData, progressData, courseData, accessedVia);
            return View(model);
        }
    }
}
