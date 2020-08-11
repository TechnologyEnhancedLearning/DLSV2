namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using Microsoft.AspNetCore.Mvc;

    public partial class LearningPortalController
    {
        public IActionResult Completed()
        {
            var completedCourses = courseService.GetCompletedCourses();
            var bannerText = GetBannerText();
            var model = new CompletedViewModel(completedCourses, bannerText);
            return View(model);
        }
    }
}
