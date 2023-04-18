namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.UserFeedback;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.UserFeedback;
    using GDS.MultiPageFormData;
    using GDS.MultiPageFormData.Enums;
    using Microsoft.AspNetCore.Mvc;

    public class UserFeedbackController : Controller
    {
        private readonly IUserFeedbackDataService _userFeedbackDataService;
        private readonly IMultiPageFormService multiPageFormService;

        public UserFeedbackController(
            IUserFeedbackDataService userFeedbackDataService,
            IMultiPageFormService multiPageFormService
        )
        {
            this._userFeedbackDataService = userFeedbackDataService;
            this.multiPageFormService = multiPageFormService;
        }

        [Route("/Index")]
        public Task<IActionResult> Index(string sourceUrl)
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            UserFeedbackViewModel userFeedbackViewModel = new()
            {
                UserId = User.GetUserId(),
                SourceUrl = sourceUrl,
            };

            if (userFeedbackViewModel.UserId == null || userFeedbackViewModel.UserId == 0)
            {
                return Task.FromResult<IActionResult>(View("GuestFeedbackStart", userFeedbackViewModel));
            }
            else
            {
                return Task.FromResult(StartUserFeedbackSession(userFeedbackViewModel));
            }
        }

        public IActionResult StartUserFeedbackSession(UserFeedbackViewModel userFeedbackViewModel)
        {
            var userFeedbackSessionData = new UserFeedbackSessionData()
            {
                UserID = userFeedbackViewModel.UserId,
                SourceUrl = userFeedbackViewModel.SourceUrl
            };

            TempData.Clear();
            multiPageFormService.SetMultiPageFormData(
                userFeedbackSessionData,
                MultiPageFormDataFeature.AddUserFeedback,
                TempData
            );
            //return RedirectToAction("VerificationPickSupervisor", new { selfAssessmentId });
            return RedirectToAction("UserFeedbackTaskAchieved", userFeedbackViewModel);
        }

        public Task<IActionResult> UserFeedbackTaskAchieved(string sourceUrl, UserFeedbackViewModel userFeedbackModel)
        {
            var session = multiPageFormService.GetMultiPageFormData<UserFeedbackSessionData>(
                MultiPageFormDataFeature.AddUserFeedback,
                TempData
            ).GetAwaiter().GetResult();

            return Task.FromResult<IActionResult>(View("UserFeedbackTaskAchieved", userFeedbackModel));
        }

        [HttpPost]
        public IActionResult UserFeedbackTaskAchievedSave(UserFeedbackViewModel userFeedbackViewModel)
        {
            TempData.Clear();
            multiPageFormService.SetMultiPageFormData(
                userFeedbackViewModel,
                MultiPageFormDataFeature.SearchInSelfAssessmentOverviewGroups,
                TempData
            );
            //return RedirectToAction("", model);
            return View("UserFeedbackTaskAttempted", userFeedbackViewModel);
        }



        [HttpPost]
        public IActionResult UserFeedbackTaskAttemptedSave(UserFeedbackViewModel userFeedbackViewModel)
        {
            //    TempData.Clear();
            //    multiPageFormService.SetMultiPageFormData(
            //        model,
            //        MultiPageFormDataFeature.SearchInSelfAssessmentOverviewGroups,
            //        TempData
            //    );
            //return RedirectToAction("", model);
            return View("UserFeedbackTaskDifficulty", userFeedbackViewModel);
        }

        public async Task<IActionResult> SaveUserFeedback(
            string userFeedbackText,
            bool? taskAchieved,
            string? taskAttempted,
            int? taskRating,
            string sourceUrl
        )
        {
            var userId = User.GetUserId();

            _userFeedbackDataService.SaveUserFeedback(
                userId,
                sourceUrl,
                taskAchieved,
                taskAttempted,
                userFeedbackText,
                taskRating
            );

            //TODO: Probs need error handling here with associated user error message.
            return RedirectToAction("UserFeedbackComplete");
        }

        [Route("/UserFeedbackComplete")]
        public Task<IActionResult> UserFeedbackComplete()
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            var userFeedbackModel = new UserFeedbackViewModel();

            return Task.FromResult<IActionResult>(View( "UserFeedbackComplete", userFeedbackModel));
        }

        [Route("/GuestFeedbackComplete")]
        public Task<IActionResult> GuestFeedbackComplete()
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            var userFeedbackModel = new UserFeedbackViewModel();

            return Task.FromResult<IActionResult>(View("GuestFeedbackComplete", userFeedbackModel));
        }
    }
}
