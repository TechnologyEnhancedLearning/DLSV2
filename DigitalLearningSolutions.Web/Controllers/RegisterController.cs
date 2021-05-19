namespace DigitalLearningSolutions.Web.Controllers
{
    using System;
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
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IRegistrationService registrationService;
        private readonly ICryptoService cryptoService;
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

                return View();
            }

            var email = delegateRegistrationData.RegisterViewModel.Email;
            if (email != null && userService.GetUsersByEmailAddress(email).delegateUsers.Count != 0)
            {
                ModelState.AddModelError(nameof(RegisterViewModel.Email), "A user with this email address already exists");
            }

            return View(delegateRegistrationData.RegisterViewModel);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpPost]
        public IActionResult Index(RegisterViewModel model)
        {
            if (model.Email != null && userService.GetUsersByEmailAddress(model.Email).delegateUsers.Count != 0)
            {
                ModelState.AddModelError(nameof(RegisterViewModel.Email), "A user with this email address already exists");
            }

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
            var centre = centresDataService.GetCentreName((int)data.LearnerInformationViewModel.Centre!);
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
                var centre = centresDataService.GetCentreName((int)data.LearnerInformationViewModel.Centre!);
                var jobGroup = jobGroupsDataService.GetJobGroupName((int)data.LearnerInformationViewModel.JobGroup!);
                var viewModel = RegistrationMappingHelper.MapToSummary(data, centre!, jobGroup!);
                viewModel.Terms = model.Terms;
                return View(viewModel);
            }

            var baseUrl = ConfigHelper.GetAppConfig()["CurrentSystemBaseUrl"];
            var candidateNumber = registrationService.RegisterDelegate(RegistrationMappingHelper.MapToDelegateRegistrationModel(data), baseUrl);
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
    }
}
