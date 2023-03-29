namespace DigitalLearningSolutions.Web.Controllers
{
    using System;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Feedback;
    using Microsoft.AspNetCore.Mvc;
    using System.Web;

    public class FeedbackController : Controller
    {
        private readonly IUserFeedbackDataService _feedbackDataService;
        private FeedbackViewModel _feedbackViewModel;
        //private string _sourceUrl;

        public FeedbackController(
            IUserFeedbackDataService feedbackDataService
            )
        {
            this._feedbackDataService = feedbackDataService;
        }

        [Route("/Index")]
        public async Task<IActionResult> Index(string sourceUrl)
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayFeedbackBar] = true;

            //this._sourceUrl = sourceUrl;

            _feedbackViewModel = new FeedbackViewModel();

            _feedbackViewModel.SourceUrl = sourceUrl;

            return View("FeedbackGuest", _feedbackViewModel);
        }

        [Route("/FeedbackComplete")]
        public async Task<IActionResult> FeedbackComplete(
        )
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayFeedbackBar] = true;

            var feedbackModel = new FeedbackViewModel();

            return View("FeedbackComplete", feedbackModel);
        }

        public async Task<IActionResult> SaveFeedback(
            string feedbackText,
            bool? taskAchieved,
            string? taskAttempted,
            int? taskRating,
            string sourceUrl
        )
        {
            var userId = User.GetUserId();

            _feedbackDataService.SaveUserFeedback(
                 userId,
                 sourceUrl,
                 taskAchieved,
                 taskAttempted,
                 feedbackText,
                 taskRating
             );

            //TODO: Probs need error handling here with associated user error message.
            return RedirectToAction("FeedbackComplete");
        }
    }
}
