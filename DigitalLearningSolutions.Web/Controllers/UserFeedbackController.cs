namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Collections.Generic;
    using System.Transactions;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.UserFeedback;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.UserFeedback;
    using GDS.MultiPageFormData;
    using GDS.MultiPageFormData.Enums;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    public class UserFeedbackController : Controller
    {
        private readonly IUserFeedbackDataService _userFeedbackDataService;
        private readonly IMultiPageFormService _multiPageFormService;
        private UserFeedbackViewModel _userFeedbackViewModel;

        public UserFeedbackController(
            IUserFeedbackDataService userFeedbackDataService,
            IMultiPageFormService multiPageFormService
        )
        {
            this._userFeedbackDataService = userFeedbackDataService;
            this._multiPageFormService = multiPageFormService;
            this._userFeedbackViewModel = new UserFeedbackViewModel();
        }

        [Route("/Index")]
        public IActionResult Index(string sourceUrl, string sourcePageTitle)
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            _multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.AddUserFeedback, TempData);
            
            _userFeedbackViewModel = new()
            {
                UserId = User.GetUserId(),
                UserRoles = DeriveUserRoles(),
                SourceUrl = sourceUrl,
                SourcePageTitle = sourcePageTitle,
                TaskAchieved = null,
                TaskAttempted = string.Empty,
                FeedbackText = string.Empty,
                TaskRating = null,
            };

            if (_userFeedbackViewModel.UserId == null || _userFeedbackViewModel.UserId == 0)
            {
                return GuestFeedbackStart(_userFeedbackViewModel);
            }
            return StartUserFeedbackSession(_userFeedbackViewModel);
        }

        private string DeriveUserRoles()
        {
            List<string> roles = new List<string>();

            if (User.GetCustomClaimAsBool(CustomClaimTypes.LearnUserAuthenticated) ?? false)
            {
                roles.Add("LearningPortalAccess");
            }
            if (User.HasCentreAdminPermissions())
            {
                roles.Add("TrackingSystemAccess");
            }
            if (User.GetCustomClaimAsBool(CustomClaimTypes.UserAuthenticatedCm) ?? false)
            {
                roles.Add("ContentManagementSystemAccess");
            }
            if (User.GetCustomClaimAsBool(CustomClaimTypes.IsSupervisor) ?? false)
            {
                roles.Add("Supervisor");
            }
            if (User.GetCustomClaimAsBool(CustomClaimTypes.IsNominatedSupervisor) ?? false)
            {
                roles.Add("NominatedSupervisor");
            }
            if (User.GetCustomClaimAsBool(CustomClaimTypes.UserContentCreator) ?? false)
            {
                roles.Add("ContentCreatorAccess");
            }
            if (User.GetCustomClaimAsBool(CustomClaimTypes.IsFrameworkDeveloper) ?? false)
            {
                roles.Add("FrameworksAccess");
            }
            if (User.GetCustomClaimAsBool(CustomClaimTypes.IsFrameworkContributor) ?? false)
            {
                roles.Add("FrameworkContributor");
            }
            if (User.GetCustomClaimAsBool(CustomClaimTypes.IsWorkforceManager) ?? false)
            {
                roles.Add("WorkforceManager");
            }
            if (User.GetCustomClaimAsBool(CustomClaimTypes.IsWorkforceContributor) ?? false)
            {
                roles.Add("WorkforceContributor");
            }
            if (User.HasSuperAdminPermissions())
            {
                roles.Add("SuperAdminAccess");
            }

            return string.Join(", ", roles);
        }

        [HttpGet]
        [FeatureGate(FeatureFlags.UserFeedbackBar)]
        [Route("/StartUserFeedbackSession")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddUserFeedback) }
        )]
        public IActionResult StartUserFeedbackSession(UserFeedbackViewModel userFeedbackViewModel)
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            var userFeedbackSessionData = new UserFeedbackTempData()
            {
                UserId = userFeedbackViewModel.UserId,
                UserRoles = userFeedbackViewModel.UserRoles,
                SourceUrl = userFeedbackViewModel.SourceUrl,
                SourcePageTitle = userFeedbackViewModel.SourcePageTitle,
                TaskAchieved = userFeedbackViewModel.TaskAchieved,
                TaskAttempted = userFeedbackViewModel.TaskAttempted ?? string.Empty,
                FeedbackText = userFeedbackViewModel.FeedbackText ?? string.Empty,
                TaskRating = userFeedbackViewModel.TaskRating,
            };

            _multiPageFormService.SetMultiPageFormData(
                userFeedbackSessionData,
                MultiPageFormDataFeature.AddUserFeedback,
                TempData
            );

            userFeedbackViewModel = MapMultiformDataToViewModel(userFeedbackViewModel);

            return RedirectToAction("UserFeedbackTaskAchieved", userFeedbackViewModel);
        }

        [HttpGet]
        [FeatureGate(FeatureFlags.UserFeedbackBar)]
        [Route("/UserFeedbackTaskAchieved")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddUserFeedback) }
        )]
        public IActionResult UserFeedbackTaskAchieved(UserFeedbackViewModel userFeedbackViewModel)
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            userFeedbackViewModel = MapMultiformDataToViewModel(userFeedbackViewModel);

            return View("UserFeedbackTaskAchieved", userFeedbackViewModel);
        }

        [HttpPost]
        [FeatureGate(FeatureFlags.UserFeedbackBar)]
        [Route("/UserFeedbackTaskAchievedSet")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddUserFeedback) }
        )]
        public IActionResult UserFeedbackTaskAchievedSet(UserFeedbackViewModel userFeedbackViewModel)
        {
            userFeedbackViewModel = MapMultiformDataToViewModel(userFeedbackViewModel);

            SaveMultiPageFormData(userFeedbackViewModel);

            return RedirectToAction("UserFeedbackTaskAttempted", userFeedbackViewModel);
        }

        [HttpGet]
        [FeatureGate(FeatureFlags.UserFeedbackBar)]
        [Route("/UserFeedbackTaskAttempted")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddUserFeedback) }
        )]
        public IActionResult UserFeedbackTaskAttempted(UserFeedbackViewModel userFeedbackViewModel)
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            userFeedbackViewModel = MapMultiformDataToViewModel(userFeedbackViewModel);

            return View("UserFeedbackTaskAttempted", userFeedbackViewModel);
        }

        [HttpPost]
        [FeatureGate(FeatureFlags.UserFeedbackBar)]
        [Route("/UserFeedbackTaskAttemptedSet")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddUserFeedback) }
        )]
        public IActionResult UserFeedbackTaskAttemptedSet(UserFeedbackViewModel userFeedbackViewModel)
        {
            userFeedbackViewModel = MapMultiformDataToViewModel(userFeedbackViewModel);

            SaveMultiPageFormData(userFeedbackViewModel);

            return RedirectToAction("UserFeedbackTaskDifficulty", userFeedbackViewModel);
        }

        [HttpGet]
        [FeatureGate(FeatureFlags.UserFeedbackBar)]
        [Route("/UserFeedbackTaskDifficulty")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddUserFeedback) }
        )]
        public IActionResult UserFeedbackTaskDifficulty(UserFeedbackViewModel userFeedbackViewModel)
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            userFeedbackViewModel = MapMultiformDataToViewModel(userFeedbackViewModel);

            return View("UserFeedbackTaskDifficulty", userFeedbackViewModel);
        }

        [HttpPost]
        [FeatureGate(FeatureFlags.UserFeedbackBar)]
        [Route("/UserFeedbackTaskDifficultySet")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddUserFeedback) }
        )]
        public IActionResult UserFeedbackTaskDifficultySet(UserFeedbackViewModel userFeedbackViewModel)
        {
            userFeedbackViewModel = MapMultiformDataToViewModel(userFeedbackViewModel);

            SaveMultiPageFormData(userFeedbackViewModel);

            return RedirectToAction("UserFeedbackSave", userFeedbackViewModel);
        }

        public IActionResult UserFeedbackSave(UserFeedbackViewModel userFeedbackViewModel)
        {
            var data = _multiPageFormService.GetMultiPageFormData<UserFeedbackTempData>(
                MultiPageFormDataFeature.AddUserFeedback,
                TempData
            ).GetAwaiter().GetResult();

            using var transaction = new TransactionScope();

            _userFeedbackDataService.SaveUserFeedback(
                data.UserId,
                data.UserRoles,
                data.SourceUrl,
                data.TaskAchieved,
                data.TaskAttempted,
                data.FeedbackText,
                data.TaskRating
            );

            _multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.AddUserFeedback, TempData);

            transaction.Complete();

            userFeedbackViewModel.SourceUrl = data.SourceUrl;

            return RedirectToAction("UserFeedbackComplete", userFeedbackViewModel);
        }

        [HttpGet]
        [FeatureGate(FeatureFlags.UserFeedbackBar)]
        [Route("/UserFeedbackComplete")]
        public IActionResult UserFeedbackComplete(UserFeedbackViewModel userFeedbackViewModel)
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            return View("UserFeedbackComplete", userFeedbackViewModel);
        }

        [HttpGet]
        [FeatureGate(FeatureFlags.UserFeedbackBar)]
        [Route("/GuestFeedbackStart")]
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddUserFeedback) }
        )]
        public IActionResult GuestFeedbackStart(UserFeedbackViewModel userFeedbackViewModel)
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            return View("GuestFeedbackStart", userFeedbackViewModel);
        }

        [HttpGet]
        [FeatureGate(FeatureFlags.UserFeedbackBar)]
        [Route("/GuestFeedbackComplete")]
        [ResponseCache(CacheProfileName = "Never")]
        public IActionResult GuestFeedbackComplete()
        {
            var userFeedbackModel = new UserFeedbackViewModel();

            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            _multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.AddUserFeedback, TempData);

            return View("GuestFeedbackComplete", userFeedbackModel);
        }

        [HttpPost]
        [FeatureGate(FeatureFlags.UserFeedbackBar)]
        [Route("/GuestFeedbackComplete")]
        public IActionResult GuestFeedbackComplete(UserFeedbackViewModel userFeedbackViewModel)
        {
            if (!(userFeedbackViewModel.TaskAchieved == null && userFeedbackViewModel.TaskAttempted == null && userFeedbackViewModel.FeedbackText == null && userFeedbackViewModel.TaskRating == null))
            {
                using var transaction = new TransactionScope();

                _userFeedbackDataService.SaveUserFeedback(
                    null,
                    null,
                    userFeedbackViewModel.SourceUrl,
                    null,
                    userFeedbackViewModel.TaskAttempted,
                    userFeedbackViewModel.FeedbackText,
                    null
                );

                _multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.AddUserFeedback, TempData);

                transaction.Complete();
            }

            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            return RedirectToAction("GuestFeedbackComplete", userFeedbackViewModel);
        }

        [HttpPost]
        [FeatureGate(FeatureFlags.UserFeedbackBar)]
        [Route("/UserFeedbackReturnToUrl")]
        public IActionResult UserFeedbackReturnToUrl(string sourceUrl)
        {
            return Redirect(sourceUrl);
        }

        private UserFeedbackViewModel MapMultiformDataToViewModel(UserFeedbackViewModel viewModel)
        {
            var data = _multiPageFormService.GetMultiPageFormData<UserFeedbackTempData>(
                MultiPageFormDataFeature.AddUserFeedback,
                TempData
            ).GetAwaiter().GetResult();

            viewModel.UserId ??= data.UserId;
            viewModel.UserRoles ??= data.UserRoles;
            viewModel.SourceUrl ??= data.SourceUrl;
            viewModel.SourcePageTitle ??= data.SourcePageTitle;
            viewModel.TaskAchieved ??= data.TaskAchieved;
            viewModel.TaskAttempted ??= data.TaskAttempted;
            viewModel.FeedbackText ??= data.FeedbackText;
            viewModel.TaskRating ??= data.TaskRating;

            return viewModel;
        }

        private void SaveMultiPageFormData(UserFeedbackViewModel viewModelDelta)
        {
            var data = _multiPageFormService.GetMultiPageFormData<UserFeedbackTempData>(
                MultiPageFormDataFeature.AddUserFeedback,
                TempData
            ).GetAwaiter().GetResult();

            if (viewModelDelta.TaskAchieved != data.TaskAchieved)
            {
                data.TaskAchieved = viewModelDelta.TaskAchieved;
            }
            if (viewModelDelta.TaskAttempted != data.TaskAttempted)
            {
                data.TaskAttempted = viewModelDelta.TaskAttempted;
            }
            if (viewModelDelta.FeedbackText != data.FeedbackText)
            {
                data.FeedbackText = viewModelDelta.FeedbackText;
            }
            if (viewModelDelta.TaskRating != data.TaskRating)
            {
                data.TaskRating = viewModelDelta.TaskRating;
            }

            _multiPageFormService.SetMultiPageFormData(
                data,
                MultiPageFormDataFeature.AddUserFeedback,
                TempData
            );
        }
    }
}
