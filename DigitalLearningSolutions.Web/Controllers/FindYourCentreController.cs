namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.FindYourCentre;
    using Microsoft.AspNetCore.Mvc;

    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    [SetSelectedTab(nameof(NavMenuTab.FindYourCentre))]
    [Route("FindYourCentre")]
    public class FindYourCentreController : Controller
    {
        private const string FindCentreFilterCookieName = "FindCentre";
        private readonly ICentresService centresService;
        private readonly IRegionDataService regionDataService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;

        public FindYourCentreController(
            ICentresService centresService,
            IRegionDataService regionDataService,
            ISearchSortFilterPaginateService searchSortFilterPaginateService
        )
        {
            this.centresService = centresService;
            this.regionDataService = regionDataService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
        }

        [RedirectDelegateOnlyToLearningPortal]
        [Route("{page=1:int}")]
        public IActionResult Index(
            int page = 1,
            string? searchString = null,
            string? existingFilterString = null,
            string? newFilterToAdd = null,
            bool clearFilters = false,
            int? itemsPerPage = null
        )
        {
            existingFilterString = FilteringHelper.GetFilterString(
                existingFilterString,
                newFilterToAdd,
                clearFilters,
                Request,
                FindCentreFilterCookieName
            );

            var centreSummaries = centresService.GetAllCentreSummariesForFindCentre();
            var regions = regionDataService.GetRegionsAlphabetical();

            var availableFilters = FindYourCentreViewModelFilterOptions
                .GetFindCentreFilterModels(regions).ToList();

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                new SearchOptions(searchString),
                null,
                new FilterOptions(
                    existingFilterString,
                    availableFilters
                ),
                new PaginationOptions(page, itemsPerPage)
            );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                centreSummaries,
                searchSortPaginationOptions
            );

            var model = new FindYourCentreViewModel(
                result,
                availableFilters
            );

            Response.UpdateFilterCookie(FindCentreFilterCookieName, result.FilterString);

            return View(model);
        }
    }
}
