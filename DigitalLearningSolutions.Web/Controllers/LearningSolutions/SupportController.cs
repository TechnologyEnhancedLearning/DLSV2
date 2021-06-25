namespace DigitalLearningSolutions.Web.Controllers.LearningSolutions
{
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.LearningSolutions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class SupportController : Controller
    {
        [Route("/{application}/Support")]
        [Authorize(Policy = CustomPolicies.UserCentreAdminOrFrameworksAdmin)]
        public IActionResult Index(ApplicationType application)
        {
            if (ApplicationType.TrackingSystem.Equals(application) ||
                ApplicationType.Frameworks.Equals(application))
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
