namespace DigitalLearningSolutions.Web.Controllers.LearningSolutions
{
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.LearningSolutions;
    using Microsoft.AspNetCore.Mvc;

    public class SupportController : Controller
    {
        [Route("/{applicationBaseUrl}/Support")]
        public IActionResult Index(string applicationBaseUrl)
        {
            if (applicationBaseUrl == ApplicationType.TrackingSystem.ApplicationBaseUrl ||
                applicationBaseUrl == ApplicationType.Frameworks.ApplicationBaseUrl)
            {
                return View(
                    "/Views/LearningSolutions/Support.cshtml",
                    new SupportViewModel(applicationBaseUrl, SupportPage.Support)
                );
            }

            return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
        }
    }
}
