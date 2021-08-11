namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("TrackingSystem/Delegates/All")]
    public class AllDelegatesController : Controller
    {
        private readonly CustomPromptHelper customPromptHelper;
        private readonly IUserDataService userDataService;

        public AllDelegatesController(IUserDataService userDataService, CustomPromptHelper customPromptHelper)
        {
            this.userDataService = userDataService;
            this.customPromptHelper = customPromptHelper;
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
            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            filterBy = FilteringHelper.AddNewFilterToFilterBy(filterBy, filterValue);

            var centreId = User.GetCentreId();
            var delegateUsers = userDataService.GetDelegateUserCardsByCentreId(centreId);
            var model = new AllDelegatesViewModel(
                centreId,
                delegateUsers,
                customPromptHelper,
                page,
                searchString,
                sortBy,
                sortDirection,
                filterBy
            );

            return View(model);
        }

        [Route("AllDelegateItems")]
        public IActionResult AllDelegateItems()
        {
            var centreId = User.GetCentreId();
            var delegateUsers = userDataService.GetDelegateUserCardsByCentreId(centreId);
            var model = new AllDelegateItemsViewModel(centreId, delegateUsers, customPromptHelper);
            return View(model);
        }
    }
}
