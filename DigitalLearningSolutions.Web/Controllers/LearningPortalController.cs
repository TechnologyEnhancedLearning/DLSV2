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

        public IActionResult Index()
        {
            return RedirectToAction("Current");
        }

        public IActionResult Current()
        {
            var currentCourses = courseService.GetCurrentCourses();
            var model = new CurrentViewModel(currentCourses);
            return View(model);
        }

        public IActionResult Completed()
        {
            var completedCourses = courseService.GetCompletedCourses();
            var model = new CompletedViewModel(completedCourses);
            return View(model);
        }

        public IActionResult Available()
        {
            var availableCourses = courseService.GetAvailableCourses();
            var model = new AvailableViewModel(availableCourses);
            return View(model);
        }

        public IActionResult AccessibilityHelp()
        {
            return View();
        }

        public IActionResult Terms()
        {
            return View();
        }
    }
}
