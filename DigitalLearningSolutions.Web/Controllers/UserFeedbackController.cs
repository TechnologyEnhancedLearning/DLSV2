namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Threading.Tasks;
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

        //--------------------------------------------------
        // Step Zero

        [HttpGet]
        public IActionResult StartUserFeedbackSession(UserFeedbackViewModel userFeedbackViewModel)
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            var userFeedbackSessionData = new UserFeedbackTempData()
            {
                UserID = userFeedbackViewModel.UserId,
                SourceUrl = userFeedbackViewModel.SourceUrl,
                TaskAchieved = null,
                TaskAttempted = null,
                FeedbackText = null,
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

        //--------------------------------------------------
        // Step One
        
        [HttpGet]
        public IActionResult UserFeedbackTaskAchieved(UserFeedbackViewModel userFeedbackViewModel)
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            return View("UserFeedbackTaskAchieved", userFeedbackViewModel);
        }

        [HttpPost]
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

        //--------------------------------------------------
        // Step Two

        [HttpGet]
        public IActionResult UserFeedbackTaskAttempted(UserFeedbackViewModel userFeedbackViewModel)
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            return View("UserFeedbackTaskAttempted", userFeedbackViewModel);
        }

        [HttpPost]
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

        //--------------------------------------------------
        // Step Three

        [HttpGet]
        public IActionResult UserFeedbackTaskDifficulty(UserFeedbackViewModel userFeedbackViewModel)
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            return View("UserFeedbackTaskDifficulty", userFeedbackViewModel);
        }

        [HttpPost]
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

        //--------------------------------------------------
        // Step Four

        public IActionResult UserFeedbackSave(UserFeedbackViewModel userFeedbackViewModel)
        {
            var data = multiPageFormService.GetMultiPageFormData<UserFeedbackTempData>(
                MultiPageFormDataFeature.AddUserFeedback,
                TempData
            ).GetAwaiter().GetResult();

            using var transaction = new TransactionScope();

            _userFeedbackDataService.SaveUserFeedback(
                data.UserID,
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

        //--------------------------------------------------
        // Guest feedback

        [HttpGet]
        [Route("/GuestFeedbackComplete")]
        public IActionResult GuestFeedbackComplete()
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            var userFeedbackModel = new UserFeedbackViewModel();

            return View("GuestFeedbackComplete", userFeedbackModel);
        }

        [HttpPost]
        [Route("/GuestFeedbackComplete")]
        public IActionResult GuestFeedbackComplete(UserFeedbackViewModel userFeedbackViewModel)
        {

            return View("GuestFeedbackComplete", userFeedbackViewModel);
        }

        //--------------------------------------------------
        // Return url

        [HttpPost]
        public IActionResult UserFeedbackReturnToUrl(string sourceUrl)
        {
            return Redirect(sourceUrl);
        }
    }
}
