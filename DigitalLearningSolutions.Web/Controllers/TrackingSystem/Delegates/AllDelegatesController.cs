namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
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
        private readonly IDelegateDownloadFileService delegateDownloadFileService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly PromptsService promptsService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly IUserDataService userDataService;

        public AllDelegatesController(
            IDelegateDownloadFileService delegateDownloadFileService,
            IUserDataService userDataService,
            PromptsService promptsService,
            IJobGroupsDataService jobGroupsDataService,
            ISearchSortFilterPaginateService searchSortFilterPaginateService
        )
        {
            this.delegateDownloadFileService = delegateDownloadFileService;
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
            bool clearFilters = false,
            int? itemsPerPage = null
        )
        {
            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            existingFilterString = FilteringHelper.GetFilterString(
                existingFilterString,
                newFilterToAdd,
                clearFilters,
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

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                new SearchOptions(searchString),
                new SortOptions(sortBy, sortDirection),
                new FilterOptions(
                    existingFilterString,
                    availableFilters,
                    DelegateActiveStatusFilterOptions.IsActive.FilterValue
                ),
                new PaginationOptions(page, itemsPerPage)
            );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                delegateUsers,
                searchSortPaginationOptions
            );

            var model = new AllDelegatesViewModel(
                result,
                customPrompts,
                availableFilters
            );

            Response.UpdateFilterCookie(DelegateFilterCookieName, result.FilterString);

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

        [Route("Export")]
        public IActionResult Export(
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Ascending,
            string? existingFilterString = null
        )
        {
            var centreId = User.GetCentreId();
            var content = delegateDownloadFileService.GetAllDelegatesFileForCentre(
                centreId,
                searchString,
                sortBy,
                sortDirection,
                existingFilterString
            );

            const string fileName = "Digital Learning Solutions Delegates.xlsx";
            return File(
                content,
                FileHelper.GetContentTypeFromFileName(fileName),
                fileName
            );
        }
    }
}
