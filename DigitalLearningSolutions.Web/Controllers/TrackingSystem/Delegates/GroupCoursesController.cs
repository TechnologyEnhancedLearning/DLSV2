namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupCourses;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [ServiceFilter(typeof(VerifyAdminUserCanAccessGroup))]
    [Route("TrackingSystem/Delegates/Groups/{groupId:int}/Courses")]
    public class GroupCoursesController : Controller
    {
        private const string GroupAddCourseFilterCookieName = "GroupAddCourseFilter";
        private readonly ICourseService courseService;
        private readonly IGroupsService groupsService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly IUserService userService;

        public GroupCoursesController(
            IUserService userService,
            ICourseService courseService,
            IGroupsService groupsService,
            ISearchSortFilterPaginateService searchSortFilterPaginateService
        )
        {
            this.userService = userService;
            this.courseService = courseService;
            this.groupsService = groupsService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
        }

        [Route("{page:int=1}")]
        public IActionResult Index(int groupId, int page = 1)
        {
            var centreId = User.GetCentreId();
            var groupName = groupsService.GetGroupName(groupId, centreId);

            var categoryIdFilter = User.GetAdminCourseCategoryFilter();

            var groupCourses = groupsService.GetGroupCoursesForCategory(groupId, centreId, categoryIdFilter);

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                null,
                null,
                null,
                new PaginationOptions(page)
            );
            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                groupCourses,
                searchSortPaginationOptions
            );

            var model = new GroupCoursesViewModel(groupId, groupName!, result);

            return View(model);
        }

        [Route("{groupCustomisationId:int}/Remove")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessGroupCourse))]
        public IActionResult RemoveGroupCourse(int groupId, int groupCustomisationId)
        {
            var centreId = User.GetCentreId();
            var groupName = groupsService.GetGroupName(groupId, centreId);
            var groupCourse = groupsService.GetUsableGroupCourseForCentre(groupCustomisationId, groupId, centreId);

            var model = new RemoveGroupCourseViewModel(
                groupCourse!.GroupCustomisationId,
                groupCourse.CourseName,
                groupName!
            );

            return View(model);
        }

        [HttpPost("{groupCustomisationId:int}/Remove")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessGroupCourse))]
        public IActionResult RemoveGroupCourse(int groupId, int groupCustomisationId, RemoveGroupCourseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            groupsService.RemoveGroupCourseAndRelatedProgress(
                groupCustomisationId,
                groupId,
                model.DeleteStartedEnrolments
            );

            return RedirectToAction(nameof(Index), new { groupId });
        }

        [HttpGet("Add/SelectCourse/{page:int=1}")]
        public IActionResult AddCourseToGroupSelectCourse(
            int groupId,
            string? searchString = null,
            string? existingFilterString = null,
            string? newFilterToAdd = null,
            int page = 1
        )
        {
            existingFilterString = FilteringHelper.GetFilterString(
                existingFilterString,
                newFilterToAdd,
                Request,
                GroupAddCourseFilterCookieName
            );

            var centreId = User.GetCentreId();

            var adminCategoryFilter = User.GetAdminCourseCategoryFilter();

            var courses = courseService.GetEligibleCoursesToAddToGroup(centreId, adminCategoryFilter, groupId).ToList();
            var categories = courseService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId);
            var topics = courseService.GetTopicsForCentreAndCentrallyManagedCourses(centreId);

            var groupName = groupsService.GetGroupName(groupId, centreId);

            var availableFilters = (adminCategoryFilter == null
                ? AddCourseToGroupViewModelFilterOptions.GetAllCategoriesFilters(categories, topics)
                : AddCourseToGroupViewModelFilterOptions.GetSingleCategoryFilters(courses)).ToList();

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                new SearchOptions(searchString),
                new SortOptions(nameof(CourseAssessmentDetails.CourseName), GenericSortingHelper.Ascending),
                new FilterOptions(
                    existingFilterString,
                    availableFilters,
                    null
                ),
                new PaginationOptions(page)
            );
            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                courses,
                searchSortPaginationOptions
            );

            var model = new AddCourseToGroupCoursesViewModel(
                result,
                availableFilters,
                adminCategoryFilter,
                groupId,
                groupName!
            );

            Response.UpdateOrDeleteFilterCookie(GroupAddCourseFilterCookieName, result.FilterString);

            return View(model);
        }

        [Route("AddCourseToGroupSelectCourseAllCourses")]
        public IActionResult AddCourseToGroupSelectCourseAllCourses(int groupId)
        {
            var centreId = User.GetCentreId();

            var adminCategoryFilter = User.GetAdminCourseCategoryFilter();

            var courses = courseService.GetEligibleCoursesToAddToGroup(centreId, adminCategoryFilter, groupId);
            var categories = courseService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId);
            var topics = courseService.GetTopicsForCentreAndCentrallyManagedCourses(centreId);
            var model = new AddCourseToGroupCoursesAllCoursesViewModel(courses, categories, topics, groupId);
            return View("AddCourseToGroupSelectCourseAllCourses", model);
        }

        [HttpGet("Add/{customisationId:int}")]
        [ServiceFilter(typeof(VerifyAdminUserCanViewCourse))]
        public IActionResult AddCourseToGroup(int groupId, int customisationId)
        {
            var centreId = User.GetCentreId();
            var groupLabel = groupsService.GetGroupName(groupId, centreId)!;
            var courseCategoryId = courseService.GetCourseCategoryId(customisationId, centreId)!.Value;
            var courseNameInfo = courseService.GetCourseNameAndApplication(customisationId)!;
            var supervisors = userService.GetSupervisorsAtCentreForCategory(centreId, courseCategoryId);
            var viewModel = new AddCourseViewModel(groupId, customisationId, supervisors, groupLabel!, courseNameInfo!);
            return View(viewModel);
        }

        [HttpPost("Add/{customisationId:int}")]
        [ServiceFilter(typeof(VerifyAdminUserCanViewCourse))]
        public IActionResult AddCourseToGroup(AddCourseFormData formData, int groupId, int customisationId)
        {
            var centreId = User.GetCentreId();
            if (!ModelState.IsValid)
            {
                var courseCategoryId = courseService.GetCourseCategoryId(customisationId, centreId)!.Value;
                var supervisors = userService.GetSupervisorsAtCentreForCategory(centreId, courseCategoryId);
                var model = new AddCourseViewModel(formData, groupId, customisationId, supervisors);
                return View(model);
            }

            var completeWithinMonths = formData.MonthsToComplete == null ? 0 : int.Parse(formData.MonthsToComplete);
            groupsService.AddCourseToGroup(
                groupId,
                customisationId,
                completeWithinMonths,
                User.GetAdminId()!.Value,
                formData.CohortLearners,
                formData.SupervisorId,
                centreId
            );

            var confirmationViewModel = new AddCourseConfirmationViewModel(
                groupId,
                formData.GroupName,
                formData.CourseName,
                completeWithinMonths
            );
            return View("AddCourseToGroupConfirmation", confirmationViewModel);
        }
    }
}
