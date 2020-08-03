namespace DigitalLearningSolutions.Web.Controllers
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.Models;
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
        private readonly IUnlockService unlockService;
        private readonly ILogger<LearningPortalController> logger;
        private readonly IConfiguration config;

        public LearningPortalController(
            ICourseService courseService,
            IUnlockService unlockService,
            ILogger<LearningPortalController> logger,
            IConfiguration config)
        {
            this.courseService = courseService;
            this.unlockService = unlockService;
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
        public IActionResult SetCompleteByDate(int id, int day, int month, int year, int progressId)
        {
            if (day == 0 && month == 0 && year == 0)
            {
                courseService.SetCompleteByDate(progressId, candidateId, null);
                return RedirectToAction("Current");
            }

            try
            {
                var completeByDate = new DateTime(year, month, day);
                courseService.SetCompleteByDate(progressId, candidateId, completeByDate);
            }
            catch (ArgumentOutOfRangeException)
            {
                return RedirectToAction("SetCompleteByDate", new { id, errorMessage = "Please enter a valid date" });
            }

            return RedirectToAction("Current");
        }

        [Route("/LearningPortal/Current/CompleteBy/{id:int}")]
        public IActionResult SetCompleteByDate(int id, string? errorMessage)
        {
            var currentCourses = courseService.GetCurrentCourses(candidateId);
            var model = currentCourses
                .Where(c => c.CustomisationID == id)
                .Select(c => new CurrentViewModel.CurrentCourseViewModel(c, config))
                .First();

            ViewData["errorMessage"] = errorMessage;
            return View(model);
        }

        [Route("/LearningPortal/Current/Remove/{id:int}")]
        public IActionResult RemoveCurrentCourseConfirmation(int id)
        {
            var currentCourses = courseService.GetCurrentCourses(candidateId);
            var model = currentCourses
                .Where(c => c.CustomisationID == id)
                .Select(c => new CurrentViewModel.CurrentCourseViewModel(c, config))
                .First();

            return View(model);
        }

        [Route("/LearningPortal/Current/Remove/{progressId:int}")]
        [HttpPost]
        public IActionResult RemoveCurrentCourse(int progressId)
        {
            courseService.RemoveCurrentCourse(progressId, candidateId);
            return RedirectToAction("Current");
        }

        [Route("/LearningPortal/Current/RequestUnlock/{progressId:int}")]
        public IActionResult RequestUnlock(int progressId)
        {
            try
            {
                unlockService.SendUnlockRequest(progressId);
            }
            catch (UnlockDataMissingException)
            {
                logger.LogError("Encountered error while sending email. Unlock data was null");
                return StatusCode(500);
            }

            return View("UnlockCurrentCourse");
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
