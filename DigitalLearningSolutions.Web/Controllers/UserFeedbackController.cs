namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Transactions;
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
        public IActionResult Index(string sourceUrl)
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            UserFeedbackViewModel userFeedbackViewModel = new()
            {
                UserId = User.GetUserId(),
                SourceUrl = sourceUrl,
            };

            if (userFeedbackViewModel.UserId == null || userFeedbackViewModel.UserId == 0)
            {
                return GuestFeedbackStart(userFeedbackViewModel);
            }
            else
            {
                return StartUserFeedbackSession(userFeedbackViewModel);
            }
        }

        [HttpGet]
        [Route("/StartUserFeedbackSession")]
        public IActionResult StartUserFeedbackSession(UserFeedbackViewModel userFeedbackViewModel)
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            var userFeedbackSessionData = new UserFeedbackTempData()
            {
                UserId = userFeedbackViewModel.UserId,
                SourceUrl = userFeedbackViewModel.SourceUrl,
                TaskAchieved = null,
                TaskAttempted = string.Empty,
                FeedbackText = string.Empty,
                TaskRating = null,
            };

            TempData.Clear();
            multiPageFormService.SetMultiPageFormData(
                userFeedbackSessionData,
                MultiPageFormDataFeature.AddUserFeedback,
                TempData
            );
            return RedirectToAction("UserFeedbackTaskAchieved", userFeedbackViewModel);
        }

        [HttpGet]
        [Route("/UserFeedbackTaskAchieved")]
        public IActionResult UserFeedbackTaskAchieved(UserFeedbackViewModel userFeedbackViewModel)
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            return View("UserFeedbackTaskAchieved", userFeedbackViewModel);
        }

        [HttpPost]
        [Route("/UserFeedbackTaskAchievedSet")]
        public IActionResult UserFeedbackTaskAchievedSet(UserFeedbackViewModel userFeedbackViewModel)
        {
            var data = multiPageFormService.GetMultiPageFormData<UserFeedbackTempData>(
                MultiPageFormDataFeature.AddUserFeedback,
                TempData
            ).GetAwaiter().GetResult();

            data.TaskAchieved = userFeedbackViewModel.TaskAchieved;

            multiPageFormService.SetMultiPageFormData(
                data,
                MultiPageFormDataFeature.AddUserFeedback,
                TempData
            );

            return RedirectToAction("UserFeedbackTaskAttempted", userFeedbackViewModel);
        }

        [HttpGet]
        [Route("/UserFeedbackTaskAttempted")]
        public IActionResult UserFeedbackTaskAttempted(UserFeedbackViewModel userFeedbackViewModel)
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            return View("UserFeedbackTaskAttempted", userFeedbackViewModel);
        }

        [HttpPost]
        [Route("/UserFeedbackTaskAttemptedSet")]
        public IActionResult UserFeedbackTaskAttemptedSet(UserFeedbackViewModel userFeedbackViewModel)
        {
            var data = multiPageFormService.GetMultiPageFormData<UserFeedbackTempData>(
                MultiPageFormDataFeature.AddUserFeedback,
                TempData
            ).GetAwaiter().GetResult();

            data.TaskAttempted = userFeedbackViewModel.TaskAttempted;
            data.FeedbackText = userFeedbackViewModel.FeedbackText;

            multiPageFormService.SetMultiPageFormData(
                data,
                MultiPageFormDataFeature.AddUserFeedback,
                TempData
            );

            return RedirectToAction("UserFeedbackTaskDifficulty", userFeedbackViewModel);
        }

        [HttpGet]
        [Route("/UserFeedbackTaskDifficulty")]
        public IActionResult UserFeedbackTaskDifficulty(UserFeedbackViewModel userFeedbackViewModel)
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            return View("UserFeedbackTaskDifficulty", userFeedbackViewModel);
        }

        [HttpPost]
        [Route("/UserFeedbackTaskDifficultySet")]
        public IActionResult UserFeedbackTaskDifficultySet(UserFeedbackViewModel userFeedbackViewModel)
        {
            var data = multiPageFormService.GetMultiPageFormData<UserFeedbackTempData>(
                MultiPageFormDataFeature.AddUserFeedback,
                TempData
            ).GetAwaiter().GetResult();

            data.TaskRating = userFeedbackViewModel.TaskRating;

            multiPageFormService.SetMultiPageFormData(
                data,
                MultiPageFormDataFeature.AddUserFeedback,
                TempData
            );

            return RedirectToAction("UserFeedbackSave", userFeedbackViewModel);
        }

        public IActionResult UserFeedbackSave(UserFeedbackViewModel userFeedbackViewModel)
        {
            var data = multiPageFormService.GetMultiPageFormData<UserFeedbackTempData>(
                MultiPageFormDataFeature.AddUserFeedback,
                TempData
            ).GetAwaiter().GetResult();

            using var transaction = new TransactionScope();

            _userFeedbackDataService.SaveUserFeedback(
                data.UserId,
                data.SourceUrl,
                data.TaskAchieved,
                data.TaskAttempted,
                data.FeedbackText,
                data.TaskRating
            );

            multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.AddUserFeedback, TempData);

            transaction.Complete();

            TempData.Clear();

            userFeedbackViewModel.SourceUrl = data.SourceUrl;

            return RedirectToAction("UserFeedbackComplete", userFeedbackViewModel);
        }

        [HttpGet]
        [Route("/UserFeedbackComplete")]
        public IActionResult UserFeedbackComplete(UserFeedbackViewModel userFeedbackViewModel)
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            return View("UserFeedbackComplete", userFeedbackViewModel);
        }

        [HttpGet]
        [Route("/GuestFeedbackStart")]
        public IActionResult GuestFeedbackStart(UserFeedbackViewModel userFeedbackViewModel)
        {
            //var userFeedbackModel = new UserFeedbackViewModel();

            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            return View("GuestFeedbackStart", userFeedbackViewModel);
        }

        [HttpGet]
        [Route("/GuestFeedbackComplete")]
        public IActionResult GuestFeedbackComplete()
        {
            var userFeedbackModel = new UserFeedbackViewModel();

            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            return View("GuestFeedbackComplete", userFeedbackModel);
        }

        [HttpPost]
        [Route("/GuestFeedbackComplete")]
        public IActionResult GuestFeedbackComplete(UserFeedbackViewModel userFeedbackViewModel)
        {

            _userFeedbackDataService.SaveUserFeedback(
                null,
                userFeedbackViewModel.SourceUrl,
                null,
                userFeedbackViewModel.TaskAttempted,
                userFeedbackViewModel.FeedbackText,
                null
            );

            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            return View("GuestFeedbackComplete", userFeedbackViewModel);
        }

        [HttpPost]
        [Route("/UserFeedbackReturnToUrl")]
        public IActionResult UserFeedbackReturnToUrl(string sourceUrl)
        {
            return Redirect(sourceUrl);
        }
    }
}
