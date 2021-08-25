namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AllDelegates;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("TrackingSystem/Delegates/All")]
    public class AllDelegatesController : Controller
    {
        private const string DelegateFilterCookieName = "DelegateFilter";
        private readonly CentreCustomPromptHelper centreCustomPromptHelper;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IUserDataService userDataService;

        public AllDelegatesController(
            IUserDataService userDataService,
            CentreCustomPromptHelper centreCustomPromptHelper,
            IJobGroupsDataService jobGroupsDataService
        )
        {
            this.userDataService = userDataService;
            this.centreCustomPromptHelper = centreCustomPromptHelper;
            this.jobGroupsDataService = jobGroupsDataService;
        }

        [Route("{page=1:int}")]
        public IActionResult Index(
            int page = 1,
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = BaseSearchablePageViewModel.Ascending,
            string? filterBy = null,
            string? filterValue = null
        )
        {
            if (filterBy == null && filterValue == null)
            {
                filterBy = Request.Cookies[DelegateFilterCookieName];
            }
            else if (filterBy?.ToUpper() == FilteringHelper.ClearString)
            {
                filterBy = null;
            }

            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            filterBy = FilteringHelper.AddNewFilterToFilterBy(filterBy, filterValue);

            var centreId = User.GetCentreId();
            var delegateUsers = userDataService.GetDelegateUserCardsByCentreId(centreId);
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical();
            var model = new AllDelegatesViewModel(
                centreId,
                delegateUsers,
                jobGroups,
                centreCustomPromptHelper,
                page,
                searchString,
                sortBy,
                sortDirection,
                filterBy
            );

            Response.UpdateOrDeleteFilterCookie(DelegateFilterCookieName, filterBy);

            return View(model);
        }

        [Route("AllDelegateItems")]
        public IActionResult AllDelegateItems()
        {
            var centreId = User.GetCentreId();
            var delegateUsers = userDataService.GetDelegateUserCardsByCentreId(centreId);
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical();
            var model = new AllDelegateItemsViewModel(centreId, delegateUsers, jobGroups, centreCustomPromptHelper);
            return View(model);
        }
    }
}
