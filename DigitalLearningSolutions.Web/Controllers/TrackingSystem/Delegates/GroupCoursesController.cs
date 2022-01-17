namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.Enums;
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
    [Route("TrackingSystem/Delegates/Groups/{groupId:int}/Courses")]
    public class GroupCoursesController : Controller
    {
        private readonly ICourseService courseService;
        private readonly IGroupsService groupsService;
        private readonly IUserService userService;

        public GroupCoursesController(
            IUserService userService,
            ICourseService courseService,
            IGroupsService groupsService
        )
        {
            this.userService = userService;
            this.courseService = courseService;
            this.groupsService = groupsService;
        }

        [Route("{page:int=1}")]
        public IActionResult Index(int groupId, int page = 1)
        {
            var centreId = User.GetCentreId();
            var groupName = groupsService.GetGroupName(groupId, centreId);

            if (groupName == null)
            {
                return NotFound();
            }

            var categoryIdFilter = User.GetAdminCourseCategoryFilter();

            var groupCourses = groupsService.GetGroupCoursesForCategory(groupId, centreId, categoryIdFilter);

            var model = new GroupCoursesViewModel(groupId, groupName, groupCourses, page);

            return View(model);
        }

        [Route("{groupCustomisationId:int}/Remove")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessGroup))]
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
        [ServiceFilter(typeof(VerifyAdminUserCanAccessGroup))]
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

        [HttpGet]
        [Route("Add/SelectCourse")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessGroup))]
        public IActionResult AddCourseToGroupSelectCourse(int groupId)
        {
            var centreId = User.GetCentreId();

            var adminCategoryFilter = User.GetAdminCourseCategoryFilter();

            var courses = courseService.GetEligibleCoursesToAddToGroup(centreId, adminCategoryFilter, groupId);

            var groupName = groupsService.GetGroupName(groupId, centreId);

            var model = new AddCourseToGroupCoursesViewModel(courses, groupId, groupName!);

            return View(model);
        }

        [HttpGet]
        [Route("Add/{customisationId:int}")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessGroup))]
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

        [HttpPost]
        [Route("Add/{customisationId:int}")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessGroup))]
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

            // TODO HEEDLS-658 Confirmation page
            return RedirectToAction("Index", new { groupId });
        }
    }
}
