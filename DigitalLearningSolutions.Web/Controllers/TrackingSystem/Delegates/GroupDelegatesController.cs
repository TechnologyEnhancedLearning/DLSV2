namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
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
        private readonly IGroupsService groupsService;
        private readonly IJobGroupsService jobGroupsService;
        private readonly PromptsService promptsService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly IUserService userService;

        public GroupDelegatesController(
            IJobGroupsService jobGroupsService,
            IUserService userService,
            PromptsService promptsService,
            IGroupsService groupsService,
            ISearchSortFilterPaginateService searchSortFilterPaginateService
        )
        {
            this.promptsService = promptsService;
            this.jobGroupsService = jobGroupsService;
            this.userService = userService;
            this.groupsService = groupsService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
        }

        [Route("{page:int=1}")]
        public IActionResult Index(int groupId, int page = 1)
        {
            var centreId = User.GetCentreId();
            var groupName = groupsService.GetGroupName(groupId, centreId);

            var groupDelegates = groupsService.GetGroupDelegates(groupId);

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                null,
                null,
                null,
                new PaginationOptions(page)
            );
            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                groupDelegates,
                searchSortPaginationOptions
            );

            var model = new GroupDelegatesViewModel(groupId, groupName!, result);

            return View(model);
        }

        [HttpGet("Add/SelectDelegate/{page=1:int}")]
        public IActionResult SelectDelegate(
            int groupId,
            string? searchString = null,
            string? existingFilterString = null,
            string? newFilterToAdd = null,
            bool clearFilters = false,
            int page = 1
        )
        {
            existingFilterString = FilteringHelper.GetFilterString(
                existingFilterString,
                newFilterToAdd,
                clearFilters,
                Request,
                AddGroupDelegateCookieName
            );

            var centreId = User.GetCentreId();
            var jobGroups = jobGroupsService.GetJobGroupsAlphabetical().ToList();
            var customPrompts = promptsService.GetCentreRegistrationPrompts(centreId).ToList();
            var delegateUsers = userService.GetDelegatesNotRegisteredForGroupByGroupId(groupId, centreId);
            var groupName = groupsService.GetGroupName(groupId, centreId);
            var promptsWithOptions = customPrompts.Where(customPrompt => customPrompt.Options.Count > 0);
            var availableFilters = GroupDelegatesViewModelFilterOptions.GetAddGroupDelegateFilterViewModels(
                jobGroups,
                promptsWithOptions
            );

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                new SearchOptions(searchString),
                new SortOptions(
                    DefaultSortByOptions.Name.PropertyName,
                    GenericSortingHelper.Ascending
                ),
                new FilterOptions(existingFilterString, availableFilters),
                new PaginationOptions(page)
            );
            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                delegateUsers,
                searchSortPaginationOptions
            );

            var model = new AddGroupDelegateViewModel(
                result,
                availableFilters,
                customPrompts,
                groupId,
                groupName!
            );

            Response.UpdateFilterCookie(AddGroupDelegateCookieName, result.FilterString);
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

            var model = new ConfirmDelegateAddedViewModel(delegateUser!, groupName!, groupId);
            return View(model);
        }

        [HttpGet("Add/SelectDelegate/AllItems")]
        public IActionResult SelectDelegateAllItems(int groupId)
        {
            var centreId = User.GetCentreId();
            var jobGroups = jobGroupsService.GetJobGroupsAlphabetical();
            var customPrompts = promptsService.GetCentreRegistrationPrompts(centreId);
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
        public IActionResult RemoveGroupDelegate(int groupId, int delegateId, ReturnPageQuery returnPageQuery)
        {
            var centreId = User.GetCentreId();
            var groupName = groupsService.GetGroupName(groupId, centreId);
            var groupDelegates = groupsService.GetGroupDelegates(groupId).ToList();
            var delegateUser = groupDelegates.SingleOrDefault(gd => gd.DelegateId == delegateId);

            var progressId = groupsService.GetRelatedProgressIdForGroupDelegate(groupId, delegateId);

            var model = new RemoveGroupDelegateViewModel(
                delegateUser!,
                groupName!,
                groupId,
                progressId,
                returnPageQuery
            );

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

            return RedirectToAction("Index", new { groupId, page = model.ReturnPageQuery.PageNumber });
        }
    }
}
