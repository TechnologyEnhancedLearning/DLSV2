namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.LearningContent;
    using Microsoft.AspNetCore.Mvc;

    [RedirectDelegateOnlyToLearningPortal]
    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    [SetSelectedTab(nameof(NavMenuTab.Welcome))]
    public class LearningContentController : Controller
    {
        private readonly IBrandsService brandsService;
        private readonly ITutorialService tutorialService;

        public LearningContentController(IBrandsService brandsService, ITutorialService tutorialService)
        {
            this.brandsService = brandsService;
            this.tutorialService = tutorialService;
        }

        [Route("Home/LearningContent/{brandId:int}")]
        public IActionResult Index(int brandId)
        {
            var brand = brandsService.GetPublicBrandById(brandId);
            if (brand == null)
            {
                return NotFound();
            }

            var tutorials = tutorialService.GetPublicTutorialSummariesForBrand(brandId);

            var model = new LearningContentViewModel(brand, tutorials);

            return View(model);
        }
    }
}
