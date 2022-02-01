namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.GroupDelegates;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [Route("TrackingSystem/Delegates/Groups/{groupId:int}/Delegates")]
    [ServiceFilter(typeof(VerifyAdminUserCanAccessGroup))]
    public class GroupDelegatesController : Controller
    {
        private const string AddGroupDelegateCookieName = "AddGroupDelegateFilter";
        private readonly CentreCustomPromptHelper centreCustomPromptHelper;
        private readonly IGroupsService groupsService;
        private readonly IJobGroupsService jobGroupsService;
        private readonly IUserService userService;

        public GroupDelegatesController(
            IJobGroupsService jobGroupsService,
            IUserService userService,
            CentreCustomPromptHelper centreCustomPromptHelper,
            IGroupsService groupsService
        )
        {
            this.centreCustomPromptHelper = centreCustomPromptHelper;
            this.jobGroupsService = jobGroupsService;
            this.userService = userService;
            this.groupsService = groupsService;
        }

        [Route("{page:int=1}")]
        public IActionResult Index(int groupId, int page = 1)
        {
            var centreId = User.GetCentreId();
            var groupName = groupsService.GetGroupName(groupId, centreId);

            var groupDelegates = groupsService.GetGroupDelegates(groupId);

            var model = new GroupDelegatesViewModel(groupId, groupName!, groupDelegates, page);

            return View(model);
        }
        
        [HttpGet("Add/SelectDelegate/{page=1:int}")]
        public IActionResult SelectDelegate(
            int groupId,
            string? searchString = null,
            string? filterBy = null,
            string? filterValue = null,
            int page = 1
        )
        {
            filterBy = FilteringHelper.GetFilterBy(
                filterBy,
                filterValue,
                Request,
                AddGroupDelegateCookieName
            );

            var centreId = User.GetCentreId();
            var jobGroups = jobGroupsService.GetJobGroupsAlphabetical().ToList();
            var customPrompts = centreCustomPromptHelper.GetCustomPromptsForCentre(centreId).ToList();
            var delegateUsers = userService.GetDelegatesNotRegisteredForGroupByGroupId(groupId, centreId);
            var groupName = groupsService.GetGroupName(groupId, centreId);

            var model = new AddGroupDelegateViewModel(
                delegateUsers,
                jobGroups,
                customPrompts,
                page,
                groupId,
                groupName!,
                searchString,
                filterBy
            );

            Response.UpdateOrDeleteFilterCookie(AddGroupDelegateCookieName, filterBy);
            return View(model);
        }

        [HttpPost("Add/{delegateId:int}")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessDelegateUser))]
        public IActionResult AddDelegate(int groupId, int delegateId)
        {
            var delegateUser = userService.GetDelegateUserById(delegateId);
            var adminId = User.GetAdminId();

            groupsService.AddDelegateToGroupAndEnrolOnGroupCourses(
                groupId,
                delegateUser!,
                adminId
            );

            return RedirectToAction("ConfirmDelegateAdded", new { groupId, delegateId });
        }

        [HttpGet("Add/{delegateId:int}/Confirmation")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessDelegateUser))]
        public IActionResult ConfirmDelegateAdded(int groupId, int delegateId)
        {
            var delegateUser = userService.GetDelegateUserById(delegateId);

            var centreId = User.GetCentreId();
            var groupName = groupsService.GetGroupName(groupId, centreId);

            var model = new ConfirmDelegateAddedViewModel(delegateUser!.FullName, groupName!, groupId);
            return View(model);
        }

        [HttpGet("Add/SelectDelegate/AllItems")]
        public IActionResult SelectDelegateAllItems(int groupId)
        {
            var centreId = User.GetCentreId();
            var jobGroups = jobGroupsService.GetJobGroupsAlphabetical();
            var customPrompts = centreCustomPromptHelper.GetCustomPromptsForCentre(centreId);
            var delegateUsers = userService.GetDelegatesNotRegisteredForGroupByGroupId(groupId, centreId);

            var model = new SelectDelegateAllItemsViewModel(
                delegateUsers,
                jobGroups,
                customPrompts,
                groupId
            );

            return View(model);
        }

        [HttpGet("{delegateId:int}/Remove")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessDelegateUser))]
        public IActionResult RemoveGroupDelegate(int groupId, int delegateId)
        {
            var centreId = User.GetCentreId();
            var groupName = groupsService.GetGroupName(groupId, centreId);
            var groupDelegates = groupsService.GetGroupDelegates(groupId).ToList();
            var delegateUser = groupDelegates.SingleOrDefault(gd => gd.DelegateId == delegateId);

            var progressId = groupsService.GetRelatedProgressIdForGroupDelegate(groupId, delegateId);

            var model = new RemoveGroupDelegateViewModel(delegateUser!, groupName!, groupId, progressId);

            return View(model);
        }

        [HttpPost("{delegateId:int}/Remove")]
        [ServiceFilter(typeof(VerifyAdminUserCanAccessDelegateUser))]
        public IActionResult RemoveGroupDelegate(RemoveGroupDelegateViewModel model, int groupId, int delegateId)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            groupsService.RemoveDelegateFromGroup(groupId, delegateId, model.RemoveStartedEnrolments);

            return RedirectToAction("Index", new { groupId });
        }
    }
}
