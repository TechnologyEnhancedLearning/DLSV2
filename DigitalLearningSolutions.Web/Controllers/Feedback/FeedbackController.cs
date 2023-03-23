namespace DigitalLearningSolutions.Web.Controllers.FeedbackController
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Feedback;
    using Microsoft.AspNetCore.Mvc;

    public class FeedbackController : Controller
    {
        public FeedbackController(
        )
        {
        }

        [Route("Feedback")]
        public async Task<IActionResult> Index(
        )
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayFeedbackBar] = false;

            var feedbackModel = new FeedbackViewModel();
            
            return View("Feedback", feedbackModel);
        }

        public async Task<IActionResult> FeedbackGuest_One(
        )
        {
            var feedbackModel = new FeedbackViewModel();

            return View("FeedbackGuest_One", feedbackModel);
        }
        public async Task<IActionResult> FeedbackGuest_Two(
        )
        {
            var feedbackModel = new FeedbackViewModel();

            return View("FeedbackGuest_Two", feedbackModel);
        }
    }
}
