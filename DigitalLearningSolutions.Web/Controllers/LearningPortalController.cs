namespace DigitalLearningSolutions.Web.Controllers
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class LearningPortalController : Controller
    {
        //TODO placeholder candidateId, replace once HEEDLS-4 is implemented
        private readonly int candidateId = 254480;
        private readonly ICourseService courseService;
        private readonly ILogger<LearningPortalController> logger;
        private readonly IConfiguration config;

        public LearningPortalController(
            ICourseService courseService,
            ILogger<LearningPortalController> logger,
            IConfiguration config)
        {
            this.courseService = courseService;
            this.logger = logger;
            this.config = config;
        }

        public IActionResult Current()
        {
            logger.LogInformation("Getting current courses");
            var currentCourses = courseService.GetCurrentCourses(candidateId);
            var model = new CurrentViewModel(currentCourses, config);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/LearningPortal/Current/CompleteBy/{id:int}")]
        public IActionResult SetCompleteByDate(int id, int day, int month, int year)
        {
            // TODO HEEDLS-46: Save new complete by date
            Console.WriteLine(id);
            Console.WriteLine(day);
            Console.WriteLine(month);
            Console.WriteLine(year);


            return RedirectToAction("Current");
        }

        [Route("/LearningPortal/Current/CompleteBy/{id:int}")]
        public IActionResult SetCompleteByDate(int id)
        {
            var currentCourses = courseService.GetCurrentCourses(candidateId);
            var model = currentCourses
                .Where(c => c.CustomisationID == id)
                .Select(c => new CurrentViewModel.CurrentCourseViewModel(c, config))
                .First();
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
        public new IActionResult StatusCode(int code)
        {
            return View(code == 404 ? "Error/PageNotFound" : "Error/UnknownError");
        }
    }
}
