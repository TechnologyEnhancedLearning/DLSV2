﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup
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
            var data = TempData.Peek<AddNewCentreCourseData>()!;

            var centreId = User.GetCentreId();
            var topics = courseTopicsDataService.GetCourseTopicsAvailableAtCentre(centreId).Select(c => c.CourseTopic);

            var applicationOptions = GetApplicationOptionsSelectList();

            var model = new SelectCourseViewModel(applicationOptions, data!.Application?.ApplicationId);

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
                var model = new SelectCourseViewModel(formData, applicationSelectList);
                return View("AddNewCentreCourse/SelectCourse", model);
            }

            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCourseCategoryFilter();
            var applicationOptions =
                courseService.GetApplicationOptionsAlphabeticalListForCentre(centreId, categoryId);

            var selectedApplication =
                applicationOptions.Single(ap => ap.ApplicationId == formData.ApplicationId);

            data!.SetApplicationAndResetModels(selectedApplication);

            TempData.Set(data);

            return RedirectToAction("SetCourseDetails");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpGet]
        [Route("AddCourse/SetCourseDetails")]
        public IActionResult SetCourseDetails()
        {
            var data = TempData.Peek<AddNewCentreCourseData>();
            var model = data?.SetCourseDetailsModel ?? new SetCourseDetailsViewModel(data!.Application!);

            return View("AddNewCentreCourse/SetCourseDetails", model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpPost]
        [Route("AddCourse/SetCourseDetails")]
        public IActionResult SetCourseDetails(SetCourseDetailsViewModel model)
        {
            var data = TempData.Peek<AddNewCentreCourseData>();
            var centreId = User.GetCentreId();

            if (string.IsNullOrWhiteSpace(model.CustomisationName))
            {
                model.CustomisationName = string.Empty;
            }

            CourseDetailsValidator.ValidateCustomisationName(
                model,
                ModelState,
                courseService,
                centreId
            );
            CourseDetailsValidator.ValidatePassword(model, ModelState);
            CourseDetailsValidator.ValidateEmail(model, ModelState);
            CourseDetailsValidator.ValidateCompletionCriteria(model, ModelState);

            if (!ModelState.IsValid)
            {
                return View("AddNewCentreCourse/SetCourseDetails", model);
            }

            data!.SetCourseDetailsModel = model;
            TempData.Set(data);

            return RedirectToAction("SetCourseOptions");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpGet]
        [Route("AddCourse/SetCourseOptions")]
        public IActionResult SetCourseOptions()
        {
            var data = TempData.Peek<AddNewCentreCourseData>();

            var model = data!.SetCourseOptionsModel ?? new EditCourseOptionsFormData();
            model.SetUpCheckboxes(data.Application!.DiagAssess);

            return View("AddNewCentreCourse/SetCourseOptions", model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpPost]
        [Route("AddCourse/SetCourseOptions")]
        public IActionResult SetCourseOptions(EditCourseOptionsFormData model)
        {
            var data = TempData.Peek<AddNewCentreCourseData>();

            data!.SetCourseOptionsModel = model;
            TempData.Set(data);

            return RedirectToAction("SetCourseContent");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpGet]
        [Route("AddCourse/SetCourseContent")]
        public IActionResult SetCourseContent()
        {
            var data = TempData.Peek<AddNewCentreCourseData>();

            var model = GetSetCourseContentModel(data!);

            data.SetCourseContentModel = model;
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

            data!.SetCourseContentModel = model;
            TempData.Set(data);

            return RedirectToAction("SetSectionContent");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpGet]
        [Route("AddCourse/SetSectionContent")]
        public IActionResult SetSectionContent()
        {
            var data = TempData.Peek<AddNewCentreCourseData>();

            var model = GetSetSectionContentModel(data!);

            return View("../AddNewCentreCourse/SetSectionContent", model);

            // Since I'll have the selected sections stored in temp data, I can pass the index of each section to the
            // SetSectionContent controller get method. And the post can either redirect back to the get method with
            // the next index, or to the next controller action if all sections have been assessed.
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<AddNewCentreCourseData>))]
        [HttpPost]
        [Route("AddCourse/SetSectionContent")]
        public IActionResult SetSectionContent(
            SetSectionContentViewModel model
        )
        {
            var data = TempData.Peek<AddNewCentreCourseData>();

            if (!ModelState.IsValid)
            {
                return View("AddNewCentreCourse/SetSectionContent", model);
            }

            data!.SetSectionContentModel = model;
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
                    data!.Application!.ApplicationId,
                    data.SetCourseDetailsModel!.CustomisationName ?? string.Empty,
                    data.SetCourseDetailsModel.Password,
                    data.SetCourseOptionsModel!.AllowSelfEnrolment,
                    int.Parse(data.SetCourseDetailsModel.TutCompletionThreshold!),
                    data.SetCourseDetailsModel.PostLearningAssessment,
                    int.Parse(data.SetCourseDetailsModel.DiagCompletionThreshold!),
                    data.SetCourseOptionsModel.DiagnosticObjectiveSelection,
                    data.SetCourseOptionsModel.HideInLearningPortal,
                    data.SetCourseDetailsModel.NotificationEmails
                );

                var tutorialModels = data.SetSectionContentModel!.GetTutorialsFromSections();
                var tutorials = tutorialModels.Select(
                    tm => new Tutorial(tm.TutorialId, tm.TutorialName, tm.LearningEnabled, tm.DiagnosticEnabled)
                );
                tutorialService.UpdateTutorialsStatuses(tutorials, customisationId);

                TempData.Clear();
                TempData.Add("customisationId", customisationId);
                TempData.Add("applicationName", data.SetCourseDetailsModel.ApplicationName);
                TempData.Add("customisationName", data.SetCourseDetailsModel.CustomisationName);

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

        private SetCourseContentViewModel GetSetCourseContentModel(AddNewCentreCourseData data)
        {
            if (data.SetCourseContentModel != null)
            {
                return data.SetCourseContentModel;
            }

            var sections =
                sectionService.GetSectionsForApplication(data!.Application!.ApplicationId);
            var sectionModels = sections.Select(section => new SelectSectionViewModel(section, false)).ToList();

            return new SetCourseContentViewModel(sectionModels);
        }

        private SetSectionContentViewModel GetSetSectionContentModel(AddNewCentreCourseData data)
        {
            if (data.SetSectionContentModel != null)
            {
                return data.SetSectionContentModel;
            }

            var model = new SetSectionContentViewModel(data.SetCourseContentModel!.SectionsToInclude);
            foreach (var section in model.Sections)
            {
                var tutorials = tutorialService.GetTutorialsForSection(section.SectionId);
                section.SetTutorials(tutorials);
            }

            return model;
        }
    }
}
