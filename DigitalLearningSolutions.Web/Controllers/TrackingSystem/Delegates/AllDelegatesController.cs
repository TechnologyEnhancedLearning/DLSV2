namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AllDelegates;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [Route("TrackingSystem/Delegates/All")]
    public class AllDelegatesController : Controller
    {
        private const string DelegateFilterCookieName = "DelegateFilter";
        private readonly PromptsService promptsService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IUserDataService userDataService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;

        public AllDelegatesController(
            IUserDataService userDataService,
            PromptsService promptsService,
            IJobGroupsDataService jobGroupsDataService,
            ISearchSortFilterPaginateService searchSortFilterPaginateService
        )
        {
            this.userDataService = userDataService;
            this.promptsService = promptsService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
        }

        [Route("{page=1:int}")]
        public IActionResult Index(
            int page = 1,
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Ascending,
            string? existingFilterString = null,
            string? newFilterToAdd = null,
            int? itemsPerPage = null
        )
        {
            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            existingFilterString = FilteringHelper.GetFilterString(
                existingFilterString,
                newFilterToAdd,
                Request,
                DelegateFilterCookieName,
                DelegateActiveStatusFilterOptions.IsActive.FilterValue
            );

            var centreId = User.GetCentreId();
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical();
            var customPrompts = promptsService.GetCentreRegistrationPrompts(centreId).ToList();
            var delegateUsers = userDataService.GetDelegateUserCardsByCentreId(centreId);

            var promptsWithOptions = customPrompts.Where(customPrompt => customPrompt.Options.Count > 0);
            var availableFilters = AllDelegatesViewModelFilterOptions.GetAllDelegatesFilterViewModels(
                jobGroups,
                promptsWithOptions
            );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                delegateUsers,
                searchString,
                sortBy: sortBy,
                sortDirection : sortDirection,
                filterString: existingFilterString,
                defaultFilterString: DelegateActiveStatusFilterOptions.IsActive.FilterValue,
                availableFilters: availableFilters,
                pageNumber: page,
                itemsPerPage: itemsPerPage ?? SearchSortFilterPaginateService.DefaultItemsPerPage
            );

            var model = new AllDelegatesViewModel(
                result,
                customPrompts,
                availableFilters
            );

            Response.UpdateOrDeleteFilterCookie(DelegateFilterCookieName, result.FilterString);

            return View(model);
        }

        [NoCaching]
        [Route("AllDelegateItems")]
        public IActionResult AllDelegateItems()
        {
            var centreId = User.GetCentreId();
            var jobGroups = jobGroupsDataService.GetJobGroupsAlphabetical();
            var customPrompts = promptsService.GetCentreRegistrationPrompts(centreId);
            var delegateUsers = userDataService.GetDelegateUserCardsByCentreId(centreId);

            var model = new AllDelegateItemsViewModel(delegateUsers, jobGroups, customPrompts);

            return View(model);
        }
    }
}
