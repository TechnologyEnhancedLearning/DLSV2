namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Completed;
    using Microsoft.AspNetCore.Mvc;

    public partial class LearningPortalController
    {
        public IActionResult Completed(string? searchString = null, string sortBy = SortByOptionTexts.CompletedDate, string sortDirection = BaseCoursePageViewModel.DescendingText)
        {
            var completedCourses = courseService.GetCompletedCourses(GetCandidateId());
            var bannerText = GetBannerText();
            var model = new CompletedPageViewModel(
                completedCourses,
                config,
                searchString,
                sortBy,
                sortDirection,
                bannerText
            );
            return View("Completed/Completed", model);
        }
    }
}
