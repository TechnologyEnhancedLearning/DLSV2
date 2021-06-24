namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Completed;
    using Microsoft.AspNetCore.Mvc;

    public partial class LearningPortalController
    {
        [Route("/LearningPortal/Completed/{page=1:int}")]
        public IActionResult Completed(
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = BaseSearchablePageViewModel.Descending,
            int page = 1
        )
        {
            sortBy ??= CourseSortByOptions.CompletedDate.PropertyName;

            var completedCourses = courseDataService.GetCompletedCourses(User.GetCandidateIdKnownNotNull());
            var bannerText = GetBannerText();
            var model = new CompletedPageViewModel(
                completedCourses,
                config,
                searchString,
                sortBy,
                sortDirection,
                bannerText,
                page
            );
            return View("Completed/Completed", model);
        }

        public IActionResult AllCompletedItems()
        {
            var completedCourses = courseDataService.GetCompletedCourses(User.GetCandidateIdKnownNotNull());
            var model = new AllCompletedItemsPageViewModel(completedCourses, config);
            return View("Completed/AllCompletedItems", model);
        }
    }
}
