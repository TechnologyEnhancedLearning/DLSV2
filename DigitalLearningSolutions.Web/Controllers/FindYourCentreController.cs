namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Services;
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

        private readonly ICentresService centresService;

        public FindYourCentreController(IConfiguration configuration, ICentresService centresService)
        {
            this.configuration = configuration;
            this.centresService = centresService;
        }

        [RedirectDelegateOnlyToLearningPortal]
        public IActionResult Index(string? centreId)
        {
            var centreSummaries = centresService.GetAllCentreSummariesForFindCentre();
            /*var model = centreId == null
                ? new FindYourCentreViewModel(configuration, centreSummaries)
                : new FindYourCentreViewModel(centreId, configuration);*/
            var model = new FindYourCentreViewModel(configuration, centreSummaries);

            return View(model);
        }
    }
}
