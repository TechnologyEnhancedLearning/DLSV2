namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.FindYourCentre;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.AllDelegates;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    [SetSelectedTab(nameof(NavMenuTab.FindYourCentre))]
    public class FindYourCentreController : Controller
    {
        private const string FindCentreFilterCookieName = "FindCentre";

        private readonly IConfiguration configuration;

        private readonly ICentresService centresService;

        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;

        public FindYourCentreController(IConfiguration configuration, ICentresService centresService, ISearchSortFilterPaginateService searchSortFilterPaginateService)
        {
            this.configuration = configuration;
            this.centresService = centresService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
        }

        /*[RedirectDelegateOnlyToLearningPortal]
        public IActionResult Index()
        {
            var centreSummaries = centresService.GetAllCentreSummariesForFindCentre();
            /*var model = centreId == null
                ? new FindYourCentreViewModel(configuration, centreSummaries)
                : new FindYourCentreViewModel(centreId, configuration);#1#
            var model = new FindYourCentreViewModel(configuration, centreSummaries);

            return View(model);
        }*/

        [Route("{page=1:int}")]
        public IActionResult Index(
            int page = 1,
            string? searchString = null,
            /*string? sortBy = null,
            string sortDirection = GenericSortingHelper.Ascending,*/
            string? existingFilterString = null,
            string? newFilterToAdd = null,
            bool clearFilters = false,
            int? itemsPerPage = null
        )
        {
            /*
            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            */
            /*existingFilterString = FilteringHelper.GetFilterString(
                existingFilterString,
                newFilterToAdd,
                clearFilters,
                Request,
                FindCentreFilterCookieName,
                FindCentreActiveStatusFilterOptions.IsActive.FilterValue
            );*/


            var centreSummaries = centresService.GetAllCentreSummariesForFindCentre();

            /*
            var availableFilters = FindCentreViewModelFilterOptions.GetAllDelegatesFilterViewModels(
                jobGroups,
                promptsWithOptions
            );
            */

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                new SearchOptions(searchString),
                null,
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
