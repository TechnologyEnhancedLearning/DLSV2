﻿namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [Route("TrackingSystem/Delegates/Groups")]
    public class DelegateGroupsController : Controller
    {
        private const string DelegateGroupsFilterCookieName = "DelegateGroupsFilter";
        private readonly ICentreCustomPromptsService centreCustomPromptsService;
        private readonly ICourseService courseService;
        private readonly IGroupsService groupsService;
        private readonly IUserService userService;

        public DelegateGroupsController(
            ICentreCustomPromptsService centreCustomPromptsService,
            IGroupsService groupsService,
            IUserService userService,
            ICourseService courseService
        )
        {
            this.centreCustomPromptsService = centreCustomPromptsService;
            this.groupsService = groupsService;
            this.userService = userService;
            this.courseService = courseService;
        }

        [Route("{page=1:int}")]
        public IActionResult Index(
            string? sortBy = null,
            string sortDirection = BaseSearchablePageViewModel.Ascending,
            string? filterBy = null,
            string? filterValue = null,
            int page = 1
        )
        {
            if (filterBy == null && filterValue == null)
            {
                filterBy = Request.Cookies[DelegateGroupsFilterCookieName];
            }
            else if (filterBy?.ToUpper() == FilteringHelper.ClearString)
            {
                filterBy = null;
            }

            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            filterBy = FilteringHelper.AddNewFilterToFilterBy(filterBy, filterValue);

            var centreId = User.GetCentreId();
            var groups = groupsService.GetGroupsForCentre(centreId).ToList();

            var model = new DelegateGroupsViewModel(
                groups,
                GetRegistrationPromptsWithSetOptions(centreId),
                sortBy,
                sortDirection,
                filterBy,
                page
            );

            Response.UpdateOrDeleteFilterCookie(DelegateGroupsFilterCookieName, filterBy);

            return View(model);
        }

        [Route("AllDelegateGroups")]
        public IActionResult AllDelegateGroups()
        {
            var centreId = User.GetCentreId();
            var groups = groupsService.GetGroupsForCentre(centreId).ToList();

            var model = new AllDelegateGroupsViewModel(groups, GetRegistrationPromptsWithSetOptions(centreId));

            return View(model);
        }

        [Route("{groupId:int}/Delegates/{page:int=1}")]
        public IActionResult GroupDelegates(int groupId, int page = 1)
        {
            var centreId = User.GetCentreId();
            var groupName = groupsService.GetGroupName(groupId, centreId);

            if (groupName == null)
            {
                return NotFound();
            }

            var groupDelegates = groupsService.GetGroupDelegates(groupId);

            var model = new GroupDelegatesViewModel(groupId, groupName, groupDelegates, page);

            return View(model);
        }

        [HttpGet]
        [Route("{groupId:int}/Delegates/Remove/{delegateId:int}")]
        public IActionResult GroupDelegatesRemove(int groupId, int delegateId)
        {
            var centreId = User.GetCentreId();
            var groupName = groupsService.GetGroupName(groupId, centreId);
            var groupDelegates = groupsService.GetGroupDelegates(groupId).ToList();
            var delegateUser = groupDelegates.SingleOrDefault(gd => gd.DelegateId == delegateId);

            if (groupName == null || delegateUser == null)
            {
                return NotFound();
            }

            var progressId = groupsService.GetRelatedProgressIdForGroupDelegate(groupId, delegateId);

            var model = new GroupDelegatesRemoveViewModel(delegateUser, groupName, groupId, progressId);

            return View(model);
        }

        [HttpPost]
        [Route("{groupId:int}/Delegates/Remove/{delegateId:int}")]
        public IActionResult GroupDelegatesRemove(GroupDelegatesRemoveViewModel model, int groupId, int delegateId)
        {
            var centreId = User.GetCentreId();
            var groupName = groupsService.GetGroupName(groupId, centreId);
            var groupDelegates = groupsService.GetGroupDelegates(groupId).ToList();
            var delegateUser = groupDelegates.SingleOrDefault(gd => gd.DelegateId == delegateId);

            if (groupName == null || delegateUser == null)
            {
                return NotFound();
            }

            if (!model.ConfirmRemovalFromGroup)
            {
                ModelState.AddModelError(
                    nameof(GroupDelegatesRemoveViewModel.ConfirmRemovalFromGroup),
                    "You must confirm before removing this user from the group"
                );
                return View(model);
            }

            groupsService.RemoveDelegateFromGroup(groupId, delegateId, model.RemoveStartedEnrolments);

            return RedirectToAction("GroupDelegates", new { groupId });
        }

        [Route("{groupId:int}/Courses/{page:int=1}")]
        public IActionResult GroupCourses(int groupId, int page = 1)
        {
            var centreId = User.GetCentreId();
            var groupName = groupsService.GetGroupName(groupId, centreId);

            if (groupName == null)
            {
                return NotFound();
            }

            var groupCourses = groupsService.GetGroupCourses(groupId, centreId);

            var model = new GroupCoursesViewModel(groupId, groupName, groupCourses, page);

            return View(model);
        }

        [Route("{groupId:int}/Delete")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessGroup))]
        public IActionResult DeleteGroup(int groupId)
        {
            var delegates = groupsService.GetGroupDelegates(groupId);
            var courses = groupsService.GetGroupCourses(groupId, User.GetCentreId());

            if (delegates.Any() || courses.Any())
            {
                return RedirectToAction("ConfirmDeleteGroup", new { groupId });
            }

            groupsService.DeleteDelegateGroup(groupId, false);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("{groupId:int}/Delete/Confirm")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessGroup))]
        public IActionResult ConfirmDeleteGroup(int groupId)
        {
            var groupLabel = groupsService.GetGroupName(groupId, User.GetCentreId())!;
            var delegateCount = groupsService.GetGroupDelegates(groupId).Count();
            var courseCount = groupsService.GetGroupCourses(groupId, User.GetCentreId()).Count();

            var model = new ConfirmDeleteGroupViewModel
            {
                GroupLabel = groupLabel,
                DelegateCount = delegateCount,
                CourseCount = courseCount,
            };

            return View(model);
        }

        [HttpPost]
        [Route("{groupId:int}/Delete/Confirm")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessGroup))]
        public IActionResult ConfirmDeleteGroup(int groupId, ConfirmDeleteGroupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            groupsService.DeleteDelegateGroup(groupId, model.DeleteEnrolments);

            return RedirectToAction("Index");
        }

        [Route("Add")]
        [HttpGet]
        public IActionResult AddDelegateGroup()
        {
            return View(new AddDelegateGroupViewModel());
        }

        [Route("Add")]
        [HttpPost]
        public IActionResult AddDelegateGroup(AddDelegateGroupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            groupsService.AddDelegateGroup(
                User.GetCentreId(),
                model.GroupName!,
                model.GroupDescription,
                User.GetAdminId()!.Value
            );
            return RedirectToAction("Index");
        }

        [Route("{groupId:int}/EditDescription")]
        [HttpGet]
        public IActionResult EditDescription(int groupId)
        {
            var centreId = User.GetCentreId();
            var group = groupsService.GetGroupAtCentreById(groupId, centreId);

            if (group == null)
            {
                return NotFound();
            }

            var model = new EditDelegateGroupDescriptionViewModel(group);
            return View(model);
        }

        [Route("{groupId:int}/EditDescription")]
        [HttpPost]
        public IActionResult EditDescription(EditDelegateGroupDescriptionViewModel model, int groupId)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var centreId = User.GetCentreId();
            groupsService.UpdateGroupDescription(
                groupId,
                centreId,
                model.Description
            );

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("{groupId:int}/Courses/Add/{customisationId:int}")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessGroup))]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessCourse))]
        public IActionResult AddCourseToGroup(int groupId, int customisationId)
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCategoryId()!.Value;
            var groupLabel = groupsService.GetGroupName(groupId, centreId)!;
            var courseDetails = courseService.GetCourseDetailsForAdminCategoryId(customisationId, centreId, categoryId);
            var supervisors = userService.GetSupervisorsAtCentreForCategory(centreId, courseDetails!.CourseCategoryId);
            var viewModel = new AddCourseViewModel(groupId, customisationId, supervisors, groupLabel, courseDetails);
            return View(viewModel);
        }

        [HttpPost]
        [Route("{groupId:int}/Courses/Add/{customisationId:int}")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessGroup))]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessCourse))]
        public IActionResult AddCourseToGroup(AddCourseFormData formData, int groupId, int customisationId)
        {
            if (!ModelState.IsValid)
            {
                var courseCategoryId = courseService.GetCourseCategoryId(customisationId);
                var supervisors = userService.GetSupervisorsAtCentreForCategory(User.GetCentreId(), courseCategoryId);
                var model = new AddCourseViewModel(formData, groupId, customisationId, supervisors);
                return View(model);
            }

            // TODO HEEDLS-658 Save + Confirmation page
            return RedirectToAction("GroupCourses", new { groupId });
        }

        private IEnumerable<CustomPrompt> GetRegistrationPromptsWithSetOptions(int centreId)
        {
            return centreCustomPromptsService.GetCustomPromptsForCentreByCentreId(centreId).CustomPrompts
                .Where(cp => cp.Options.Any());
        }
    }
}
