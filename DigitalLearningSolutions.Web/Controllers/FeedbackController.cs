namespace DigitalLearningSolutions.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Feedback;
    using Microsoft.AspNetCore.Mvc;

    public class FeedbackController : Controller
    {
        private readonly IUserFeedbackDataService feedbackDataService;

        public FeedbackController(
            IUserFeedbackDataService feedbackDataService
            )
        {
            this.feedbackDataService = feedbackDataService;
        }

        [Route("/Index")]
        public async Task<IActionResult> Index(
        )
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayFeedbackBar] = false;

            var feedbackModel = new FeedbackViewModel();

            return View("Feedback", feedbackModel);
        }

        [Route("/FeedbackGuest_One")]
        public async Task<IActionResult> FeedbackGuest_One(
        )
        {
            var feedbackModel = new FeedbackViewModel();

            return View("FeedbackGuest_One", feedbackModel);
        }

        [Route("/FeedbackGuest_Two")]
        public async Task<IActionResult> FeedbackGuest_Two(
        )
        {
            var feedbackModel = new FeedbackViewModel();

            return View("FeedbackGuest_Two", feedbackModel);
        }

        public async Task<IActionResult> SaveFeedback(
            int? userID,
            string feedbackText,
            string sourcePageUrl,
            bool? taskAchieved,
            string? taskAttempted,
            int? taskRating
        )
        {
            this.feedbackDataService.SaveUserFeedback(
                 userID,
                 sourcePageUrl,
                 taskAchieved,
                 taskAttempted,
                 feedbackText,
                 taskRating
             );

            //TODO: Probs need error handling here with associated user error message.
            return Ok();
        }
    }
}
