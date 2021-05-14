namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.ViewModels.FindYourCentre;
    using Microsoft.AspNetCore.Mvc;

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
