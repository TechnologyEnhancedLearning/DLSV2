namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Available;
    using Microsoft.AspNetCore.Mvc;

    public partial class LearningPortalController
    {
        [Route("/LearningPortal/Available/{page=1:int}")]
        public IActionResult Available(
            string? searchString = null,
            string sortBy = SortByOptionTexts.CourseName,
            string sortDirection = BaseCoursePageViewModel.AscendingText,
            int page = 1
        )
        {
            var availableCourses = courseService.GetAvailableCourses(GetCandidateId(), GetCentreId());
            var bannerText = GetBannerText();
            var model = new AvailablePageViewModel(
                availableCourses,
                config,
                searchString,
                sortBy,
                sortDirection,
                bannerText,
                page
            );
            return View("Available/Available", model);
        }
    }
}
