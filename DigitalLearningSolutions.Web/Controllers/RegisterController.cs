﻿namespace DigitalLearningSolutions.Web.Controllers
{
    using System;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class RegisterController : Controller
    {
        private const string CookieName = "RegistrationData";
        private readonly ICentresDataService centresDataService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IRegistrationService registrationService;

        public RegisterController(ICentresDataService centresDataService, IJobGroupsDataService jobGroupsDataService,
            IRegistrationService registrationService)
        {
            this.centresDataService = centresDataService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.registrationService = registrationService;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var delegateRegistrationData = TempData.Peek<DelegateRegistrationData>();

            if (delegateRegistrationData == null || !Request.Cookies.ContainsKey(CookieName))
            {
                delegateRegistrationData = new DelegateRegistrationData();
                var id = delegateRegistrationData.Id;

                Response.Cookies.Append(
                    CookieName,
                    id.ToString(),
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(30)
                    });
                TempData.Set(delegateRegistrationData);

                return View();
            }

            return View(delegateRegistrationData.RegisterViewModel);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpPost]
        public IActionResult Index(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var data = TempData.Peek<DelegateRegistrationData>()!;
            data.RegisterViewModel = model;
            TempData.Set(data);

            return RedirectToAction("LearnerInformation");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpGet]
        public IActionResult LearnerInformation()
        {
            var data = TempData.Peek<DelegateRegistrationData>()!;
            var viewModel = data.LearnerInformationViewModel;
            ViewBag.Centres = centresDataService.GetActiveCentresAlphabetical();
            ViewBag.JobGroups = jobGroupsDataService.GetJobGroupsAlphabetical();

            return View(viewModel);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpPost]
        public IActionResult LearnerInformation(LearnerInformationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Centres = centresDataService.GetActiveCentresAlphabetical();
                ViewBag.JobGroups = jobGroupsDataService.GetJobGroupsAlphabetical();
                return View(model);
            }

            var data = TempData.Peek<DelegateRegistrationData>()!;
            data.LearnerInformationViewModel = model;
            TempData.Set(data);

            return RedirectToAction("Password");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpGet]
        public IActionResult Password()
        {
            var data = TempData.Peek<DelegateRegistrationData>()!;
            var viewModel = data.PasswordViewModel;

            return View(viewModel);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpPost]
        public IActionResult Password(PasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var data = TempData.Peek<DelegateRegistrationData>()!;
            // TODO HEEDLS-396 only ever store the password hashed
            data.PasswordViewModel = model;
            TempData.Set(data);

            return RedirectToAction("Summary");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpGet]
        public IActionResult Summary()
        {
            var data = TempData.Peek<DelegateRegistrationData>()!;
            var viewModel = MapToSummary(data);

            return View(viewModel);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpPost]
        public IActionResult Summary(SummaryViewModel model)
        {
            var data = TempData.Peek<DelegateRegistrationData>()!;
            if (!ModelState.IsValid)
            {
                var viewModel = MapToSummary(data);
                viewModel.Terms = model.Terms;
                return View(viewModel);
            }

            var candidateNumber = registrationService.RegisterDelegate(MapToDelegateRegistrationModel(data));
            TempData.Clear();
            TempData.Add("candidateNumber", candidateNumber);
            return RedirectToAction("Confirmation");
        }

        [HttpGet]
        public IActionResult Confirmation()
        {
            var candidateNumber = (string?)TempData.Peek("candidateNumber");
            if (candidateNumber == null)
            {
                return RedirectToAction("Index");
            }

            var viewModel = new ConfirmationViewModel(candidateNumber);
            return View(viewModel);
        }

        private SummaryViewModel MapToSummary(DelegateRegistrationData data)
        {
            var centre = centresDataService.GetCentreName((int)data.LearnerInformationViewModel.Centre!);
            var jobGroup = jobGroupsDataService.GetJobGroupName((int)data.LearnerInformationViewModel.JobGroup!);
            return new SummaryViewModel
            (
                data.RegisterViewModel.FirstName!,
                data.RegisterViewModel.LastName!,
                data.RegisterViewModel.Email!,
                centre!,
                jobGroup!
            );
        }

        private static DelegateRegistrationModel MapToDelegateRegistrationModel(DelegateRegistrationData data)
        {
            return new DelegateRegistrationModel
            {
                FirstName = data.RegisterViewModel.FirstName!,
                LastName = data.RegisterViewModel.LastName!,
                Email = data.RegisterViewModel.Email!,
                Centre = (int)data.LearnerInformationViewModel.Centre!,
                JobGroup = (int)data.LearnerInformationViewModel.JobGroup!,
                Password = data.PasswordViewModel.Password!
            };
        }
    }
}
