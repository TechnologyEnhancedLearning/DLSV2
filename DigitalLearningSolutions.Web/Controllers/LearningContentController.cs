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

        public LearningContentController(IBrandsService brandsService)
        {
            this.brandsService = brandsService;
        }

        [Route("{brandId:int}")]
        public IActionResult BrandPageDeliveryService(int brandId)
        {
            var brand = brandsService.GetBrandById(brandId);
            if (brand == null)
            {
                return NotFound();
            }

            var model = new LearningContentViewModel(brand);

            return View("Index", model);
        }
    }
}
