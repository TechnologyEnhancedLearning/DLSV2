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

        public IActionResult Index()
        {
            var headlineFigures = headlineFiguresService.GetHeadlineFigures();
            var model = new IndexViewModel(headlineFigures);
            return View(model);
        }
    }
}
