namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using DigitalLearningSolutions.Data.Helpers;
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
            sortBy ??= CourseSortByOptions.Name.PropertyName;

            var availableCourses = courseDataService.GetAvailableCourses(
                User.GetCandidateIdKnownNotNull(),
                User.GetCentreId()
            );
            var bannerText = GetBannerText();

            var result = searchSortFilterPaginateService.SearchFilterSortAndPaginate(
                availableCourses,
                searchString,
                sortBy: sortBy,
                sortDirection: sortDirection,
                pageNumber: page
            );

            var model = new AvailablePageViewModel(
                result,
                bannerText
            );
            return View("Available/Available", model);
        }

        public IActionResult AllAvailableItems()
        {
            var availableCourses = courseDataService.GetAvailableCourses(
                User.GetCandidateIdKnownNotNull(),
                User.GetCentreId()
            );
            var model = new AllAvailableItemsPageViewModel(availableCourses);
            return View("Available/AllAvailableItems", model);
        }

        public IActionResult EnrolOnSelfAssessment(int selfAssessmentId)
        {
            courseDataService.EnrolOnSelfAssessment(selfAssessmentId, User.GetCandidateIdKnownNotNull());
            return RedirectToAction("SelfAssessment", new { selfAssessmentId });
        }
    }
}
