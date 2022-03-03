namespace DigitalLearningSolutions.Web.Controllers.Register
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
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
    using Microsoft.FeatureManagement;

    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    [SetSelectedTab(nameof(NavMenuTab.Register))]
    public class RegisterController : Controller
    {
        private readonly PromptsService promptsService;
        private readonly ICentresDataService centresDataService;
        private readonly ICryptoService cryptoService;
        private readonly IFeatureManager featureManager;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IRegistrationService registrationService;
        private readonly ISupervisorDelegateService supervisorDelegateService;
        private readonly IUserService userService;

        public RegisterController(
            ICentresDataService centresDataService,
            IJobGroupsDataService jobGroupsDataService,
            IRegistrationService registrationService,
            ICryptoService cryptoService,
            IUserService userService,
            PromptsService promptsService,
            IFeatureManager featureManager,
            ISupervisorDelegateService supervisorDelegateService
        )
        {
            this.centresDataService = centresDataService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.registrationService = registrationService;
            this.cryptoService = cryptoService;
            this.userService = userService;
            this.promptsService = promptsService;
            this.featureManager = featureManager;
            this.supervisorDelegateService = supervisorDelegateService;
        }

        public IActionResult Index(int? centreId = null, string? inviteId = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!CheckCentreIdValid(centreId))
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

            SetDelegateRegistrationData(
                centreId,
                supervisorDelegateRecord?.ID,
                supervisorDelegateRecord?.DelegateEmail
            );

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
            ValidateEmailAddress(model);

            return View(model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpPost]
        public IActionResult PersonalInformation(PersonalInformationViewModel model)
        {
            ValidateEmailAddress(model);

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

            var model = new LearnerInformationViewModel(data);
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
            var data = TempData.Peek<DelegateRegistrationData>()!;

            if (data.Centre == null || data.JobGroup == null)
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
                    registrationService.RegisterDelegate(
                        RegistrationMappingHelper.MapSelfRegistrationToDelegateRegistrationModel(data),
                        userIp,
                        refactoredTrackingSystemEnabled,
                        data.ProfessionalRegistrationNumber,
                        data.SupervisorDelegateId
                    );

                TempData.Clear();
                TempData.Add("candidateNumber", candidateNumber);
                TempData.Add("approved", approved);
                TempData.Add("centreId", centreId);
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
            var candidateNumber = (string?)TempData.Peek("candidateNumber");
            var approvedNullable = (bool?)TempData.Peek("approved");
            var centreIdNullable = (int?)TempData.Peek("centreId");
            TempData.Clear();

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

        private void SetDelegateRegistrationData(int? centreId, int? supervisorDelegateId, string? email)
        {
            var delegateRegistrationData = new DelegateRegistrationData(centreId, supervisorDelegateId, email);
            TempData.Set(delegateRegistrationData);
        }

        private bool CheckCentreIdValid(int? centreId)
        {
            return centreId == null
                   || centresDataService.GetCentreName(centreId.Value) != null;
        }

        private void ValidateEmailAddress(PersonalInformationViewModel model)
        {
            if (model.Email == null || !model.Centre.HasValue)
            {
                return;
            }

            if (!userService.IsDelegateEmailValidForCentre(model.Email, model.Centre!.Value))
            {
                ModelState.AddModelError(
                    nameof(PersonalInformationViewModel.Email),
                    "A user with this email address is already registered at this centre"
                );
            }
        }

        private IEnumerable<EditDelegateRegistrationPromptViewModel> GetEditDelegateRegistrationPromptViewModelsFromModel(
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

        private IEnumerable<DelegateRegistrationPrompt> GetDelegateRegistrationPromptsFromData(DelegateRegistrationData data)
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
            model.DelegateRegistrationPrompts = GetEditDelegateRegistrationPromptViewModelsFromModel(model, data.Centre!.Value);
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
    }
}
