namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.FindYourCentre;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    [SetSelectedTab(nameof(NavMenuTab.FindYourCentre))]
    public class FindYourCentreController : Controller
    {
        private const string FindCentreFilterCookieName = "FindCentre";

        private readonly IConfiguration configuration;

        private readonly ICentresService centresService;

        private readonly IRegionDataService regionDataService;

        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;

        public FindYourCentreController(
            IConfiguration configuration,
            ICentresService centresService,
            IRegionDataService regionDataService,
            ISearchSortFilterPaginateService searchSortFilterPaginateService
        )
        {
            this.configuration = configuration;
            this.centresService = centresService;
            this.regionDataService = regionDataService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
        }

        [RedirectDelegateOnlyToLearningPortal]
        [Route("FindYourCentre/{page=1:int}")]
        public IActionResult Index(
            int page = 1,
            string? searchString = null,
            string? existingFilterString = null,
            string? newFilterToAdd = null,
            bool clearFilters = false,
            int? itemsPerPage = null
        )
        {
            /*existingFilterString = FilteringHelper.GetFilterString(
                existingFilterString,
                newFilterToAdd,
                clearFilters,
                Request,
                FindCentreFilterCookieName,
                FindCentreActiveStatusFilterOptions.IsActive.FilterValue
            );*/

            var centreSummaries = centresService.GetAllCentreSummariesForFindCentre();
            var regions = regionDataService.GetRegionsAlphabetical();

            /*
            var availableFilters = FindCentreViewModelFilterOptions.FindCentreFilterViewModels(regions);
            */

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                new SearchOptions(searchString),
                new SortOptions(DefaultSortByOptions.Name.PropertyName, GenericSortingHelper.Ascending),
                /*new FilterOptions(
                    existingFilterString,
                    availableFilters,
                    FindCentreActiveStatusFilterOptions.IsActive.FilterValue
                ),*/
                null,
                new PaginationOptions(page, itemsPerPage)
            );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                centreSummaries,
                searchSortPaginationOptions
            );

            var model = new FindYourCentreViewModel(
                result
                /*availableFilters*/
            );

            /*
            Response.UpdateFilterCookie(DelegateFilterCookieName, result.FilterString);
            */

            return View(model);
        }
    }
}
