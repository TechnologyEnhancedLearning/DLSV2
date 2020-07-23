namespace DigitalLearningSolutions.Web.Controllers
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    public class LearningPortalController : Controller
    {
        private readonly ICourseService courseService;
        private readonly ILogger<LearningPortalController> logger;

        public LearningPortalController(ICourseService courseService, ILogger<LearningPortalController> logger)
        {
            this.courseService = courseService;
            this.logger = logger;
        }

        public IActionResult Current()
        {
            logger.LogInformation("Getting current courses");
            var currentCourses = courseService.GetCurrentCourses();
            var model = new CurrentViewModel(currentCourses);
            return View(model);
        }

        public IActionResult Completed()
        {
            logger.LogInformation("Getting completed courses");
            var completedCourses = courseService.GetCompletedCourses();
            var model = new CompletedViewModel(completedCourses);
            return View(model);
        }

        public IActionResult Available()
        {
            logger.LogInformation("Getting available courses");
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

        public IActionResult Error()
        {
            return View("Error/UnknownError");
        }

        [Route("/LearningPortal/StatusCode/{code:int}")]
        public IActionResult StatusCode(int code)
        {
            if (code == 404)
            {
                return View("Error/PageNotFound");
            }
            else
            {
                return View("Error/UnknownError");
            }
        }
    }
}
