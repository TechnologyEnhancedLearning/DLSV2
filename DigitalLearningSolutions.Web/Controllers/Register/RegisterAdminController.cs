namespace DigitalLearningSolutions.Web.Controllers.Register
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using Microsoft.AspNetCore.Mvc;

    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    public class RegisterAdminController : Controller
    {
        private readonly ICentresDataService centresDataService;
        private readonly ICentresService centresService;
        private readonly ICryptoService cryptoService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IRegistrationService registrationService;
        private readonly IUserDataService userDataService;
        private readonly IUserService userService;

        public RegisterAdminController(
            ICentresDataService centresDataService,
            ICentresService centresService,
            ICryptoService cryptoService,
            IJobGroupsDataService jobGroupsDataService,
            IRegistrationService registrationService,
            IUserDataService userDataService,
            IUserService userService
        )
        {
            this.centresDataService = centresDataService;
            this.centresService = centresService;
            this.cryptoService = cryptoService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.registrationService = registrationService;
            this.userDataService = userDataService;
            this.userService = userService;
        }

        public IActionResult Index(int? centreId = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!centreId.HasValue || centresDataService.GetCentreName(centreId.Value) == null)
            {
                return NotFound();
            }

            if (!IsRegisterAdminAllowed(centreId.Value))
            {
                return RedirectToAction("AccessDenied", "LearningSolutions");
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

            RegistrationEmailValidator.ValidateEmailAddresses(model, ModelState, userService, centresService);

            return View(model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<RegistrationData>))]
        [HttpPost]
        public IActionResult PersonalInformation(PersonalInformationViewModel model)
        {
            var data = TempData.Peek<RegistrationData>()!;

            RegistrationEmailValidator.ValidateEmailAddresses(model, ModelState, userService, centresService);

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

            var model = new LearnerInformationViewModel(data, true);
            SetJobGroupOptions(model);

            return View(model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<RegistrationData>))]
        [HttpPost]
        public IActionResult LearnerInformation(LearnerInformationViewModel model)
        {
            var data = TempData.Peek<RegistrationData>()!;

            ProfessionalRegistrationNumberHelper.ValidateProfessionalRegistrationNumber(
                ModelState,
                model.HasProfessionalRegistrationNumber,
                model.ProfessionalRegistrationNumber
            );

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

        [NoCaching]
        [ServiceFilter(typeof(RedirectEmptySessionData<RegistrationData>))]
        [HttpGet]
        public IActionResult Summary()
        {
            var data = TempData.Peek<RegistrationData>()!;
            var model = new SummaryViewModel(data);
            PopulateSummaryExtraFields(model, data);
            return View(model);
        }

        [NoCaching]
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

            try
            {
                var registrationModel = RegistrationMappingHelper.MapToCentreManagerAdminRegistrationModel(data);
                registrationService.RegisterCentreManager(
                    registrationModel,
                    data.JobGroup!.Value
                );

                return RedirectToAction("Confirmation");
            }
            catch (DelegateCreationFailedException e)
            {
                var error = e.Error;

                if (error.Equals(DelegateCreationError.UnexpectedError))
                {
                    return new StatusCodeResult(500);
                }

                if (error.Equals(DelegateCreationError.EmailAlreadyInUse))
                {
                    return RedirectToAction("Index");
                }

                return new StatusCodeResult(500);
            }
        }

        [HttpGet]
        public IActionResult Confirmation()
        {
            TempData.Clear();
            return View();
        }

        private bool IsRegisterAdminAllowed(int centreId)
        {
            var adminUsers = userDataService.GetAdminUsersByCentreId(centreId);
            var hasCentreManagerAdmin = adminUsers.Any(user => user.IsCentreManager);
            var (autoRegistered, autoRegisterManagerEmail) = centresDataService.GetCentreAutoRegisterValues(centreId);
            return !hasCentreManagerAdmin && !autoRegistered && !string.IsNullOrWhiteSpace(autoRegisterManagerEmail);
        }

        private void SetAdminRegistrationData(int centreId)
        {
            var adminRegistrationData = new RegistrationData(centreId);
            TempData.Clear();
            TempData.Set(adminRegistrationData);
        }

        private bool IsEmailUnique(string email)
        {
            var adminUser = userDataService.GetAdminUserByEmailAddress(email);
            return adminUser == null;
        }

        private bool CanProceedWithRegistration(RegistrationData data)
        {
            return data.Centre.HasValue && data.Email != null && IsRegisterAdminAllowed(data.Centre.Value) &&
                   centresService.DoesEmailMatchCentre(data.SecondaryEmail ?? data.Email, data.Centre.Value) &&
                   IsEmailUnique(data.Email) && (data.SecondaryEmail == null || IsEmailUnique(data.SecondaryEmail));
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
