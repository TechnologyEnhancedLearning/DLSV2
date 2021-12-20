namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.AddNewCentreCourse;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseContent;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup.CourseDetails;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.CourseSetup))]
    [Route("/TrackingSystem/CourseSetup")]
    public class CourseSetupController : Controller
    {
        public const string SaveAction = "save";

        private const string CourseFilterCookieName = "CourseFilter";
        private readonly ICourseCategoriesDataService courseCategoriesDataService;
        private readonly ICourseService courseService;
        private readonly ICourseTopicsDataService courseTopicsDataService;
        private readonly ITutorialService tutorialService;

        public CourseSetupController(
            ICourseService courseService,
            ICourseCategoriesDataService courseCategoriesDataService,
            ICourseTopicsDataService courseTopicsDataService,
            ITutorialService tutorialService
        )
        {
            this.courseService = courseService;
            this.courseCategoriesDataService = courseCategoriesDataService;
            this.courseTopicsDataService = courseTopicsDataService;
            this.tutorialService = tutorialService;
        }

        [Route("{page=1:int}")]
        public IActionResult Index(
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = BaseSearchablePageViewModel.Ascending,
            string? filterBy = null,
            string? filterValue = null,
            int page = 1
        )
        {
            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            filterBy = FilteringHelper.GetFilterBy(
                filterBy,
                filterValue,
                Request,
                CourseFilterCookieName,
                CourseStatusFilterOptions.IsActive.FilterValue
            );

            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCourseCategoryFilter();
            var centreCourses =
                courseService.GetCentreSpecificCourseStatistics(centreId, categoryId);
            var categories = courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId)
                .Select(c => c.CategoryName);
            var topics = courseTopicsDataService.GetCourseTopicsAvailableAtCentre(centreId).Select(c => c.CourseTopic);

            var model = new CourseSetupViewModel(
                centreCourses,
                categories,
                topics,
                searchString,
                sortBy,
                sortDirection,
                filterBy,
                page
            );

            Response.UpdateOrDeleteFilterCookie(CourseFilterCookieName, filterBy);

            return View(model);
        }

        [Route("AllCourseStatistics")]
        public IActionResult AllCourseStatistics()
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCourseCategoryFilter();
            var centreCourses =
                courseService.GetCentreSpecificCourseStatistics(centreId, categoryId);
            var categories = courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId)
                .Select(c => c.CategoryName);
            var topics = courseTopicsDataService.GetCourseTopicsAvailableAtCentre(centreId).Select(c => c.CourseTopic);

            var model = new AllCourseStatisticsViewModel(centreCourses, categories, topics);
            return View(model);
        }

        [HttpGet]
        [Route("AddCourseNew")]
        public IActionResult AddCourseNew()
        {
            TempData.Clear();

            var addNewCentreCourseData = new AddNewCentreCourseData();
            TempData.Set(addNewCentreCourseData);

            return RedirectToAction("SelectCourse");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpGet]
        [Route("AddCourse/SelectCourse")]
        public IActionResult SelectCourse()
        {
            var centreId = User.GetCentreId();
            var topics = courseTopicsDataService.GetCourseTopicsAvailableAtCentre(centreId).Select(c => c.CourseTopic);

            var applicationOptions = GetApplicationOptionsSelectList();
            var model = new SelectCourseViewModel(applicationOptions);

            return View("AddNewCentreCourse/SelectCourse", model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpPost]
        [Route("AddCourse/SelectCourse")]
        public IActionResult SelectCourse(SelectCourseFormData formData)
        {
            var data = TempData.Peek<AddNewCentreCourseData>()!;

            if (!ModelState.IsValid)
            {
                var applicationOptions = GetApplicationOptionsSelectList();
                var model = new SelectCourseViewModel(applicationOptions);
                return View("AddNewCentreCourse/SelectCourse", model);
            }

            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCourseCategoryFilter();
            var selectedApplication =
                courseService.GetApplicationOptionsAlphabeticalListForCentre(centreId, categoryId)
                    .Single(ap => ap.ApplicationId == formData.ApplicationId);

            data!.SetCourse(selectedApplication);
            TempData.Set(data);

            return RedirectToAction("SetCourseDetails");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpGet]
        [Route("AddCourse/SetCourseDetails")]
        public IActionResult SetCourseDetails()
        {
            var data = TempData.Peek<AddNewCentreCourseData>()!;
            var model = new SetCourseDetailsViewModel(data!.SelectCourseViewModel.Application);

            return View("AddNewCentreCourse/SetCourseDetails", model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpPost]
        [Route("AddCourse/SetCourseDetails")]
        public IActionResult SetCourseDetails(EditCourseDetailsFormData formData)
        {
            var data = TempData.Peek<AddNewCentreCourseData>()!;

            var centreId = User.GetCentreId();
            var customisationName =
                formData.CustomisationName == null || string.IsNullOrWhiteSpace(formData.CustomisationName)
                    ? string.Empty
                    : formData.CustomisationName;

            // TODO: Move this validation to the form data?
            ValidateCustomisationName(0, customisationName, centreId, formData);
            ValidatePassword(formData);
            ValidateEmail(formData);
            ValidateCompletionCriteria(formData);

            if (!ModelState.IsValid)
            {
                var model = new SetCourseDetailsViewModel(data!.SelectCourseViewModel.Application);
                return View("AddNewCentreCourse/SetCourseDetails", model);
            }

            data!.SetCourseDetails(formData);
            TempData.Set(data);

            return RedirectToAction("SetCourseOptions");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpGet]
        [Route("AddCourse/SetCourseOptions")]
        public IActionResult SetCourseOptions()
        {
            var model = new EditCourseOptionsFormData();

            return View("AddNewCentreCourse/SetCourseOptions", model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpPost]
        [Route("AddCourse/SetCourseOptions")]
        public IActionResult SetCourseDetails(EditCourseOptionsFormData model)
        {
            var data = TempData.Peek<AddNewCentreCourseData>()!;

            if (!ModelState.IsValid)
            {
                return View("AddNewCentreCourse/SetCourseOptions", model);
            }

            data.SetCourseOptions(model);
            TempData.Set(data);

            return RedirectToAction("SetCourseContent");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpGet]
        [Route("AddCourse/SetCourseContent")]
        public IActionResult SetCourseContent()
        {
            var model = new SetCourseContentViewModel();

            return View("AddNewCentreCourse/SetCourseContent", model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpPost]
        [Route("AddCourse/SetCourseContent")]
        public IActionResult SetCourseContent(SetCourseContentViewModel model)
        {
            var data = TempData.Peek<AddNewCentreCourseData>()!;

            if (!ModelState.IsValid)
            {
                return View("AddNewCentreCourse/SetCourseContent", model);
            }

            data.SetCourseContent(model);
            TempData.Set(data);

            return RedirectToAction("EditSectionContent");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpGet]
        [Route("AddCourse/EditSectionContent")]
        public IActionResult EditSectionContent()
        {
            var model = new EditCourseSectionFormData();

            return View("AddNewCentreCourse/EditSectionContent", model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpPost]
        [Route("AddCourse/EditSectionContent")]
        public IActionResult EditSectionContent(
            EditCourseSectionFormData formData,
            int customisationId,
            string action
        )
        {
            var data = TempData.Peek<AddNewCentreCourseData>()!;

            /*if (!ModelState.IsValid)
            {
                return View("AddNewCentreCourse/SetSectionContent", model);
            }

            return action == SaveAction
                ? EditSave(formData, customisationId)
                : ProcessBulkSelect(formData, customisationId, action);*/

            /*data.SetSectionContent(model);
            TempData.Set(data);*/

            return RedirectToAction("Summary");
        }

        [HttpGet]
        [Route("AddCourse/Summary")]
        public IActionResult Summary()
        {
            var data = TempData.Peek<AddNewCentreCourseData>()!;

            var model = new SummaryViewModel();
            data.PopulateSummaryData(model);

            return View("AddNewCentreCourse/Summary", model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpPost]
        [Route("AddCourse/Summary")]
        public IActionResult Summary(SummaryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("AddNewCentreCourse/Summary", model);
            }

            var data = TempData.Peek<AddNewCentreCourseData>()!;

            // TODO: Use a try...catch statement to try saving the new course, and update the tutorial statuses

            // courseService.CreateNewCentreCourse();
            // tutorialService.UpdateTutorialsStatuses();

            // TODO: Get the newly created course's customisationId and customisationName and save them in tempdata

            // TempData.Clear();
            // TempData.Add("customisationId", customisationId);
            // TempData.Add("customisationName", customisationName);

            return RedirectToAction("Confirmation");
        }

        [HttpGet]
        [Route("AddCourse/Confirmation")]
        public IActionResult Confirmation()
        {
            var customisationId = (int)TempData.Peek("customisationId");
            var customisationName = (string)TempData.Peek("courseName");
            TempData.Clear();

            var model = new ConfirmationViewModel(customisationId, customisationName);

            return View("AddNewCentreCourse/Confirmation", model);
        }

        private IEnumerable<SelectListItem> GetApplicationOptionsSelectList(int? selectedId = null)
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCourseCategoryFilter()!;
            var categoryIdFilter = categoryId == 0 ? null : categoryId;

            var orderedApplications = courseService
                .GetApplicationOptionsAlphabeticalListForCentre(centreId, categoryIdFilter)
                .ToList();
            var applicationOptions = orderedApplications.Select(a => (a.ApplicationId, a.ApplicationName));

            return SelectListHelper.MapOptionsToSelectListItems(applicationOptions, selectedId);
        }

        private void ValidateCustomisationName(
            int customisationId,
            string customisationName,
            int centreId,
            EditCourseDetailsFormData formData
        )
        {
            if (customisationName == string.Empty && courseService.DoesCourseNameExistAtCentre(
                customisationId,
                customisationName,
                centreId,
                formData.ApplicationId
            ))
            {
                ModelState.AddModelError(
                    nameof(EditCourseDetailsViewModel.CustomisationName),
                    "A course with no add on already exists"
                );
            }
            else if (customisationName.Length > 250)
            {
                ModelState.AddModelError(
                    nameof(EditCourseDetailsViewModel.CustomisationName),
                    "Course name must be 250 characters or fewer, including any additions"
                );
            }
            // TODO: Refactor DoesCourseNameExistAtCentre to have customisationId optional
            else if (courseService.DoesCourseNameExistAtCentre(
                customisationId,
                customisationName,
                centreId,
                formData.ApplicationId
            ))
            {
                ModelState.AddModelError(
                    nameof(EditCourseDetailsViewModel.CustomisationName),
                    "Course name must be unique, including any additions"
                );
            }
        }

        private void ValidatePassword(EditCourseDetailsFormData formData)
        {
            if (formData.PasswordProtected)
            {
                return;
            }

            if (ModelState.HasError(nameof(formData.Password)))
            {
                ModelState.ClearErrorsOnField(nameof(formData.Password));
            }

            formData.Password = null;
        }

        private void ValidateEmail(EditCourseDetailsFormData formData)
        {
            if (formData.ReceiveNotificationEmails)
            {
                return;
            }

            if (ModelState.HasError(nameof(formData.NotificationEmails)))
            {
                ModelState.ClearErrorsOnField(nameof(formData.NotificationEmails));
            }

            formData.NotificationEmails = null;
        }

        private void ValidateCompletionCriteria(EditCourseDetailsFormData formData)
        {
            if (!formData.IsAssessed)
            {
                return;
            }

            if (ModelState.HasError(nameof(formData.TutCompletionThreshold)))
            {
                ModelState.ClearErrorsOnField(nameof(formData.TutCompletionThreshold));
            }

            if (ModelState.HasError(nameof(formData.DiagCompletionThreshold)))
            {
                ModelState.ClearErrorsOnField(nameof(formData.DiagCompletionThreshold));
            }

            formData.TutCompletionThreshold = "0";
            formData.DiagCompletionThreshold = "0";
        }
    }
}
