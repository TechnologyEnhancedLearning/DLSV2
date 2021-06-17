namespace DigitalLearningSolutions.Web.Controllers.LearningSolutions
{
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.LearningSolutions;
    using Microsoft.AspNetCore.Mvc;

    public class SupportController : Controller
    {
        [Route("/{applicationName}/Support")]
        public IActionResult Index(string applicationName)
        {
            if (applicationName == "TrackingSystem" || applicationName == "Frameworks")
            {
                return View("/Views/LearningSolutions/Support.cshtml" ,new SupportViewModel(applicationName, SupportPage.Support));
            }
            
            return RedirectToAction("StatusCode", "LearningSolutions", new { code = 404 });
        }
    }
}
