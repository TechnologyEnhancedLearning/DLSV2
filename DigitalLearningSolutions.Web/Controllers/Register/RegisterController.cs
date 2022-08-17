namespace DigitalLearningSolutions.Web.Controllers.Register
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
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
    using Microsoft.FeatureManagement;

    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    [SetSelectedTab(nameof(NavMenuTab.Register))]
    public class RegisterController : Controller
    {
        private readonly ICentresDataService centresDataService;
        private readonly IConfiguration config;
        private readonly ICryptoService cryptoService;
        private readonly IEmailVerificationService emailVerificationService;
        private readonly IFeatureManager featureManager;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly PromptsService promptsService;
        private readonly IRegistrationService registrationService;
        private readonly ISupervisorDelegateService supervisorDelegateService;
        private readonly IUserDataService userDataService;
        private readonly IUserService userService;

        public RegisterController(
            ICentresDataService centresDataService,
            IJobGroupsDataService jobGroupsDataService,
            IRegistrationService registrationService,
            ICryptoService cryptoService,
            PromptsService promptsService,
            IFeatureManager featureManager,
            ISupervisorDelegateService supervisorDelegateService,
            IEmailVerificationService emailVerificationService,
            IUserService userService,
            IUserDataService userDataService,
            IConfiguration config
        )
        {
            this.centresDataService = centresDataService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.registrationService = registrationService;
            this.cryptoService = cryptoService;
            this.promptsService = promptsService;
            this.featureManager = featureManager;
            this.supervisorDelegateService = supervisorDelegateService;
            this.emailVerificationService = emailVerificationService;
            this.userService = userService;
            this.userDataService = userDataService;
            this.config = config;
        }

        public IActionResult Index(int? centreId = null, string? inviteId = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "RegisterAtNewCentre", new { centreId, inviteId });
            }

            var centreName = GetCentreName(centreId);

            if (centreId != null && centreName == null)
            {
                return NotFound();
            }

            var model = new RegisterViewModel(centreId, centreName, inviteId);

            return View(model);
        }

        [HttpGet]
        public IActionResult Start(int? centreId = null, string? inviteId = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "RegisterAtNewCentre", new { centreId, inviteId });
            }

            var centreName = GetCentreName(centreId);

            if (centreId != null && centreName == null)
            {
                return NotFound();
            }

            var supervisorDelegateRecord = centreId.HasValue && !string.IsNullOrEmpty(inviteId) &&
                                           Guid.TryParse(inviteId, out var inviteHash)
                ? supervisorDelegateService.GetSupervisorDelegateRecordByInviteHash(inviteHash)
                : null;

            if (supervisorDelegateRecord?.CentreId != centreId)
            {
                supervisorDelegateRecord = null;
            }

            var delegateRegistrationData = new DelegateRegistrationData(
                centreId,
                supervisorDelegateRecord?.ID,
                supervisorDelegateRecord?.DelegateEmail
            );

            TempData.Set(delegateRegistrationData);

            return RedirectToAction("PersonalInformation");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpGet]
        public IActionResult PersonalInformation()
        {
            var data = TempData.Peek<DelegateRegistrationData>()!;

            var model = new PersonalInformationViewModel(data);
            PopulatePersonalInformationExtraFields(model);

            // Check this email and centre combination doesn't already exist in case we were redirected
            // back here by the user trying to submit the final page of the form
            ValidateEmailAddresses(model);

            return View(model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpPost]
        public IActionResult PersonalInformation(PersonalInformationViewModel model)
        {
            ValidateEmailAddresses(model);

            var data = TempData.Peek<DelegateRegistrationData>()!;

            if (!ModelState.IsValid)
            {
                PopulatePersonalInformationExtraFields(model);
                return View(model);
            }

            if (data.Centre != model.Centre)
            {
                data.ClearCustomPromptAnswers();
            }

            data.SetPersonalInformation(model);
            TempData.Set(data);

            return RedirectToAction("LearnerInformation");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpGet]
        public IActionResult LearnerInformation()
        {
            var data = TempData.Peek<DelegateRegistrationData>()!;

            if (data.Centre == null)
            {
                return RedirectToAction("Index");
            }

            var model = new LearnerInformationViewModel(data, true);
            PopulateLearnerInformationExtraFields(model, data);
            return View(model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpPost]
        public IActionResult LearnerInformation(LearnerInformationViewModel model)
        {
            var data = TempData.Peek<DelegateRegistrationData>()!;

            if (data.Centre == null)
            {
                return RedirectToAction("Index");
            }

            var centreId = data.Centre.Value;

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

            return RedirectToAction("Password");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpGet]
        public IActionResult Password()
        {
            return View(new ConfirmPasswordViewModel());
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpPost]
        public IActionResult Password(ConfirmPasswordViewModel model)
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

        [NoCaching]
        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpGet]
        public IActionResult Summary()
        {
            var data = TempData.Peek<DelegateRegistrationData>()!;
            var viewModel = new SummaryViewModel(data);
            PopulateSummaryExtraFields(viewModel, data);
            return View(viewModel);
        }

        [NoCaching]
        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpPost]
        public async Task<IActionResult> Summary(SummaryViewModel model)
        {
            var data = TempData.Peek<DelegateRegistrationData>();

            if (data!.Centre == null || data.JobGroup == null)
            {
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                var viewModel = new SummaryViewModel(data);
                PopulateSummaryExtraFields(viewModel, data);
                viewModel.Terms = model.Terms;
                return View(viewModel);
            }

            var centreId = (int)data.Centre;
            var refactoredTrackingSystemEnabled =
                await featureManager.IsEnabledAsync(FeatureFlags.RefactoredTrackingSystem);

            var userIp = Request.GetUserIpAddressFromRequest();

            try
            {
                var (candidateNumber, approved) =
                    registrationService.CreateDelegateAccountForNewUser(
                        RegistrationMappingHelper.MapSelfRegistrationToDelegateRegistrationModel(data),
                        userIp,
                        refactoredTrackingSystemEnabled,
                        true,
                        data.SupervisorDelegateId
                    );

                CreateEmailVerificationHashesAndSendVerificationEmails(
                    candidateNumber,
                    model.PrimaryEmail!,
                    model.CentreSpecificEmail
                );

                TempData.Clear();

                return RedirectToAction(
                    "Confirmation",
                    new
                    {
                        centreId,
                        candidateNumber,
                        approved,
                        unverifiedPrimaryEmail = data.PrimaryEmail,
                        data.CentreSpecificEmail,
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
        public IActionResult Confirmation(
            int? centreId,
            string candidateNumber,
            bool approved,
            string? unverifiedPrimaryEmail,
            string? centreSpecificEmail
        )
        {
            if (centreId == null)
            {
                return RedirectToAction("Index");
            }

            var centreIdForContactInformation = approved ? null : centreId;
            var centreName = GetCentreName(centreId);

            var model = new ConfirmationViewModel(
                candidateNumber,
                approved,
                centreIdForContactInformation,
                unverifiedPrimaryEmail,
                centreSpecificEmail,
                centreName!
            );

            return View(model);
        }

        private string? GetCentreName(int? centreId)
        {
            return centreId == null ? null : centresDataService.GetCentreName(centreId.Value);
        }

        private IEnumerable<EditDelegateRegistrationPromptViewModel>
            GetEditDelegateRegistrationPromptViewModelsFromModel(
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

        private IEnumerable<DelegateRegistrationPrompt> GetDelegateRegistrationPromptsFromData(
            DelegateRegistrationData data
        )
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

        private void PopulatePersonalInformationExtraFields(PersonalInformationViewModel model)
        {
            model.CentreName = model.Centre.HasValue ? centresDataService.GetCentreName(model.Centre.Value) : null;
            model.CentreOptions = SelectListHelper.MapOptionsToSelectListItems(
                centresDataService.GetCentresForDelegateSelfRegistrationAlphabetical(),
                model.Centre
            );
        }

        private void PopulateLearnerInformationExtraFields(
            LearnerInformationViewModel model,
            DelegateRegistrationData data
        )
        {
            model.DelegateRegistrationPrompts =
                GetEditDelegateRegistrationPromptViewModelsFromModel(model, data.Centre!.Value);
            model.JobGroupOptions = SelectListHelper.MapOptionsToSelectListItems(
                jobGroupsDataService.GetJobGroupsAlphabetical(),
                model.JobGroup
            );
        }

        private void PopulateSummaryExtraFields(SummaryViewModel model, DelegateRegistrationData data)
        {
            model.Centre = centresDataService.GetCentreName((int)data.Centre!);
            model.JobGroup = jobGroupsDataService.GetJobGroupName((int)data.JobGroup!);
            model.DelegateRegistrationPrompts = GetDelegateRegistrationPromptsFromData(data);
        }

        private void ValidateEmailAddresses(PersonalInformationViewModel model)
        {
            RegistrationEmailValidator.ValidatePrimaryEmailIfNecessary(
                model.PrimaryEmail,
                nameof(PersonalInformationViewModel.PrimaryEmail),
                ModelState,
                userDataService,
                CommonValidationErrorMessages.EmailInUseDuringDelegateRegistration
            );

            RegistrationEmailValidator.ValidateCentreEmailIfNecessary(
                model.CentreSpecificEmail,
                model.Centre,
                nameof(PersonalInformationViewModel.CentreSpecificEmail),
                ModelState,
                userDataService
            );
        }

        private void CreateEmailVerificationHashesAndSendVerificationEmails(
            string candidateNumber,
            string primaryEmail,
            string? centreSpecificEmail
        )
        {
            var userId = userDataService.GetUserIdFromUsername(candidateNumber);
            var userEntity = userService.GetUserById((int)userId!);
            var unverifiedEmails = PossibleEmailUpdateHelper.GetUnverifiedPrimaryAndCentreEmails(
                primaryEmail,
                centreSpecificEmail
            );

            emailVerificationService.CreateEmailVerificationHashesAndSendVerificationEmails(
                userEntity!.UserAccount,
                unverifiedEmails,
                config.GetAppRootPath()
            );
        }
    }
}
