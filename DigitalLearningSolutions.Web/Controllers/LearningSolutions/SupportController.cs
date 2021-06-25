namespace DigitalLearningSolutions.Web.Controllers.LearningSolutions
{
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.LearningSolutions;
    using Microsoft.AspNetCore.Mvc;

    public class SupportController : Controller
    {
        [Route("/{application}/Support")]
        public IActionResult Index(ApplicationType application)
        {
            if (ApplicationType.TrackingSystem == application ||
                ApplicationType.Frameworks == application)
            {
                return View(
                    "/Views/LearningSolutions/Support.cshtml",
                    new SupportViewModel(application, SupportPage.Support)
                );
            }

            return NotFound();
        }
    }
}
