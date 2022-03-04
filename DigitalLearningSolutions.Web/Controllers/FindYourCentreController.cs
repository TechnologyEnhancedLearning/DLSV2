namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.FindYourCentre;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    [SetSelectedTab(nameof(NavMenuTab.FindYourCentre))]
    public class FindYourCentreController : Controller
    {
        private readonly IConfiguration configuration;

        public FindYourCentreController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [RedirectDelegateOnlyToLearningPortal]
        public IActionResult Index(string? centreId)
        {
            var model = centreId == null
                ? new FindYourCentreViewModel(configuration)
                : new FindYourCentreViewModel(centreId, configuration);

            return View(model);
        }
    }
}
