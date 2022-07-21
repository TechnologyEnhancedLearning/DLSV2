namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.FindYourCentre;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement;

    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    [SetSelectedTab(nameof(NavMenuTab.FindYourCentre))]
    [RedirectDelegateOnlyToLearningPortal]
    public class FindYourCentreController : Controller
    {
        private const string FindCentreFilterCookieName = "FindCentre";
        private readonly ICentresService centresService;
        private readonly IRegionDataService regionDataService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly IConfiguration configuration;
        private readonly IFeatureManager featureManager;

        public FindYourCentreController(
            ICentresService centresService,
            IRegionDataService regionDataService,
            ISearchSortFilterPaginateService searchSortFilterPaginateService,
            IConfiguration configuration,
            IFeatureManager featureManager
        )
        {
            this.centresService = centresService;
            this.regionDataService = regionDataService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
            this.configuration = configuration;
            this.featureManager = featureManager;
        }

        [Route("FindYourCentre/{page=1:int}")]
        public async Task<IActionResult> Index(
            int page = 1,
            string? searchString = null,
            string? existingFilterString = null,
            string? newFilterToAdd = null,
            bool clearFilters = false,
            int? itemsPerPage = null
        )
        {
            if (!await featureManager.IsEnabledAsync(FeatureFlags.RefactoredFindYourCentrePage))
            {
                var model = new FindYourCentreViewModel(configuration);

                return View("Index", model);
            }

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
                new SearchOptions(searchString, searchMatchCutoff: 90),
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

            var refactoredModel = new RefactoredFindYourCentreViewModel(
                result,
                availableFilters
            );

            Response.UpdateFilterCookie(FindCentreFilterCookieName, result.FilterString);

            return View("RefactoredFindYourCentre", refactoredModel);
        }

        public IActionResult CentreData()
        {
            var centres = centresService.GetAllCentreSummariesForMap();
            return Json(centres);
        }
    }
}
