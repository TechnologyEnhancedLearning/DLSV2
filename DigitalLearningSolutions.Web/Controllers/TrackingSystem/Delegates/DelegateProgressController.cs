﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateProgress;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("TrackingSystem/Delegates/{accessedVia}/DelegateProgress/{progressId:int}")]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [ServiceFilter(typeof(VerifyDelegateProgressAccessedViaValidRoute))]
    [ServiceFilter(typeof(VerifyAdminUserCanAccessProgress))]
    public class DelegateProgressController : Controller
    {
        private readonly ICourseService courseService;
        private readonly IUserService userService;

        public DelegateProgressController(ICourseService courseService, IUserService userService)
        {
            this.courseService = courseService;
            this.userService = userService;
        }

        public IActionResult Index(int progressId, DelegateProgressAccessRoute accessedVia)
        {
            var centreId = User.GetCentreId();
            var courseDelegatesData =
                courseService.GetDelegateCourseProgress(progressId, centreId);

            var model = new DelegateProgressViewModel(accessedVia, courseDelegatesData!);
            return View(model);
        }

        [HttpGet]
        [Route("EditSupervisor")]
        public IActionResult EditSupervisor(int progressId, DelegateProgressAccessRoute accessedVia)
        {
            var centreId = User.GetCentreId();
            var delegateCourseProgress =
                courseService.GetDelegateCourseProgress(progressId, centreId);
            var supervisors = userService.GetSupervisorsAtCentre(centreId);

            var model = new EditSupervisorViewModel(progressId, accessedVia, supervisors, delegateCourseProgress!.DelegateCourseInfo);
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
            if (accessedVia.Equals(DelegateProgressAccessRoute.CourseDelegates))
            {
                return RedirectToAction("Index", new { progressId, accessedVia });
            }
            return RedirectToAction("Index", "ViewDelegate", new { formData.DelegateId });
        }
    }
}
