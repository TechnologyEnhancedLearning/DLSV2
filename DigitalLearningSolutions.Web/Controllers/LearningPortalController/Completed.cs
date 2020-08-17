namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using Microsoft.AspNetCore.Mvc;

    public partial class LearningPortalController
    {
        public IActionResult Completed()
        {
            var completedCourses = courseService.GetCompletedCourses(GetCandidateId());
            var bannerText = GetBannerText();
            var model = new CompletedViewModel(
                completedCourses,
                config,
                bannerText
            );
            return View("Completed/Completed", model);
        }
    }
}
