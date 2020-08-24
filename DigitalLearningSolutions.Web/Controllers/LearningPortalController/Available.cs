namespace DigitalLearningSolutions.Web.Controllers.LearningPortalController
{
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal.Available;
    using Microsoft.AspNetCore.Mvc;

    public partial class LearningPortalController
    {
        public IActionResult Available()
        {
            var availableCourses = courseService.GetAvailableCourses(GetCandidateId(), GetCentreId());
            var bannerText = GetBannerText();
            var model = new AvailableViewModel(availableCourses, bannerText);
            return View(model);
        }
    }
}
