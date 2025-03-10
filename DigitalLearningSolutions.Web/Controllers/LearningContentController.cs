﻿namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Linq;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.LearningContent;
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

        [Route("Home/LearningContent/{brandId:int}/{page=1:int}")]
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
            var tutorials = tutorialService.GetPublicTutorialSummariesForBrand(brandId);
            var applications = courseService.GetApplicationsThatHaveSectionsByBrandId(brandId).ToList();

            var categories = applications.Select(x => x.CategoryName).Distinct().OrderBy(x => x).ToList();
            var topics = applications.Select(x => x.CourseTopic).Distinct().OrderBy(x => x).ToList();
            var availableFilters = LearningContentViewModelFilterOptions
                .GetFilterOptions(categories, topics).ToList();

            var filterString = FilteringHelper.GetFilterString(
                existingFilterString,
                newFilterToAdd,
                clearFilters,
                Request,
                BrandCoursesFilterCookieName,
                null,
                availableFilters
            );

            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                null,
                new SortOptions(sortBy, sortDirection),
                new FilterOptions(
                    filterString,
                    availableFilters
                ),
                new PaginationOptions(page)
            );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                applications,
                searchSortPaginationOptions
            );

            var model = new LearningContentViewModel(result, availableFilters, brand, tutorials);

            Response.UpdateFilterCookie(BrandCoursesFilterCookieName, result.FilterString);

            return View(model);
        }

        [NoCaching]
        [Route("Home/LearningContent/{brandId:int}/AllBrandCourses")]
        public IActionResult AllBrandCourses(int brandId)
        {
            var applications = courseService.GetApplicationsThatHaveSectionsByBrandId(brandId).ToList();

            var model = new AllBrandCoursesViewModel(applications);

            return View(model);
        }
    }
}
