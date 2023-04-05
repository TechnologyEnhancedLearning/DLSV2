namespace DigitalLearningSolutions.Web.Controllers.Register
{
    using System;
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using DigitalLearningSolutions.Web.ViewModels.Register.RegisterDelegateByCentre;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement.Mvc;
    using ConfirmationViewModel =
        DigitalLearningSolutions.Web.ViewModels.Register.RegisterDelegateByCentre.ConfirmationViewModel;
    using SummaryViewModel = DigitalLearningSolutions.Web.ViewModels.Register.RegisterDelegateByCentre.SummaryViewModel;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [Route("/TrackingSystem/Delegates/Register/{action}")]
    public class RegisterDelegateByCentreController : Controller
    {
        private readonly IConfiguration config;
        private readonly ICryptoService cryptoService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly PromptsService promptsService;
        private readonly IRegistrationService registrationService;
        private readonly IUserDataService userDataService;
        private readonly IClockUtility clockUtility;
        private readonly IUserService userService;

        public RegisterDelegateByCentreController(
            IJobGroupsDataService jobGroupsDataService,
            PromptsService promptsService,
            ICryptoService cryptoService,
            IUserDataService userDataService,
            IRegistrationService registrationService,
            IConfiguration config,
            IClockUtility clockUtility,
            IUserService userService
        )
        {
            this.jobGroupsDataService = jobGroupsDataService;
            this.promptsService = promptsService;
            this.userDataService = userDataService;
            this.registrationService = registrationService;
            this.cryptoService = cryptoService;
            this.config = config;
            this.clockUtility = clockUtility;
            this.userService = userService;
        }

        [NoCaching]
        [Route("/TrackingSystem/Delegates/Register")]
        public IActionResult Index()
        {
            var centreId = User.GetCentreIdKnownNotNull();

            SetCentreDelegateRegistrationData(centreId);

            return RedirectToAction("PersonalInformation");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationByCentreData>))]
        [HttpGet]
        public IActionResult PersonalInformation()
        {
            var data = TempData.Peek<DelegateRegistrationByCentreData>()!;
            var delegateRegistered = TempData.Peek("delegateRegistered")!;
            if (Convert.ToBoolean(delegateRegistered))
            {
                TempData.Clear();
                return RedirectToAction("LearningSolutions", "StatusCode", new { code = 410 });
            }
            var model = new RegisterDelegatePersonalInformationViewModel(data);

            ValidateEmailAddress(model);

            return View(model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationByCentreData>))]
        [HttpPost]
        public IActionResult PersonalInformation(RegisterDelegatePersonalInformationViewModel model)
        {
            var data = TempData.Peek<DelegateRegistrationByCentreData>()!;

            ValidateEmailAddress(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            data.SetPersonalInformation(model);
            TempData.Set(data);

            return RedirectToAction("LearnerInformation");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationByCentreData>))]
        [HttpGet]
        public IActionResult LearnerInformation()
        {
            var data = TempData.Peek<DelegateRegistrationByCentreData>()!;

            var model = new LearnerInformationViewModel(data, false);

            PopulateLearnerInformationExtraFields(model, data);

            return View(model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationByCentreData>))]
        [HttpPost]
        public IActionResult LearnerInformation(LearnerInformationViewModel model)
        {
            var data = TempData.Peek<DelegateRegistrationByCentreData>()!;

            var centreId = data.Centre!.Value;

            promptsService.ValidateCentreRegistrationPrompts(
                centreId,
                model.Answer1,
                model.Answer2,
                model.Answer3,
                model.Answer4,
                model.Answer5,
                model.Answer6,
                ModelState
            );

            ProfessionalRegistrationNumberHelper.ValidateProfessionalRegistrationNumber(
                ModelState,
                model.HasProfessionalRegistrationNumber,
                model.ProfessionalRegistrationNumber
            );

            if (!ModelState.IsValid)
            {
                PopulateLearnerInformationExtraFields(model, data);
                return View(model);
            }

            data.SetLearnerInformation(model);
            TempData.Set(data);

            return RedirectToAction("WelcomeEmail");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationByCentreData>))]
        [HttpGet]
        public IActionResult WelcomeEmail()
        {
            var data = TempData.Peek<DelegateRegistrationByCentreData>()!;

            var model = new WelcomeEmailViewModel(data);

            return View(model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationByCentreData>))]
        [HttpPost]
        public IActionResult WelcomeEmail(WelcomeEmailViewModel model)
        {
            var data = TempData.Peek<DelegateRegistrationByCentreData>()!;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            data.SetWelcomeEmail(model);
            TempData.Set(data);

            return RedirectToAction("Password");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationByCentreData>))]
        [HttpGet]
        public IActionResult Password()
        {
            var model = new PasswordViewModel();
            return View(model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationByCentreData>))]
        [HttpPost]
        public IActionResult Password(PasswordViewModel model)
        {
            var data = TempData.Peek<DelegateRegistrationByCentreData>()!;
            RegistrationPasswordValidator.ValidatePassword(model.Password, data.FirstName, data.LastName, ModelState);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            data.PasswordHash = model.Password != null ? cryptoService.GetPasswordHash(model.Password) : null;

            TempData.Set(data);

            return RedirectToAction("Summary");
        }

        [NoCaching]
        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationByCentreData>))]
        [HttpGet]
        public IActionResult Summary()
        {
            var data = TempData.Peek<DelegateRegistrationByCentreData>()!;
            var viewModel = new SummaryViewModel(data);
            PopulateSummaryExtraFields(viewModel, data);
            return View(viewModel);
        }

        [NoCaching]
        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationByCentreData>))]
        [HttpPost]
        public IActionResult Summary(SummaryViewModel model)
        {
            var data = TempData.Peek<DelegateRegistrationByCentreData>()!;
            var baseUrl = config.GetAppRootPath();

            try
            {
                var candidateNumber = registrationService.RegisterDelegateByCentre(
                    RegistrationMappingHelper.MapCentreRegistrationToDelegateRegistrationModel(data),
                    baseUrl,
                    false
                );

                TempData.Clear();
                TempData.Add("delegateNumber", candidateNumber);
                TempData.Add("passwordSet", data.IsPasswordSet);
                TempData.Add("delegateRegistered", true);
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
            var delegateNumber = (string?)TempData.Peek("delegateNumber");

            if (delegateNumber == null)
            {
                return RedirectToAction("Index");
            }

            var viewModel = new ConfirmationViewModel(delegateNumber);
            return View(viewModel);
        }

        private void SetCentreDelegateRegistrationData(int centreId)
        {
            var centreDelegateRegistrationData = new DelegateRegistrationByCentreData(centreId, clockUtility.UtcToday);
            TempData.Set(centreDelegateRegistrationData);
        }

        private void ValidateEmailAddress(RegisterDelegatePersonalInformationViewModel model)
        {
            RegistrationEmailValidator.ValidateEmailNotHeldAtCentreIfEmailNotYetValidated(
                model.CentreSpecificEmail,
                model.Centre!.Value,
                nameof(RegisterDelegatePersonalInformationViewModel.CentreSpecificEmail),
                ModelState,
                userService
            );
        }

        private IEnumerable<EditDelegateRegistrationPromptViewModel> GetEditCustomFieldsFromModel(
            LearnerInformationViewModel model,
            int centreId
        )
        {
            return promptsService.GetEditDelegateRegistrationPromptViewModelsForCentre(
                centreId,
                model.Answer1,
                model.Answer2,
                model.Answer3,
                model.Answer4,
                model.Answer5,
                model.Answer6
            );
        }

        private IEnumerable<DelegateRegistrationPrompt> GetCustomFieldsFromData(DelegateRegistrationData data)
        {
            return promptsService.GetDelegateRegistrationPromptsForCentre(
                data.Centre!.Value,
                data.Answer1,
                data.Answer2,
                data.Answer3,
                data.Answer4,
                data.Answer5,
                data.Answer6
            );
        }

        private void PopulateLearnerInformationExtraFields(
            LearnerInformationViewModel model,
            RegistrationData data
        )
        {
            model.DelegateRegistrationPrompts = GetEditCustomFieldsFromModel(model, data.Centre!.Value);
            model.JobGroupOptions = SelectListHelper.MapOptionsToSelectListItems(
                jobGroupsDataService.GetJobGroupsAlphabetical(),
                model.JobGroup
            );
        }

        private void PopulateSummaryExtraFields(SummaryViewModel model, DelegateRegistrationData data)
        {
            model.JobGroup = jobGroupsDataService.GetJobGroupName((int)data.JobGroup!);
            model.DelegateRegistrationPrompts = GetCustomFieldsFromData(data);
        }
    }
}
