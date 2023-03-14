namespace DigitalLearningSolutions.Web.Controllers.Register
{
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    public class RegisterAdminController : Controller
    {
        private readonly ICentresDataService centresDataService;
        private readonly ICentresService centresService;
        private readonly IConfiguration config;
        private readonly ICryptoService cryptoService;
        private readonly IEmailVerificationService emailVerificationService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IRegisterAdminService registerAdminService;
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
            IRegisterAdminService registerAdminService,
            IEmailVerificationService emailVerificationService,
            IUserService userService,
            IConfiguration config
        )
        {
            this.centresDataService = centresDataService;
            this.centresService = centresService;
            this.cryptoService = cryptoService;
            this.emailVerificationService = emailVerificationService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.registrationService = registrationService;
            this.userDataService = userDataService;
            this.registerAdminService = registerAdminService;
            this.userService = userService;
            this.config = config;
        }

        public IActionResult Index(int? centreId = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "RegisterInternalAdmin", new { centreId });
            }

            if (!centreId.HasValue || centresDataService.GetCentreName(centreId.Value) == null)
            {
                return NotFound();
            }

            if (!registerAdminService.IsRegisterAdminAllowed(centreId.Value))
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

            ValidateEmailAddresses(model);

            return View(model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<RegistrationData>))]
        [HttpPost]
        public IActionResult PersonalInformation(PersonalInformationViewModel model)
        {
            var data = TempData.Peek<RegistrationData>()!;

            ValidateEmailAddresses(model);

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
                registrationService.RegisterCentreManager(registrationModel, true);

                CreateEmailVerificationHashesAndSendVerificationEmails(
                    registrationModel.PrimaryEmail!,
                    registrationModel.CentreSpecificEmail
                );

                return RedirectToAction(
                    "Confirmation",
                    new
                    {
                        primaryEmail = data.PrimaryEmail,
                        centreId = data.Centre,
                        centreSpecificEmail = data.CentreSpecificEmail,
                    }
                );
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
        public IActionResult Confirmation(string primaryEmail, int centreId, string? centreSpecificEmail)
        {
            TempData.Clear();

            var centreName = centresDataService.GetCentreName(centreId);

            var model = new AdminConfirmationViewModel(primaryEmail, centreSpecificEmail, centreName!);

            return View(model);
        }

        private void SetAdminRegistrationData(int centreId)
        {
            var adminRegistrationData = new RegistrationData(centreId);
            TempData.Clear();
            TempData.Set(adminRegistrationData);
        }

        private bool CanProceedWithRegistration(RegistrationData data)
        {
            return data.Centre.HasValue && data.PrimaryEmail != null &&
                   registerAdminService.IsRegisterAdminAllowed(data.Centre.Value) &&
                   centresService.IsAnEmailValidForCentreManager(
                       data.PrimaryEmail,
                       data.CentreSpecificEmail,
                       data.Centre.Value
                   ) &&
                   !userDataService.PrimaryEmailIsInUse(data.PrimaryEmail) &&
                   (data.CentreSpecificEmail == null || !userDataService.CentreSpecificEmailIsInUseAtCentre(
                       data.CentreSpecificEmail,
                       data.Centre.Value
                   ));
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

        private void ValidateEmailAddresses(PersonalInformationViewModel model)
        {
            RegistrationEmailValidator.ValidatePrimaryEmailIfNecessary(
                model.PrimaryEmail,
                nameof(RegistrationData.PrimaryEmail),
                ModelState,
                userDataService,
                CommonValidationErrorMessages.EmailInUseDuringAdminRegistration
            );

            RegistrationEmailValidator.ValidateCentreEmailIfNecessary(
                model.CentreSpecificEmail,
                model.Centre,
                nameof(RegistrationData.CentreSpecificEmail),
                ModelState,
                userDataService
            );

            RegistrationEmailValidator.ValidateEmailsForCentreManagerIfNecessary(
                model.PrimaryEmail,
                model.CentreSpecificEmail,
                model.Centre,
                model.CentreSpecificEmail == null
                    ? nameof(PersonalInformationViewModel.PrimaryEmail)
                    : nameof(PersonalInformationViewModel.CentreSpecificEmail),
                ModelState,
                centresService
            );
        }

        private void CreateEmailVerificationHashesAndSendVerificationEmails(
            string primaryEmail,
            string? centreSpecificEmail
        )
        {
            var userAccount = userService.GetUserAccountByEmailAddress(primaryEmail);

            var unverifiedEmails = new List<string> { primaryEmail };
            if (centreSpecificEmail != null)
            {
                unverifiedEmails.Add(centreSpecificEmail);
            }

            emailVerificationService.CreateEmailVerificationHashesAndSendVerificationEmails(
                userAccount!,
                unverifiedEmails.ToList(),
                config.GetAppRootPath()
            );
        }
    }
}
