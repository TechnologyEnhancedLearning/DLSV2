namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.FindYourCentre;
    using Microsoft.AspNetCore.Mvc;

    [RedirectDelegateOnlyToLearningPortal]
    [SetApplicationTypeAndSelectedTab(nameof(ApplicationType.Main), nameof(Tab.FindYourCentre))]
    public class FindYourCentreController : Controller
    {
        public IActionResult Index(string? centreId)
        {
            var model = centreId == null ? new FindYourCentreViewModel() : new FindYourCentreViewModel(centreId);
            return View(model);
        }
    }
}
