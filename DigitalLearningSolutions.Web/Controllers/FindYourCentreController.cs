namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.FindYourCentre;
    using Microsoft.AspNetCore.Mvc;

    [SetApplicationTypeAndSelectedTab(nameof(ApplicationType.Main), nameof(Tab.FindYourCentre))]
    public class FindYourCentreController : Controller
    {
        [RedirectDelegateOnlyToLearningPortal]
        public IActionResult Index(string? centreId)
        {
            var model = centreId == null ? new FindYourCentreViewModel() : new FindYourCentreViewModel(centreId);
            return View(model);
        }
    }
}
