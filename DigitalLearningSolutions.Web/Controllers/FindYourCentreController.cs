namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.FindYourCentre;
    using Microsoft.AspNetCore.Mvc;

    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    [SetSelectedTab(nameof(NavMenuTab.FindYourCentre))]
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
