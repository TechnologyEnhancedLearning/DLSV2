namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models;
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
        private readonly ISectionService sectionService;
        private readonly ITutorialService tutorialService;

        public CourseSetupController(
            ICourseService courseService,
            ICourseCategoriesDataService courseCategoriesDataService,
            ICourseTopicsDataService courseTopicsDataService,
            ITutorialService tutorialService,
            ISectionService sectionService
        )
        {
            this.courseService = courseService;
            this.courseCategoriesDataService = courseCategoriesDataService;
            this.courseTopicsDataService = courseTopicsDataService;
            this.tutorialService = tutorialService;
            this.sectionService = sectionService;
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
                var applicationSelectList = GetApplicationOptionsSelectList();
                var model = new SelectCourseViewModel(applicationSelectList);
                return View("AddNewCentreCourse/SelectCourse", model);
            }

            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCourseCategoryFilter();
            var applicationOptions =
                courseService.GetApplicationOptionsAlphabeticalListForCentre(centreId, categoryId);

            var selectedApplication =
                applicationOptions.Single(ap => ap.ApplicationId == formData.ApplicationId);

            data!.SetCourse(selectedApplication);
            TempData.Set(data);

            return RedirectToAction("SetCourseDetails");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpGet]
        [Route("AddCourse/SetCourseDetails")]
        public IActionResult SetCourseDetails()
        {
            var data = TempData.Peek<AddNewCentreCourseData>();
            var model = new SetCourseDetailsViewModel(data!.SelectCourseViewModel.Application);

            return View("AddNewCentreCourse/SetCourseDetails", model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpPost]
        [Route("AddCourse/SetCourseDetails")]
        public IActionResult SetCourseDetails(EditCourseDetailsFormData formData)
        {
            var data = TempData.Peek<AddNewCentreCourseData>();
            var centreId = User.GetCentreId();

            if (string.IsNullOrWhiteSpace(formData.CustomisationName))
            {
                formData.CustomisationName = string.Empty;
            }

            CourseDetailsValidator.ValidateCustomisationName(
                formData,
                ModelState,
                courseService,
                centreId
            );
            CourseDetailsValidator.ValidatePassword(formData, ModelState);
            CourseDetailsValidator.ValidateEmail(formData, ModelState);
            CourseDetailsValidator.ValidateCompletionCriteria(formData, ModelState);

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
            var data = TempData.Peek<AddNewCentreCourseData>();

            var diagAssess = data!.SelectCourseViewModel.Application.DiagAssess;
            var model = new SetCourseOptionsViewModel(diagAssess);

            return View("AddNewCentreCourse/SetCourseOptions", model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpPost]
        [Route("AddCourse/SetCourseOptions")]
        public IActionResult SetCourseDetails(EditCourseOptionsFormData formData)
        {
            var data = TempData.Peek<AddNewCentreCourseData>();

            if (!ModelState.IsValid)
            {
                var diagAssess = data!.SelectCourseViewModel.Application.DiagAssess;
                var model = new SetCourseOptionsViewModel(diagAssess);
                return View("AddNewCentreCourse/SetCourseOptions", model);
            }

            data!.SetCourseOptions(formData);
            TempData.Set(data);

            return RedirectToAction("SetCourseContent");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpGet]
        [Route("AddCourse/SetCourseContent")]
        public IActionResult SetCourseContent()
        {
            var data = TempData.Peek<AddNewCentreCourseData>();

            var sections =
                sectionService.GetSectionsForApplication(data!.SelectCourseViewModel.Application.ApplicationId);
            var sectionModels = sections.Select(section => new SelectSectionViewModel(section, false)).ToList();

            var model = new SetCourseContentViewModel(sectionModels);

            data.SetCourseContent(model);
            TempData.Set(data);

            return View("AddNewCentreCourse/SetCourseContent", model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpPost]
        [Route("AddCourse/SetCourseContent")]
        public IActionResult SetCourseContent(SetCourseContentViewModel model)
        {
            var data = TempData.Peek<AddNewCentreCourseData>();

            model.SetSectionsToInclude();

            if (!ModelState.IsValid)
            {
                return View("AddNewCentreCourse/SetCourseContent", model);
            }

            data!.SetCourseContent(model);
            TempData.Set(data);

            return RedirectToAction("SetSectionContent");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpGet]
        [Route("AddCourse/EditSectionContent")]
        public IActionResult SetSectionContent()
        {
            var data = TempData.Peek<AddNewCentreCourseData>();

            var model = new SetSectionContentViewModel(data!.SetCourseContentViewModel.SectionsToInclude);
            foreach (var section in model.Sections)
            {
                var tutorials = tutorialService.GetTutorialsForSection(section.SectionId);
                section.SetTutorials(tutorials);
            }

            data.SetSectionContent(model);
            TempData.Set(data);

            return View("../AddNewCentreCourse/SetSectionContent", model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpPost]
        [Route("AddCourse/EditSectionContent")]
        public IActionResult SetSectionContent(
            SetSectionContentViewModel model
        )
        {
            var data = TempData.Peek<AddNewCentreCourseData>();

            if (!ModelState.IsValid)
            {
                return View("AddNewCentreCourse/SetSectionContent", model);
            }

            data!.SetSectionContent(model);
            TempData.Set(data);

            return RedirectToAction("Summary");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpGet]
        [Route("AddCourse/Summary")]
        public IActionResult Summary()
        {
            var data = TempData.Peek<AddNewCentreCourseData>();

            var model = new SummaryViewModel(data!);

            return View("AddNewCentreCourse/Summary", model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpPost]
        [Route("AddCourse/Summary")]
        public IActionResult Summary(SummaryViewModel model)
        {
            var data = TempData.Peek<AddNewCentreCourseData>()!;

            try
            {
                var centreId = User.GetCentreId();
                var customisationId = courseService.CreateNewCentreCourse(
                    centreId,
                    data.SelectCourseViewModel.ApplicationId.Value,
                    data.SetCourseDetailsViewModel.CustomisationName ?? string.Empty,
                    data.SetCourseDetailsViewModel.Password,
                    data.SetCourseOptionsViewModel.AllowSelfEnrolment,
                    int.Parse(data.SetCourseDetailsViewModel.TutCompletionThreshold!),
                    data.SetCourseDetailsViewModel.PostLearningAssessment,
                    int.Parse(data.SetCourseDetailsViewModel.DiagCompletionThreshold!),
                    data.SetCourseOptionsViewModel.DiagnosticObjectiveSelection,
                    data.SetCourseOptionsViewModel.HideInLearningPortal,
                    data.SetCourseDetailsViewModel.NotificationEmails
                );

                var tutorialModels = data.SetSectionContentViewModel.GetTutorialsFromSections();
                var tutorials = tutorialModels.Select(
                    tm => new Tutorial(tm.TutorialId, tm.TutorialName, tm.LearningEnabled, tm.DiagnosticEnabled)
                );
                tutorialService.UpdateTutorialsStatuses(tutorials, customisationId);

                TempData.Clear();
                TempData.Add("customisationId", customisationId);
                TempData.Add("applicationName", data.SetCourseDetailsViewModel.ApplicationName);
                TempData.Add("customisationName", data.SetCourseDetailsViewModel.CustomisationName);

                return RedirectToAction("Confirmation");
            }
            catch (Exception e)
            {
                return new StatusCodeResult(500);
            }
        }

        [HttpGet]
        [Route("AddCourse/Confirmation")]
        public IActionResult Confirmation()
        {
            var customisationId = (int)TempData.Peek("customisationId");
            var applicationName = (string)TempData.Peek("applicationName");
            var customisationName = (string)TempData.Peek("customisationName");
            TempData.Clear();

            var model = new ConfirmationViewModel(customisationId, applicationName, customisationName);

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
    }
}
