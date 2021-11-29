namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AddDelegateToGroup;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [Route("TrackingSystem/Delegates/Groups/AddDelegateToGroup")]
    public class AddDelegateToGroupController : Controller
    {
        private const string AddDelegateToGroupCookieName = "AddDelegateToGroupFilter";
        private readonly CentreCustomPromptHelper centreCustomPromptHelper;
        private readonly IClockService clockService;
        private readonly IGroupsDataService groupsDataService;
        private readonly IGroupsService groupsService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IUserDataService userDataService;

        public AddDelegateToGroupController(
            IJobGroupsDataService jobGroupsDataService,
            IUserDataService userDataService,
            CentreCustomPromptHelper centreCustomPromptHelper,
            IGroupsService groupsService,
            IGroupsDataService groupsDataService,
            IClockService clockService
        )
        {
            this.centreCustomPromptHelper = centreCustomPromptHelper;
            this.jobGroupsDataService = jobGroupsDataService;
            this.userDataService = userDataService;
            this.groupsService = groupsService;
            this.groupsDataService = groupsDataService;
            this.clockService = clockService;
        }

        [Route("{groupId:int}")]
        public IActionResult Index(
            int groupId = 1,
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
                AddDelegateToGroupCookieName,
                DelegateActiveStatusFilterOptions.IsActive.FilterValue
            );

            var centreId = User.GetCentreId();
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical();
            var customPrompts = centreCustomPromptHelper.GetCustomPromptsForCentre(centreId);
            var delegateUsers = userDataService.GetDelegatesNotRegisteredForGroupByGroupId(groupId, centreId);
            var groupName = userDataService.GetGroupNameById(groupId);

            TempData.Clear();
            TempData["groupId"] = groupId;

            var model = new AddDelegateToGroupViewModel(
                delegateUsers,
                jobGroups,
                customPrompts,
                page,
                groupId,
                groupName!,
                searchString,
                sortBy,
                sortDirection,
                filterBy
            );

            Response.UpdateOrDeleteFilterCookie(AddDelegateToGroupCookieName, filterBy);
            return View(model);
        }

        [Route("DelegateAddedToGroupConfirmation/{groupId:int}/{delegateId:int}")]
        public IActionResult DelegateAddedToGroupConfirmation(int groupId, int delegateId)
        {
            var delegateUser = userDataService.GetDelegateUserById(delegateId);
            if (delegateUser is null)
            {
                return NotFound();
            }

            var adminId = User.GetAdminId();
            var groupName = userDataService.GetGroupNameById(groupId);

            var newDetails = new MyAccountDetailsData(
                adminId,
                delegateUser?.Id,
                string.Empty,
                delegateUser?.FirstName!,
                delegateUser?.LastName!,
                delegateUser?.EmailAddress!,
                null
            );

            groupsDataService.AddDelegateToGroup(
                delegateUser!.Id,
                groupId,
                clockService.UtcNow,
                0
            );

            groupsService.EnrolDelegateOnGroupCourses(
                delegateUser!,
                newDetails,
                groupId,
                adminId
            );

            var model = new DelegateAddedToGroupConfirmationViewModel(delegateUser?.FullName!, groupName!, groupId);
            return View(model);
        }

        [Route("AddDelegateToGroupItems")]
        public IActionResult AddDelegateToGroupItems()
        {
            var groupId = (int)TempData["groupId"];
            var centreId = User.GetCentreId();
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical();
            var customPrompts = centreCustomPromptHelper.GetCustomPromptsForCentre(centreId);
            var delegateUsers = userDataService.GetDelegatesNotRegisteredForGroupByGroupId(groupId, centreId);

            var model = new AddDelegateToGroupItemsViewModel(
                delegateUsers,
                jobGroups,
                customPrompts,
                groupId
            );

            return View(model);
        }
    }
}
