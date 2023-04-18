namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Feedback;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Feedback;
    using GDS.MultiPageFormData;
    using GDS.MultiPageFormData.Enums;
    using Microsoft.AspNetCore.Mvc;

    public class FeedbackController : Controller
    {
        private readonly IUserFeedbackDataService _feedbackDataService;
        //private FeedbackViewModel _feedbackViewModel;
        private readonly IMultiPageFormService multiPageFormService;

        public FeedbackController(
            IUserFeedbackDataService feedbackDataService,
            IMultiPageFormService multiPageFormService
        )
        {
            this._feedbackDataService = feedbackDataService;
            this.multiPageFormService = multiPageFormService;
        }

        [Route("/Index")]
        public async Task<IActionResult> Index(string sourceUrl)
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayFeedbackBar] = true;

            FeedbackViewModel feedbackViewModel = new()
            {
                UserId = User.GetUserId(),
                SourceUrl = sourceUrl,
            };

            if (feedbackViewModel.UserId == null || feedbackViewModel.UserId == 0)
            {
                return View("FeedbackGuest", feedbackViewModel);
            }
            else
            {
                return StartLoggedInFeedbackSession(feedbackViewModel);
            }
        }

        public IActionResult StartLoggedInFeedbackSession(FeedbackViewModel feedbackViewModel)
        {
            var feedbackSessionData = new FeedbackSessionData()
            {
                UserID = feedbackViewModel.UserId,
                SourceUrl = feedbackViewModel.SourceUrl
            };

            TempData.Clear();
            multiPageFormService.SetMultiPageFormData(
                feedbackSessionData,
                MultiPageFormDataFeature.AddUserFeedback,
                TempData
            );
            //return RedirectToAction("VerificationPickSupervisor", new { selfAssessmentId });
            return RedirectToAction("FeedbackLoggedInStepOne", feedbackViewModel);
        }

        public async Task<IActionResult> FeedbackLoggedInStepOne(string sourceUrl, FeedbackViewModel feedbackModel)
        {
            var session = multiPageFormService.GetMultiPageFormData<FeedbackSessionData>(
                MultiPageFormDataFeature.AddUserFeedback,
                TempData
            ).GetAwaiter().GetResult();

            return View("FeedbackLoggedIn_One", feedbackModel);
        }

        [HttpPost]
        public IActionResult FeedbackLoggedInStepOneSave(FeedbackViewModel feedbackViewModel)
        {
            TempData.Clear();
            multiPageFormService.SetMultiPageFormData(
                feedbackViewModel,
                MultiPageFormDataFeature.SearchInSelfAssessmentOverviewGroups,
                TempData
            );
            //return RedirectToAction("", model);
            return View("FeedbackLoggedIn_Two", feedbackViewModel);
        }



        [HttpPost]
        public IActionResult FeedbackLoggedInStepTwoSave(FeedbackViewModel feedbackViewModel)
        {
            //    TempData.Clear();
            //    multiPageFormService.SetMultiPageFormData(
            //        model,
            //        MultiPageFormDataFeature.SearchInSelfAssessmentOverviewGroups,
            //        TempData
            //    );
            //return RedirectToAction("", model);
            return View("FeedbackLoggedIn_Three", feedbackViewModel);
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

        [Route("/FeedbackComplete")]
        public async Task<IActionResult> FeedbackComplete(
        )
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayFeedbackBar] = true;

            var feedbackModel = new FeedbackViewModel();

            return View("FeedbackComplete", feedbackModel);
        }
    }
}
