namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Collections.Immutable;
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.LearningContent;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;
    using Microsoft.AspNetCore.Mvc;

    [RedirectDelegateOnlyToLearningPortal]
    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    [SetSelectedTab(nameof(NavMenuTab.Welcome))]
    public class LearningContentController : Controller
    {
        private const string BrandCoursesFilterCookieName = "BrandCoursesFilter";
        private readonly IBrandsService brandsService;
        private readonly ICourseService courseService;
        private readonly ISearchSortFilterPaginateService searchSortFilterPaginateService;
        private readonly ITutorialService tutorialService;

        public LearningContentController(
            IBrandsService brandsService,
            ITutorialService tutorialService,
            ICourseService courseService,
            ISearchSortFilterPaginateService searchSortFilterPaginateService
        )
        {
            this.brandsService = brandsService;
            this.tutorialService = tutorialService;
            this.courseService = courseService;
            this.searchSortFilterPaginateService = searchSortFilterPaginateService;
        }

        [Route("Home/LearningContent/{brandId:int}")]
        public IActionResult Index(
            int brandId,
            int page = 1,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Ascending,
            string? existingFilterString = null,
            string? newFilterToAdd = null,
            bool clearFilters = false
        )
        {
            var brand = brandsService.GetPublicBrandById(brandId);
            if (brand == null)
            {
                return NotFound();
            }

            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            existingFilterString = FilteringHelper.GetFilterString(
                existingFilterString,
                newFilterToAdd,
                clearFilters,
                Request,
                BrandCoursesFilterCookieName
            );

            var tutorials = tutorialService.GetPublicTutorialSummariesForBrand(brandId);
            var applications = courseService.GetApplicationsThatHaveSectionsByBrandId(brandId).ToList();

            var categories = applications.Select(x => x.CategoryName).Distinct().OrderBy(x => x);
            var topics = applications.Select(x => x.CourseTopic).Distinct().OrderBy(x => x);
            var availableFilters = LearningContentViewModelFilterOptions
                .GetFilterOptions(categories, topics).ToList();

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                null,
                new SortOptions(sortBy, sortDirection),
                new FilterOptions(
                    existingFilterString,
                    availableFilters
                ),
                new PaginationOptions(page)
            );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                applications,
                searchSortPaginationOptions
            );

            var model = new LearningContentViewModel(result, availableFilters, brand, tutorials);

            return View(model);
        }
    }
}
