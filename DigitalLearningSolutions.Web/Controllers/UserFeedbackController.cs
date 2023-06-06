﻿namespace DigitalLearningSolutions.Web.Controllers
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
        public IActionResult Index(string sourceUrl, string sourcePageTitle)
        {
            ViewData[LayoutViewDataKeys.DoNotDisplayUserFeedbackBar] = true;

            UserFeedbackViewModel userFeedbackViewModel = new()
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

            if (userFeedbackViewModel.UserId == null || userFeedbackViewModel.UserId == 0)
            {
                return GuestFeedbackStart(userFeedbackViewModel);
            }
            else
            {
                return StartUserFeedbackSession(userFeedbackViewModel);
            }
        }

        private string DeriveUserRoles()
        {
            List<string> roles = new List<string>();

            if (User.GetCustomClaimAsBool(CustomClaimTypes.LearnUserAuthenticated) ?? false)
            {
                roles.Add("LearningPortalAccess");
            };
            if (User.HasCentreAdminPermissions())
            {
                roles.Add("TrackingSystemAccess");
            };
            if (User.GetCustomClaimAsBool(CustomClaimTypes.UserAuthenticatedCm) ?? false)
            {
                roles.Add("ContentManagementSystemAccess");
            };
            if (User.GetCustomClaimAsBool(CustomClaimTypes.IsSupervisor) ?? false)
            {
                roles.Add("Supervisor");
            };
            if (User.GetCustomClaimAsBool(CustomClaimTypes.IsNominatedSupervisor) ?? false)
            {
                roles.Add("NominatedSupervisor");
            };
            if (User.GetCustomClaimAsBool(CustomClaimTypes.UserContentCreator) ?? false)
            {
                roles.Add("ContentCreatorAccess");
            };
            if (User.GetCustomClaimAsBool(CustomClaimTypes.IsFrameworkDeveloper) ?? false)
            {
                roles.Add("FrameworksAccess ");
            };
            if (User.GetCustomClaimAsBool(CustomClaimTypes.IsFrameworkContributor) ?? false)
            {
                roles.Add("FrameworkContributor");
            };
            if (User.GetCustomClaimAsBool(CustomClaimTypes.IsWorkforceManager) ?? false)
            {
                roles.Add("WorkforceManager");
            };
            if (User.GetCustomClaimAsBool(CustomClaimTypes.IsWorkforceContributor) ?? false)
            {
                roles.Add("WorkforceContributor");
            };
            if (User.HasSuperAdminPermissions())
            {
                roles.Add("SuperAdminAccess ");
            };

            return string.Join(", ", roles);
        }

        [HttpGet]
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
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddUserFeedback) }
        )]
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
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddUserFeedback) }
        )]
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
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddUserFeedback) }
        )]
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
                data.UserRoles,
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
        [ResponseCache(CacheProfileName = "Never")]
        [TypeFilter(
            typeof(RedirectToErrorEmptySessionData),
            Arguments = new object[] { nameof(MultiPageFormDataFeature.AddUserFeedback) }
        )]
        public IActionResult GuestFeedbackComplete(UserFeedbackViewModel userFeedbackViewModel)
        {
            if (!(userFeedbackViewModel.TaskAchieved == null && userFeedbackViewModel.TaskAttempted == null && userFeedbackViewModel.FeedbackText == null && userFeedbackViewModel.TaskRating == null))
            {
                _userFeedbackDataService.SaveUserFeedback(
                    null,
                    null,
                    userFeedbackViewModel.SourceUrl,
                    null,
                    userFeedbackViewModel.TaskAttempted,
                    userFeedbackViewModel.FeedbackText,
                    null
                );
            }

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
