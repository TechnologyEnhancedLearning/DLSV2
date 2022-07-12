namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [ServiceFilter(typeof(VerifyAdminUserCanManageCourse))]
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
            TempData.Clear();
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCourseCategoryFilter();

            var courseDetails = courseService.GetCourseDetailsFilteredByCategory(
                customisationId,
                centreId,
                categoryId
            );

            var model = new ManageCourseViewModel(courseDetails!);

            return View(model);
        }

        [HttpGet]
        [Route("LearningPathwayDefaults/New")]
        public IActionResult EditLearningPathwayDefaultsNew(int customisationId)
        {
            return RedirectToAction(nameof(EditLearningPathwayDefaults), new { customisationId });
        }

        [HttpGet]
        [Route("LearningPathwayDefaults")]
        public IActionResult EditLearningPathwayDefaults(int customisationId)
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCourseCategoryFilter();

            var courseDetails = courseService.GetCourseDetailsFilteredByCategory(
                customisationId,
                centreId,
                categoryId
            );

            var data = TempData.Get<EditLearningPathwayDefaultsData>();

            var model = data?.LearningPathwayDefaultsModel ?? new EditLearningPathwayDefaultsViewModel(courseDetails!);

            return View(model);
        }

        [HttpPost]
        [Route("LearningPathwayDefaults")]
        public IActionResult EditLearningPathwayDefaults(
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
                return View(model);
            }

            if (model.AutoRefresh)
            {
                SetEditLearningPathwayDefaultsTempData(model);

                return RedirectToAction("EditAutoRefreshOptions", new { customisationId = model.CustomisationId });
            }

            var completeWithinMonthsInt = GetIntegerFromStringOrConvertToZeroIfNull(model.CompleteWithinMonths);
            var validityMonthsInt = GetIntegerFromStringOrConvertToZeroIfNull(model.ValidityMonths);

            courseService.UpdateLearningPathwayDefaultsForCourse(
                model.CustomisationId,
                completeWithinMonthsInt,
                validityMonthsInt,
                model.Mandatory,
                model.AutoRefresh
            );

            return RedirectToAction("Index", new { customisationId = model.CustomisationId });
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<EditLearningPathwayDefaultsData>))]
        [HttpGet]
        [Route("AutoRefreshOptions")]
        public IActionResult EditAutoRefreshOptions(int customisationId)
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCourseCategoryFilter();

            var courseDetails = courseService.GetCourseDetailsFilteredByCategory(
                customisationId,
                centreId,
                categoryId
            )!;

            var courseOptions = GetCourseOptionsSelectList(customisationId, courseDetails.RefreshToCustomisationId);
            var model = new EditAutoRefreshOptionsViewModel(courseDetails, customisationId, courseOptions);

            return View(model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<EditLearningPathwayDefaultsData>))]
        [HttpPost]
        [Route("AutoRefreshOptions")]
        public IActionResult EditAutoRefreshOptions(
            int customisationId,
            EditAutoRefreshOptionsFormData formData
        )
        {
            if (!ModelState.IsValid)
            {
                var courseOptions = GetCourseOptionsSelectList(customisationId, formData.RefreshToCustomisationId);
                var model = new EditAutoRefreshOptionsViewModel(formData, customisationId, courseOptions);
                return View(model);
            }

            var data = TempData.Peek<EditLearningPathwayDefaultsData>()!;

            var autoRefreshMonths = GetIntegerFromStringOrConvertToZeroIfNull(formData.AutoRefreshMonths);

            var completeWithinMonthsInt =
                GetIntegerFromStringOrConvertToZeroIfNull(data.LearningPathwayDefaultsModel.CompleteWithinMonths);

            var validityMonthsInt =
                GetIntegerFromStringOrConvertToZeroIfNull(data.LearningPathwayDefaultsModel.ValidityMonths);

            var refreshToCustomisationId = formData.RefreshToCustomisationId ?? 0;

            courseService.UpdateLearningPathwayDefaultsForCourse(
                customisationId,
                completeWithinMonthsInt,
                validityMonthsInt,
                data.LearningPathwayDefaultsModel.Mandatory,
                data.LearningPathwayDefaultsModel.AutoRefresh,
                refreshToCustomisationId,
                autoRefreshMonths,
                formData.ApplyLpDefaultsToSelfEnrol
            );

            return RedirectToAction("Index", new { customisationId });
        }

        [HttpGet]
        [Route("CourseDetails")]
        public IActionResult EditCourseDetails(int customisationId)
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCourseCategoryFilter()!;

            var courseDetails = courseService.GetCourseDetailsFilteredByCategory(
                customisationId,
                centreId,
                categoryId
            );

            var model = new EditCourseDetailsViewModel(courseDetails!, customisationId);

            return View(model);
        }

        [HttpPost]
        [Route("CourseDetails")]
        public IActionResult SaveCourseDetails(
            int customisationId,
            EditCourseDetailsFormData formData
        )
        {
            var centreId = User.GetCentreId();

            CourseDetailsValidator.ValidateCustomisationName(
                formData,
                ModelState,
                courseService,
                centreId,
                customisationId
            );
            CourseDetailsValidator.ResetValueAndClearErrorsOnPasswordIfUnselected(formData, ModelState);
            CourseDetailsValidator.ResetValueAndClearErrorsOnEmailIfUnselected(formData, ModelState);
            CourseDetailsValidator.ResetValueAndClearErrorsOnOtherCompletionCriteriaIfUnselected(formData, ModelState);

            if (!ModelState.IsValid)
            {
                var model = new EditCourseDetailsViewModel(formData, customisationId);
                return View("EditCourseDetails", model);
            }

            var tutCompletionThreshold =
                formData.TutCompletionThreshold == null ? 0 : int.Parse(formData.TutCompletionThreshold);
            var diagCompletionThreshold =
                formData.DiagCompletionThreshold == null ? 0 : int.Parse(formData.DiagCompletionThreshold);

            courseService.UpdateCourseDetails(
                customisationId,
                formData.CustomisationName!,
                formData.Password!,
                formData.NotificationEmails!,
                formData.IsAssessed,
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
            var categoryId = User.GetAdminCourseCategoryFilter();

            var courseOptions = courseService.GetCourseOptionsFilteredByCategory(
                customisationId,
                centreId,
                categoryId
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

        private void SetEditLearningPathwayDefaultsTempData(EditLearningPathwayDefaultsViewModel model)
        {
            var data = new EditLearningPathwayDefaultsData(model);
            TempData.Set(data);
        }

        private IEnumerable<SelectListItem> GetCourseOptionsSelectList(int customisationId, int? selectedId = null)
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCourseCategoryFilter()!;
            var categoryIdFilter = categoryId == 0 ? null : categoryId;

            var centreCourses = courseService.GetCourseOptionsAlphabeticalListForCentre(centreId, categoryIdFilter)
                .ToList();
            centreCourses.RemoveAll(c => c.id == customisationId);
            centreCourses.Insert(0, (customisationId, "Same course"));

            return SelectListHelper.MapOptionsToSelectListItems(centreCourses, selectedId);
        }

        private int GetIntegerFromStringOrConvertToZeroIfNull(string? input)
        {
            return input == null ? 0 : int.Parse(input);
        }
    }
}
