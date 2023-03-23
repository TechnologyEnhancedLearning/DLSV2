namespace DigitalLearningSolutions.Web.Controllers.FeedbackController
{
    using System.Threading.Tasks;
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
            var feedbackModel = new FeedbackViewModel();
            
            return View("Feedback", feedbackModel);
        }
    }
}
