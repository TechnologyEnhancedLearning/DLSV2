namespace DigitalLearningSolutions.Web.Controllers.Register
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
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
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement;

    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    [SetSelectedTab(nameof(NavMenuTab.Register))]
    [Authorize(Policy = CustomPolicies.BasicUser)]
    public class RegisterAtNewCentreController : Controller
    {
        private readonly ICentresDataService centresDataService;
        private readonly IFeatureManager featureManager;
        private readonly PromptsService promptsService;
        private readonly IRegistrationService registrationService;
        private readonly ISupervisorDelegateService supervisorDelegateService;
        private readonly IUserService userService;
        private readonly IUserDataService userDataService;

        public RegisterAtNewCentreController(
            ICentresDataService centresDataService,
            IFeatureManager featureManager,
            PromptsService promptsService,
            IRegistrationService registrationService,
            ISupervisorDelegateService supervisorDelegateService,
            IUserService userService,
            IUserDataService userDataService
        )
        {
            this.centresDataService = centresDataService;
            this.featureManager = featureManager;
            this.promptsService = promptsService;
            this.registrationService = registrationService;
            this.supervisorDelegateService = supervisorDelegateService;
            this.userService = userService;
            this.userDataService = userDataService;
        }

        public IActionResult Index(int? centreId = null, string? inviteId = null)
        {
            if (!CheckCentreIdValid(centreId))
            {
                return NotFound();
            }

            // TODO HEEDLS-899 sort out supervisor delegate stuff, this is just copied from the external registration
            var supervisorDelegateRecord = centreId.HasValue && !string.IsNullOrEmpty(inviteId) &&
                                           Guid.TryParse(inviteId, out var inviteHash)
                ? supervisorDelegateService.GetSupervisorDelegateRecordByInviteHash(inviteHash)
                : null;

            if (supervisorDelegateRecord?.CentreId != centreId)
            {
                supervisorDelegateRecord = null;
            }

            TempData.Set(
                new InternalDelegateRegistrationData(
                    centreId,
                    supervisorDelegateRecord?.ID,
                    supervisorDelegateRecord?.DelegateEmail
                )
            );

            return RedirectToAction("PersonalInformation");
        }

        [HttpGet]
        [ServiceFilter(typeof(RedirectEmptySessionData<InternalDelegateRegistrationData>))]
        public IActionResult PersonalInformation()
        {
            var data = TempData.Peek<InternalDelegateRegistrationData>()!;

            var model = new InternalPersonalInformationViewModel(data);
            PopulatePersonalInformationExtraFields(model);

            // Check this email and centre combination doesn't already exist in case we were redirected
            // back here by the user trying to submit the final page of the form
            ValidateEmailAddress(model);

            return View(model);
        }

        [HttpPost]
        [ServiceFilter(typeof(RedirectEmptySessionData<InternalDelegateRegistrationData>))]
        public IActionResult PersonalInformation(InternalPersonalInformationViewModel model)
        {
            ValidateEmailAddress(model);

            if (model.Centre != null)
            {
                var delegateAccount = userService.GetUserById(User.GetUserIdKnownNotNull())!
                    .GetCentreAccountSet(model.Centre.Value)?.DelegateAccount;
                if (delegateAccount?.Active == true)
                {
                    ModelState.AddModelError(
                        nameof(InternalPersonalInformationViewModel.Centre),
                        "You are already registered at this centre"
                    );
                }
            }

            if (!ModelState.IsValid)
            {
                PopulatePersonalInformationExtraFields(model);
                return View(model);
            }

            var data = TempData.Peek<InternalDelegateRegistrationData>()!;

            if (data.Centre != model.Centre)
            {
                // If we've returned from the summary page to change values, we may have registration prompt answers
                // that are no longer valid as we've changed centres. In this case we need to clear them.
                data.ClearCustomPromptAnswers();
            }

            data.SetPersonalInformation(model);
            TempData.Set(data);

            return RedirectToAction("LearnerInformation");
        }

        [HttpGet]
        [ServiceFilter(typeof(RedirectEmptySessionData<InternalDelegateRegistrationData>))]
        public IActionResult LearnerInformation()
        {
            var data = TempData.Peek<InternalDelegateRegistrationData>()!;

            if (data.Centre == null)
            {
                return RedirectToAction("Index");
            }

            var model = new InternalLearnerInformationViewModel(data);
            PopulateLearnerInformationExtraFields(model, data);

            if (!model.DelegateRegistrationPrompts.Any())
            {
                return RedirectToAction("Summary");
            }

            return View(model);
        }

        [HttpPost]
        [ServiceFilter(typeof(RedirectEmptySessionData<InternalDelegateRegistrationData>))]
        public IActionResult LearnerInformation(InternalLearnerInformationViewModel model)
        {
            var data = TempData.Peek<InternalDelegateRegistrationData>()!;

            if (data.Centre == null)
            {
                return RedirectToAction("Index");
            }

            promptsService.ValidateCentreRegistrationPrompts(
                data.Centre.Value,
                model.Answer1,
                model.Answer2,
                model.Answer3,
                model.Answer4,
                model.Answer5,
                model.Answer6,
                ModelState
            );

            if (!ModelState.IsValid)
            {
                PopulateLearnerInformationExtraFields(model, data);
                return View(model);
            }

            data.SetLearnerInformation(model);
            TempData.Set(data);

            return RedirectToAction("Summary");
        }

        [HttpGet]
        [NoCaching]
        [ServiceFilter(typeof(RedirectEmptySessionData<InternalDelegateRegistrationData>))]
        public IActionResult Summary()
        {
            var data = TempData.Peek<InternalDelegateRegistrationData>()!;
            var viewModel = new InternalSummaryViewModel
            {
                CentreSpecificEmail = data.CentreSpecificEmail,
                Centre = centresDataService.GetCentreName((int)data.Centre!),
                DelegateRegistrationPrompts = promptsService.GetDelegateRegistrationPromptsForCentre(
                    data.Centre!.Value,
                    data.Answer1,
                    data.Answer2,
                    data.Answer3,
                    data.Answer4,
                    data.Answer5,
                    data.Answer6
                ),
            };
            return View(viewModel);
        }

        [HttpPost]
        [NoCaching]
        [ServiceFilter(typeof(RedirectEmptySessionData<InternalDelegateRegistrationData>))]
        public async Task<IActionResult> SummaryPost()
        {
            var data = TempData.Peek<InternalDelegateRegistrationData>()!;

            if (data.Centre == null)
            {
                return RedirectToAction("Index");
            }

            var userId = User.GetUserIdKnownNotNull();
            var refactoredTrackingSystemEnabled =
                await featureManager.IsEnabledAsync(FeatureFlags.RefactoredTrackingSystem);

            var userIp = Request.GetUserIpAddressFromRequest();

            try
            {
                var (candidateNumber, approved, userHasAdminAccountAtCentre) =
                    registrationService.CreateDelegateAccountForExistingUser(
                        RegistrationMappingHelper
                            .MapInternalDelegateRegistrationDataToInternalDelegateRegistrationModel(data),
                        userId,
                        userIp,
                        refactoredTrackingSystemEnabled
                    );

                TempData.Clear();
                TempData.Add("candidateNumber", candidateNumber);
                TempData.Add("approved", approved);
                TempData.Add("userHasAdminAccountAtCentre", userHasAdminAccountAtCentre);
                TempData.Add("centreId", data.Centre);
                return RedirectToAction("Confirmation");
            }
            catch (DelegateCreationFailedException e)
            {
                var error = e.Error;

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
            var hasAdminAccountNullable = (bool?)TempData.Peek("userHasAdminAccountAtCentre");
            TempData.Clear();

            if (candidateNumber == null || approvedNullable == null || centreIdNullable == null ||
                hasAdminAccountNullable == null)
            {
                return RedirectToAction("Index");
            }

            var hasAdminAccount = (bool)hasAdminAccountNullable;
            var approved = (bool)approvedNullable;
            var centreId = (int)centreIdNullable;

            var viewModel = new InternalConfirmationViewModel(
                candidateNumber,
                approved,
                hasAdminAccount,
                centreId
            );
            return View(viewModel);
        }

        private bool CheckCentreIdValid(int? centreId)
        {
            return centreId == null
                   || centresDataService.GetCentreName(centreId.Value) != null;
        }

        private void ValidateEmailAddress(InternalPersonalInformationViewModel model)
        {
            if (model.CentreSpecificEmail != null && userDataService.CentreSpecificEmailIsInUseAtCentre(
                model.CentreSpecificEmail,
                User.GetCentreIdKnownNotNull()
            ))
            {
                ModelState.AddModelError(
                    nameof(PersonalInformationViewModel.CentreSpecificEmail),
                    "This email is already in use by another user"
                );
            }
        }

        private void PopulatePersonalInformationExtraFields(InternalPersonalInformationViewModel model)
        {
            model.CentreName = model.Centre.HasValue ? centresDataService.GetCentreName(model.Centre.Value) : null;
            model.CentreOptions = SelectListHelper.MapOptionsToSelectListItems(
                centresDataService.GetCentresForDelegateSelfRegistrationAlphabetical(),
                model.Centre
            );
        }

        private void PopulateLearnerInformationExtraFields(
            InternalLearnerInformationViewModel model,
            InternalDelegateRegistrationData data
        )
        {
            model.DelegateRegistrationPrompts = promptsService.GetEditDelegateRegistrationPromptViewModelsForCentre(
                data.Centre!.Value,
                model.Answer1,
                model.Answer2,
                model.Answer3,
                model.Answer4,
                model.Answer5,
                model.Answer6
            );
        }
    }
}
