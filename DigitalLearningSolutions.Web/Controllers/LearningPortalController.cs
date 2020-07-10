namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using Microsoft.AspNetCore.Mvc;

    public class LearningPortalController : Controller
    {
        private readonly ICourseService courseService;

        public LearningPortalController(ICourseService courseService)
        {
            this.courseService = courseService;
        }

        public IActionResult Current()
        {
            var currentCourses = courseService.GetCurrentCourses();
            var model = new CurrentViewModel(currentCourses);
            return View(model);
        }

        public IActionResult Completed()
        {
            var headlineFigures = courseService.GetHeadlineFigures();
            var model = new CompletedViewModel(headlineFigures);
            return View(model);
        }

        public IActionResult Available()
        {
            var headlineFigures = courseService.GetHeadlineFigures();
            var model = new AvailableViewModel(headlineFigures);
            return View(model);
        }
    }
}
