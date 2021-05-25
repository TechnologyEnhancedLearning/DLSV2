﻿namespace DigitalLearningSolutions.Web.Controllers
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
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
        private readonly ICryptoService cryptoService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IRegistrationService registrationService;
        private readonly IUserService userService;

        public RegisterController(ICentresDataService centresDataService, IJobGroupsDataService jobGroupsDataService,
            IRegistrationService registrationService, ICryptoService cryptoService, IUserService userService)
        {
            this.centresDataService = centresDataService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.registrationService = registrationService;
            this.cryptoService = cryptoService;
            this.userService = userService;
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

                ViewBag.CentreOptions = SelectListHelper.MapOptionsToSelectListItemsWithSelectedValue
                    (centresDataService.GetActiveCentresAlphabetical(), null);

                return View(delegateRegistrationData.RegisterViewModel);
            }

            ViewBag.CentreOptions = SelectListHelper.MapOptionsToSelectListItemsWithSelectedValue
                (centresDataService.GetActiveCentresAlphabetical(), delegateRegistrationData.RegisterViewModel.Centre);

            ValidateEmailAddress(delegateRegistrationData.RegisterViewModel);

            return View(delegateRegistrationData.RegisterViewModel);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpPost]
        public IActionResult Index(RegisterViewModel model)
        {
            ValidateEmailAddress(model);

            if (!ModelState.IsValid)
            {
                ViewBag.CentreOptions = SelectListHelper.MapOptionsToSelectListItemsWithSelectedValue
                    (centresDataService.GetActiveCentresAlphabetical(), model.Centre);
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
            ViewBag.JobGroupOptions = SelectListHelper.MapOptionsToSelectListItemsWithSelectedValue
                (jobGroupsDataService.GetJobGroupsAlphabetical(), viewModel.JobGroup);

            return View(viewModel);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpPost]
        public IActionResult LearnerInformation(LearnerInformationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.JobGroupOptions = SelectListHelper.MapOptionsToSelectListItemsWithSelectedValue
                    (jobGroupsDataService.GetJobGroupsAlphabetical(), model.JobGroup);
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
            return View();
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
            data.PasswordHash = cryptoService.GetPasswordHash(model.Password!);
            TempData.Set(data);

            return RedirectToAction("Summary");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpGet]
        public IActionResult Summary()
        {
            var data = TempData.Peek<DelegateRegistrationData>()!;
            var centre = centresDataService.GetCentreName((int)data.RegisterViewModel.Centre!);
            var jobGroup = jobGroupsDataService.GetJobGroupName((int)data.LearnerInformationViewModel.JobGroup!);
            var viewModel = RegistrationMappingHelper.MapToSummary(data, centre!, jobGroup!);

            return View(viewModel);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpPost]
        public IActionResult Summary(SummaryViewModel model)
        {
            var data = TempData.Peek<DelegateRegistrationData>()!;
            if (!ModelState.IsValid)
            {
                var centre = centresDataService.GetCentreName((int)data.RegisterViewModel.Centre!);
                var jobGroup = jobGroupsDataService.GetJobGroupName((int)data.LearnerInformationViewModel.JobGroup!);
                var viewModel = RegistrationMappingHelper.MapToSummary(data, centre!, jobGroup!);
                viewModel.Terms = model.Terms;
                return View(viewModel);
            }

            var baseUrl = ConfigHelper.GetAppConfig()["CurrentSystemBaseUrl"];
            var userIP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            var (candidateNumber, approved) =
                registrationService.RegisterDelegate(RegistrationMappingHelper.MapToDelegateRegistrationModel(data),
                    baseUrl, userIP);

            if (candidateNumber == "-1")
            {
                return RedirectToAction("Error", "LearningSolutions");
            }

            if (candidateNumber == "-4")
            {
                return RedirectToAction("Index");
            }

            TempData.Clear();
            TempData.Add("candidateNumber", candidateNumber);
            TempData.Add("approved", approved);
            TempData.Add("centreId", data.RegisterViewModel.Centre);
            return RedirectToAction("Confirmation");
        }

        [HttpGet]
        public IActionResult Confirmation()
        {
            var candidateNumber = (string?)TempData.Peek("candidateNumber");
            var approvedNullable = (bool?)TempData.Peek("approved");
            var centreIdNullable = (int?)TempData.Peek("centreId");
            if (candidateNumber == null || approvedNullable == null || centreIdNullable == null)
            {
                return RedirectToAction("Index");
            }

            var approved = (bool)approvedNullable;
            var centreId = (int)centreIdNullable;

            var centreIdForContactInformation = approved ? null : (int?)centreId;
            var viewModel = new ConfirmationViewModel(candidateNumber, approved, centreIdForContactInformation);
            return View(viewModel);
        }

        private void ValidateEmailAddress(RegisterViewModel model)
        {
            if (model.Email == null)
            {
                return;
            }

            var duplicateUsers = userService.GetUsersByEmailAddress(model.Email).delegateUsers.Where
                (u => u.CentreId == model.Centre);

            if (duplicateUsers.Count() != 0)
            {
                ModelState.AddModelError(nameof(RegisterViewModel.Email), "A user at this centre with this email address already exists");
            }
        }
    }
}
