namespace DigitalLearningSolutions.Web.Controllers.Support
{
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Support;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class SupportController : Controller
    {
        [Route("/{application}/Support")]
        [Authorize(Policy = CustomPolicies.UserCentreAdminOrFrameworksAdmin)]
        public IActionResult Index(ApplicationType application)
        {
            if (!ApplicationType.TrackingSystem.Equals(application) &&
                !ApplicationType.Frameworks.Equals(application))
            {
                return NotFound();
            }

            if (ApplicationType.TrackingSystem.Equals(application) && User.HasCentreAdminPermissions() ||
                ApplicationType.Frameworks.Equals(application) && User.HasFrameworksAdminPermissions())
            {
                var model = new SupportViewModel(application, SupportPage.Support);
                return View("Support", model);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
