namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("TrackingSystem/Delegates/Groups")]
    public class DelegateGroupsController : Controller
    {
        private readonly IClockService clockService;
        private readonly IGroupsDataService groupsDataService;
        private readonly IUserDataService userDataService;

        public DelegateGroupsController(
            IGroupsDataService groupsDataService,
            IUserDataService userDataService,
            IClockService clockService
        )
        {
            this.groupsDataService = groupsDataService;
            this.userDataService = userDataService;
            this.clockService = clockService;
        }

        public IActionResult Index()
        {
            var centreId = User.GetCentreId();
            var groups = groupsDataService.GetGroupsForCentre(centreId);

            var model = new DelegateGroupsViewModel(groups);

            return View(model);
        }

        [Route("{groupId:int}/Delegates/{page:int=1}")]
        public IActionResult GroupDelegates(int groupId, int page = 1)
        {
            var centreId = User.GetCentreId();
            var groupName = groupsDataService.GetGroupName(groupId, centreId);

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
            var groupName = groupsDataService.GetGroupNameForGroupId(groupId);
            var delegateUser = userDataService.GetDelegateUserById(delegateId)!;

            var model = new GroupDelegatesRemoveViewModel(delegateUser, groupName, groupId);

            return View(model);
        }

        [HttpPost]
        [Route("{groupId:int}/Delegates/Remove/{delegateId:int}")]
        public IActionResult GroupDelegatesRemove(GroupDelegatesRemoveViewModel model, int groupId, int delegateId)
        {
            if (!model.Confirm)
            {
                ModelState.AddModelError(
                    nameof(GroupDelegatesRemoveViewModel.Confirm),
                    "You must confirm before removing this user from the group"
                );
                return View(model);
            }

            if (model.RemoveProgress)
            {
                var currentDate = clockService.UtcNow;
                groupsDataService.RemoveRelateProgressRecordsForDelegate(groupId, delegateId, currentDate);
            }

            groupsDataService.DeleteGroupDelegatesRecordForDelegate(groupId, delegateId);

            return RedirectToAction("GroupDelegates", new { groupId });
        }

        [Route("{groupId:int}/Courses/{page:int=1}")]
        public IActionResult GroupCourses(int groupId, int page = 1)
        {
            var centreId = User.GetCentreId();
            var groupName = groupsDataService.GetGroupName(groupId, centreId);

            if (groupName == null)
            {
                return NotFound();
            }
            var groupCourses = groupsDataService.GetGroupCourses(groupId, centreId);

            var model = new GroupCoursesViewModel(groupId, groupName, groupCourses, page);

            return View(model);
        }
    }
}
