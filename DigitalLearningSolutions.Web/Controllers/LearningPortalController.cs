namespace DigitalLearningSolutions.Web.Controllers
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ControllerHelpers;
    using DigitalLearningSolutions.Web.ViewModels.LearningPortal;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    [Authorize(Policy = CustomPolicies.UserOnly)]
    public class LearningPortalController : Controller
    {
        private readonly ICourseService courseService;
        private readonly IUnlockService unlockService;
        private readonly IConfigService configService;
        private readonly ILogger<LearningPortalController> logger;
        private readonly IConfiguration config;

        public LearningPortalController(
            ICourseService courseService,
            IUnlockService unlockService,
            IConfigService configService,
            ILogger<LearningPortalController> logger,
            IConfiguration config)
        {
            this.courseService = courseService;
            this.unlockService = unlockService;
            this.configService = configService;
            this.logger = logger;
            this.config = config;
        }

        public IActionResult Current(string sortBy = "Course Name", string sortDirection = "Ascending")
        {
            logger.LogInformation("Getting current courses");
            var currentCourses = courseService.GetCurrentCourses(GetCandidateId());
            var model = new CurrentViewModel(currentCourses, config, sortBy, sortDirection);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/LearningPortal/Current/CompleteBy/{id:int}")]
        public IActionResult SetCompleteByDate(int id, int day, int month, int year, int progressId)
        {
            if (day == 0 && month == 0 && year == 0)
            {
                courseService.SetCompleteByDate(progressId, GetCandidateId(), null);
                return RedirectToAction("Current");
            }

            var validationResult = DateValidator.ValidateDate(day, month, year);
            if (!validationResult.DateValid)
            {
                return RedirectToAction("SetCompleteByDate", new { id, day, month, year });
            }

            var completeByDate = new DateTime(year, month, day);
            courseService.SetCompleteByDate(progressId, GetCandidateId(), completeByDate);
            return RedirectToAction("Current");
        }

        [Route("/LearningPortal/Current/CompleteBy/{id:int}")]
        public IActionResult SetCompleteByDate(int id, int? day, int? month, int? year)
        {
            var currentCourses = courseService.GetCurrentCourses(GetCandidateId());
            var model = currentCourses
                .Where(c => c.CustomisationID == id)
                .Select(c => new CurrentViewModel.CurrentCourseViewModel(c, config))
                .First();
            if (model.CompleteByDate != null && !model.SelfEnrolled)
            {
                return StatusCode(403);
            }

            if (day != null && month != null && year != null)
            {
                model.CompleteByValidationResult = DateValidator.ValidateDate(day.Value, month.Value, year.Value);
            }

            return View(model);
        }

        [Route("/LearningPortal/Current/Remove/{id:int}")]
        public IActionResult RemoveCurrentCourseConfirmation(int id)
        {
            var currentCourses = courseService.GetCurrentCourses(GetCandidateId());
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
            courseService.RemoveCurrentCourse(progressId, GetCandidateId());
            return RedirectToAction("Current");
        }

        [Route("/LearningPortal/Current/RequestUnlock/{progressId:int}")]
        public IActionResult RequestUnlock(int progressId)
        {
            unlockService.SendUnlockRequest(progressId);

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
            var accessibilityText = configService.GetConfigValue(ConfigService.AccessibilityHelpText);
            if (accessibilityText == null)
            {
                logger.LogError("Accessibility text from Config table is null");
                return StatusCode(500);
            }
            var model = new AccessibilityHelpViewModel(accessibilityText);
            return View(model);
        }

        public IActionResult Terms()
        {
            var termsText = configService.GetConfigValue(ConfigService.TermsText);
            if (termsText == null)
            {
                logger.LogError("Terms text from Config table is null");
                return StatusCode(500);
            }
            var model = new TermsViewModel(termsText);
            return View(model);
        }

        public IActionResult Error()
        {
            return View("Error/UnknownError");
        }

        [Route("/LearningPortal/StatusCode/{code:int}")]
        public new IActionResult StatusCode(int code)
        {
            return code switch
            {
                404 => View("Error/PageNotFound"),
                403 => View("Error/Forbidden"),
                _ => View("Error/UnknownError")
            };
        }

        private int GetCandidateId()
        {
            var id = User.Claims.First(claim => claim.Type == CustomClaimTypes.LearnCandidateId);
            return Convert.ToInt32(id.Value);
        }
    }
}
