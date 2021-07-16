namespace DigitalLearningSolutions.Web.Controllers
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

    public class RegisterAdminController : Controller
    {
        private const string CookieName = "AdminRegistrationData";
        private readonly ICentresDataService centresDataService;
        private readonly ICryptoService cryptoService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IRegistrationService registrationService;
        private readonly IUserDataService userDataService;

        public RegisterAdminController(
            ICentresDataService centresDataService,
            ICryptoService cryptoService,
            IJobGroupsDataService jobGroupsDataService,
            IRegistrationService registrationService,
            IUserDataService userDataService
        )
        {
            this.centresDataService = centresDataService;
            this.cryptoService = cryptoService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.registrationService = registrationService;
            this.userDataService = userDataService;
        }

        public IActionResult Index(int? centreId = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!centreId.HasValue || !IsRegisterAdminAllowed(centreId.Value))
            {
                return NotFound();
            }

            SetAdminRegistrationData(centreId.Value);

            return RedirectToAction("PersonalInformation");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<RegistrationData>))]
        [HttpGet]
        public IActionResult PersonalInformation()
        {
            var data = TempData.Peek<RegistrationData>()!;

            var model = new PersonalInformationViewModel(data);
            SetCentreName(model);

            ValidateEmailAddress(model.Email, model.Centre!.Value);

            return View(model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<RegistrationData>))]
        [HttpPost]
        public IActionResult PersonalInformation(PersonalInformationViewModel model)
        {
            var data = TempData.Peek<RegistrationData>()!;

            ValidateEmailAddress(model.Email, model.Centre!.Value);

            if (!ModelState.IsValid)
            {
                SetCentreName(model);
                return View(model);
            }

            data.SetPersonalInformation(model);
            TempData.Set(data);

            return RedirectToAction("LearnerInformation");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<RegistrationData>))]
        [HttpGet]
        public IActionResult LearnerInformation()
        {
            var data = TempData.Peek<RegistrationData>()!;

            var model = new LearnerInformationViewModel(data);
            SetJobGroupOptions(model);

            return View(model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<RegistrationData>))]
        [HttpPost]
        public IActionResult LearnerInformation(LearnerInformationViewModel model)
        {
            var data = TempData.Peek<RegistrationData>()!;

            if (!ModelState.IsValid)
            {
                SetJobGroupOptions(model);
                return View(model);
            }

            data.SetLearnerInformation(model);
            TempData.Set(data);

            return RedirectToAction("Password");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<RegistrationData>))]
        [HttpGet]
        public IActionResult Password()
        {
            return View(new ConfirmPasswordViewModel());
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<RegistrationData>))]
        [HttpPost]
        public IActionResult Password(ConfirmPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var data = TempData.Peek<RegistrationData>()!;
            data.PasswordHash = cryptoService.GetPasswordHash(model.Password!);
            TempData.Set(data);

            return RedirectToAction("Summary");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<RegistrationData>))]
        [HttpGet]
        public IActionResult Summary()
        {
            var data = TempData.Peek<RegistrationData>()!;
            var model = new SummaryViewModel(data);
            PopulateSummaryExtraFields(model, data);
            return View(model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<RegistrationData>))]
        [HttpPost]
        public IActionResult Summary(SummaryViewModel model)
        {
            var data = TempData.Peek<RegistrationData>()!;

            if (!ModelState.IsValid)
            {
                var viewModel = new SummaryViewModel(data);
                PopulateSummaryExtraFields(viewModel, data);
                viewModel.Terms = model.Terms;
                return View(viewModel);
            }

            if (!CanProceedWithRegistration(data))
            {
                return new StatusCodeResult(500);
            }

            var registrationModel = RegistrationMappingHelper.MapToRegistrationModel(data);
            registrationService.RegisterCentreManager(registrationModel);

            return RedirectToAction("Confirmation");
        }

        [HttpGet]
        public IActionResult Confirmation()
        {
            return View();
        }

        private bool IsRegisterAdminAllowed(int centreId)
        {
            if (centresDataService.GetCentreName(centreId) == null)
            {
                return false;
            }

            var adminUsers = userDataService.GetAdminUsersByCentreId(centreId);
            var hasCentreManagerAdmin = adminUsers.Any(user => user.IsCentreManager);
            var (autoRegistered, autoRegisterManagerEmail) = centresDataService.GetCentreAutoRegisterValues(centreId);
            return !hasCentreManagerAdmin && !autoRegistered && !string.IsNullOrWhiteSpace(autoRegisterManagerEmail);
        }

        private void SetAdminRegistrationData(int centreId)
        {
            var adminRegistrationData = new RegistrationData(centreId);
            var id = adminRegistrationData.Id;

            Response.Cookies.Append(
                CookieName,
                id.ToString(),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(30)
                }
            );

            TempData.Clear();
            TempData.Set(adminRegistrationData);
        }

        private bool IsEmailUnique(string email)
        {
            var adminUser = userDataService.GetAdminUserByEmailAddress(email);
            return adminUser == null;
        }

        private bool DoesEmailMatchCentre(string email, int centreId)
        {
            var autoRegisterManagerEmail =
                centresDataService.GetCentreAutoRegisterValues(centreId).autoRegisterManagerEmail;
            return email.Equals(autoRegisterManagerEmail);
        }

        private bool CanProceedWithRegistration(RegistrationData data)
        {
            return data.Centre.HasValue && data.Email != null && IsRegisterAdminAllowed(data.Centre.Value) &&
                   DoesEmailMatchCentre(data.Email, data.Centre.Value) && IsEmailUnique(data.Email);
        }

        private void ValidateEmailAddress(string? email, int centreId)
        {
            if (email == null)
            {
                return;
            }

            if (!DoesEmailMatchCentre(email, centreId))
            {
                ModelState.AddModelError(
                    nameof(PersonalInformationViewModel.Email),
                    "This email address does not match the one held by the centre"
                );
            }

            if (!IsEmailUnique(email))
            {
                ModelState.AddModelError(
                    nameof(PersonalInformationViewModel.Email),
                    "An admin user with this email address is already registered"
                );
            }
        }

        private void SetCentreName(PersonalInformationViewModel model)
        {
            model.CentreName = centresDataService.GetCentreName(model.Centre!.Value);
        }

        private void SetJobGroupOptions(LearnerInformationViewModel model)
        {
            model.JobGroupOptions = SelectListHelper.MapOptionsToSelectListItems(
                jobGroupsDataService.GetJobGroupsAlphabetical(),
                model.JobGroup
            );
        }

        private void PopulateSummaryExtraFields(SummaryViewModel model, RegistrationData data)
        {
            model.Centre = centresDataService.GetCentreName((int)data.Centre!);
            model.JobGroup = jobGroupsDataService.GetJobGroupName((int)data.JobGroup!);
        }
    }
}
