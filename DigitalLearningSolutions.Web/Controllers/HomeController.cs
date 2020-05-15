namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.ViewModels.Home;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        private readonly IHeadlineFiguresService headlineFiguresService;

        public HomeController(IHeadlineFiguresService headlineFiguresService)
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
