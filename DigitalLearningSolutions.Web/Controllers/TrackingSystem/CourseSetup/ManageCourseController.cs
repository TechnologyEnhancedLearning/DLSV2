namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [ServiceFilter(typeof(VerifyAdminUserCanAccessCourse))]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.CourseSetup))]
    [Route("/TrackingSystem/CourseSetup/{customisationId:int}/Manage")]
    public class ManageCourseController : Controller
    {
        private readonly ICourseService courseService;

        public ManageCourseController(ICourseService courseService)
        {
            this.courseService = courseService;
        }

        [HttpGet]
        public IActionResult Index(int customisationId)
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCategoryId()!;

            var courseDetails = courseService.GetCourseDetailsForAdminCategoryId(
                customisationId,
                centreId,
                categoryId.Value
            );

            var model = new ManageCourseViewModel(courseDetails!);

            return View(model);
        }

        [HttpGet]
        [Route("LearningPathwayDefaults")]
        public IActionResult EditLearningPathwayDefaults(int customisationId)
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCategoryId()!;

            var courseDetails = courseService.GetCourseDetailsForAdminCategoryId(
                customisationId,
                centreId,
                categoryId.Value
            );

            var model = new EditLearningPathwayDefaultsViewModel(courseDetails!);

            return View(model);
        }

        [HttpPost]
        [Route("LearningPathwayDefaults")]
        public IActionResult SaveLearningPathwayDefaults(
            int customisationId,
            EditLearningPathwayDefaultsViewModel model
        )
        {
            if (customisationId != model.CustomisationId)
            {
                return new StatusCodeResult(500);
            }

            if (!ModelState.IsValid)
            {
                return View("EditLearningPathwayDefaults", model);
            }

            if (model.AutoRefresh)
            {
                // TODO in HEEDLS-442: Redirect to "Edit auto-refresh options" page
                return RedirectToAction("Index", new { customisationId = model.CustomisationId });
            }

            var completeWithinMonthsInt =
                model.CompleteWithinMonths == null ? 0 : int.Parse(model.CompleteWithinMonths);
            var validityMonthsInt =
                model.ValidityMonths == null ? 0 : int.Parse(model.ValidityMonths);

            courseService.UpdateLearningPathwayDefaultsForCourse(
                model.CustomisationId,
                completeWithinMonthsInt,
                validityMonthsInt,
                model.Mandatory,
                model.AutoRefresh
            );

            return RedirectToAction("Index", new { customisationId = model.CustomisationId });
        }

        [HttpGet]
        [Route("CourseDetails")]
        public IActionResult EditCourseDetails(int customisationId)
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCategoryId()!;

            var courseDetails = courseService.GetCourseDetailsForAdminCategoryId(
                customisationId,
                centreId,
                categoryId.Value
            );

            var model = new EditCourseDetailsViewModel(courseDetails!);

            return View(model);
        }

        [HttpPost]
        [Route("CourseDetails")]
        public IActionResult SaveCourseDetails(
            int customisationId,
            EditCourseDetailsViewModel model
        )
        {
            if (customisationId != model.CustomisationId)
            {
                return StatusCode(500);
            }

            var customisationNameSuffix =
                model.CustomisationNameSuffix == null ? "" : " - " + model.CustomisationNameSuffix;
            var customisationName = model.CustomisationName + customisationNameSuffix;

            ValidateCustomisationName(customisationName, model);
            ValidatePassword(model);
            ValidateEmail(model);
            ValidateCompletionCriteria(model);

            if (!ModelState.IsValid)
            {
                return View("EditCourseDetails", model);
            }

            var tutCompletionThreshold =
                model.TutCompletionThreshold == null ? 0 : int.Parse(model.TutCompletionThreshold);
            var diagCompletionThreshold =
                model.DiagCompletionThreshold == null ? 0 : int.Parse(model.DiagCompletionThreshold);

            courseService.UpdateCourseDetails(
                customisationId,
                customisationName,
                model.Password!,
                model.NotificationEmails!,
                model.IsAssessed,
                tutCompletionThreshold,
                diagCompletionThreshold
            );

            return RedirectToAction("Index", new { customisationId });
        }

        [HttpGet]
        [Route("EditCourseOptions")]
        public IActionResult EditCourseOptions(int customisationId)
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCategoryId()!;

            var courseOptions = courseService.GetCourseOptionsForAdminCategoryId(
                customisationId,
                centreId,
                categoryId.GetValueOrDefault()
            );

            var model = new EditCourseOptionsViewModel(courseOptions!, customisationId);
            return View(model);
        }

        [HttpPost]
        [Route("EditCourseOptions")]
        public IActionResult EditCourseOptions(
            int customisationId,
            EditCourseOptionsViewModel editCourseOptionsViewModel
        )
        {
            var courseOptions = new CourseOptions
            {
                Active = editCourseOptionsViewModel.Active,
                SelfRegister = editCourseOptionsViewModel.AllowSelfEnrolment,
                HideInLearnerPortal = editCourseOptionsViewModel.HideInLearningPortal,
                DiagObjSelect = editCourseOptionsViewModel.DiagnosticObjectiveSelection,
            };

            courseService.UpdateCourseOptions(courseOptions, customisationId);
            return RedirectToAction("Index", "ManageCourse", new { customisationId });
        }

        private void ValidateCustomisationName(string customisationName, EditCourseDetailsViewModel model)
        {
            if (customisationName.Length > 250)
            {
                ModelState.AddModelError(
                    nameof(EditCourseDetailsViewModel.CustomisationNameSuffix),
                    "Course name must be 250 characters or fewer, including any additions"
                );
            }
            else if (courseService.DoesCourseNameExistAtCentre(
                model.CustomisationId,
                customisationName,
                model.CentreId,
                model.ApplicationId
            ))
            {
                var uniqueNameErrorMessage = string.IsNullOrWhiteSpace(model.CustomisationNameSuffix)
                    ? "A course with this name already exists, add on to the course name to make it unique"
                    : "Course name must be unique, including any additions";

                ModelState.AddModelError(
                    nameof(EditCourseDetailsViewModel.CustomisationNameSuffix),
                    uniqueNameErrorMessage
                );
            }
        }

        private void ValidatePassword(EditCourseDetailsViewModel model)
        {
            if (model.PasswordProtected)
            {
                return;
            }

            if (ModelState.HasError(nameof(model.Password)))
            {
                ModelState.ClearErrorsOnField(nameof(model.Password));
            }

            model.Password = null;
        }

        private void ValidateEmail(EditCourseDetailsViewModel model)
        {
            if (model.ReceiveNotificationEmails)
            {
                return;
            }

            if (ModelState.HasError(nameof(model.NotificationEmails)))
            {
                ModelState.ClearErrorsOnField(nameof(model.NotificationEmails));
            }

            model.NotificationEmails = null;
        }

        private void ValidateCompletionCriteria(EditCourseDetailsViewModel model)
        {
            if (!model.IsAssessed)
            {
                return;
            }

            if (ModelState.HasError(nameof(model.TutCompletionThreshold)))
            {
                ModelState.ClearErrorsOnField(nameof(model.TutCompletionThreshold));
            }

            if (ModelState.HasError(nameof(model.DiagCompletionThreshold)))
            {
                ModelState.ClearErrorsOnField(nameof(model.DiagCompletionThreshold));
            }

            model.TutCompletionThreshold = "0";
            model.DiagCompletionThreshold = "0";
        }
    }
}
