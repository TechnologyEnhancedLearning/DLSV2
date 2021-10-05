namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.CustomPrompts;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("TrackingSystem/Delegates/Groups")]
    public class DelegateGroupsController : Controller
    {
        private const string DelegateGroupsFilterCookieName = "DelegateGroupsFilter";
        private readonly ICentreCustomPromptsService centreCustomPromptsService;
        private readonly IClockService clockService;
        private readonly IGroupsDataService groupsDataService;
        private readonly IGroupsService groupsService;

        public DelegateGroupsController(
            IGroupsDataService groupsDataService,
            ICentreCustomPromptsService centreCustomPromptsService,
            IClockService clockService,
            IGroupsService groupsService
        )
        {
            this.groupsDataService = groupsDataService;
            this.centreCustomPromptsService = centreCustomPromptsService;
            this.clockService = clockService;
            this.groupsService = groupsService;
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
            var groups = groupsDataService.GetGroupsForCentre(centreId).ToList();

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
            var groups = groupsDataService.GetGroupsForCentre(centreId).ToList();

            var model = new AllDelegateGroupsViewModel(groups, GetRegistrationPromptsWithSetOptions(centreId));

            return View(model);
        }

        [Route("{groupId:int}/Delegates/{page:int=1}")]
        public IActionResult GroupDelegates(int groupId, int page = 1)
        {
            var centreId = User.GetCentreId();
            var group = groupsDataService.GetGroupAtCentreById(groupId, centreId);
            var groupName = group?.GroupLabel;

            if (groupName == null)
            {
                return NotFound();
            }

            var groupDelegates = groupsDataService.GetGroupDelegates(groupId);

            var model = new GroupDelegatesViewModel(groupId, groupName, groupDelegates, page);

            return View(model);
        }

        [HttpGet]
        [Route("{groupId:int}/Delegates/Remove/{delegateId:int}")]
        public IActionResult GroupDelegatesRemove(int groupId, int delegateId)
        {
            var centreId = User.GetCentreId();
            var groupName = groupsDataService.GetGroupAtCentreById(groupId, centreId)?.GroupLabel;
            var groupDelegates = groupsDataService.GetGroupDelegates(groupId).ToList();
            var delegateUser = groupDelegates.SingleOrDefault(gd => gd.DelegateId == delegateId);

            if (groupName == null || delegateUser == null)
            {
                return NotFound();
            }

            var progressId = groupsDataService.GetRelatedProgressIdForGroupDelegate(groupId, delegateId);

            var model = new GroupDelegatesRemoveViewModel(delegateUser, groupName, groupId, progressId);

            return View(model);
        }

        [HttpPost]
        [Route("{groupId:int}/Delegates/Remove/{delegateId:int}")]
        public IActionResult GroupDelegatesRemove(GroupDelegatesRemoveViewModel model, int groupId, int delegateId)
        {
            var centreId = User.GetCentreId();
            var groupName = groupsDataService.GetGroupAtCentreById(groupId, centreId)?.GroupLabel;
            var groupDelegates = groupsDataService.GetGroupDelegates(groupId).ToList();
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

            using var transaction = new TransactionScope();
            if (model.RemoveProgress)
            {
                var currentDate = clockService.UtcNow;
                groupsDataService.RemoveRelatedProgressRecordsForGroupDelegate(groupId, delegateId, currentDate);
            }

            groupsDataService.DeleteGroupDelegatesRecordForDelegate(groupId, delegateId);
            transaction.Complete();

            return RedirectToAction("GroupDelegates", new { groupId });
        }

        [Route("{groupId:int}/Courses/{page:int=1}")]
        public IActionResult GroupCourses(int groupId, int page = 1)
        {
            var centreId = User.GetCentreId();
            var groupName = groupsDataService.GetGroupAtCentreById(groupId, centreId)?.GroupLabel;

            if (groupName == null)
            {
                return NotFound();
            }

            var groupCourses = groupsDataService.GetGroupCourses(groupId, centreId);

            var model = new GroupCoursesViewModel(groupId, groupName, groupCourses, page);

            return View(model);
        }

        [Route("{groupId:int}/EditDescription")]
        [HttpGet]
        public IActionResult EditDescription(int groupId)
        {
            var centreId = User.GetCentreId();
            var group = groupsDataService.GetGroupAtCentreById(groupId, centreId);

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
            if (!groupsDataService.TryUpdateGroupDescription(
                groupId,
                centreId,
                model.Description!
            ))
            {
                return NotFound();
            }

            return RedirectToAction("Index");
        }

        private IEnumerable<CustomPrompt> GetRegistrationPromptsWithSetOptions(int centreId)
        {
            return centreCustomPromptsService.GetCustomPromptsForCentreByCentreId(centreId).CustomPrompts
                .Where(cp => cp.Options.Any());
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
    }
}
