namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using Microsoft.AspNetCore.Mvc;

    public class LearningPortalController : Controller
    {
        private readonly IHeadlineFiguresService headlineFiguresService;

        public LearningPortalController(IHeadlineFiguresService headlineFiguresService)
        {
            this.headlineFiguresService = headlineFiguresService;
        }

        public IActionResult Current()
        {
            var headlineFigures = headlineFiguresService.GetHeadlineFigures();
            var model = new CurrentViewModel(headlineFigures);
            return View(model);
        }
    }
}
