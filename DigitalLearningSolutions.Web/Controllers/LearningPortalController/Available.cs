namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Available;
    using Microsoft.AspNetCore.Mvc;

    public partial class LearningPortalController
    {
        [Route("/LearningPortal/Available/{page=1:int}")]
        public IActionResult Available(
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = GenericSortingHelper.Ascending,
            int page = 1
        )
        {
            TempData["LearningActivity"] = "Available";
            sortBy ??= CourseSortByOptions.Name.PropertyName;

            var availableCourses = courseDataService.GetAvailableCourses(
                User.GetCandidateIdKnownNotNull(),
                User.GetCentreIdKnownNotNull()
            );
            var bannerText = GetBannerText();
            var searchSortPaginationOptions = new SearchSortFilterAndPaginateOptions(
                new SearchOptions(searchString),
                new SortOptions(sortBy, sortDirection),
                null,
                new PaginationOptions(page)
            );

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                availableCourses,
                searchSortPaginationOptions
            );

            var model = new AvailablePageViewModel(
                result,
                bannerText
            );
            return View("Available/Available", model);
        }

        [NoCaching]
        public IActionResult AllAvailableItems()
        {
            var availableCourses = courseDataService.GetAvailableCourses(
                User.GetCandidateIdKnownNotNull(),
                User.GetCentreIdKnownNotNull()
            );
            var model = new AllAvailableItemsPageViewModel(availableCourses);
            return View("Available/AllAvailableItems", model);
        }

        public IActionResult EnrolOnSelfAssessment(int selfAssessmentId)
        {
            courseDataService.EnrolOnSelfAssessment(selfAssessmentId, User.GetUserIdKnownNotNull(), User.GetCentreIdKnownNotNull());
            return RedirectToAction("SelfAssessment", new { selfAssessmentId });
        }
    }
}
