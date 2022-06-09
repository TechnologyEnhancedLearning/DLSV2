namespace DigitalLearningSolutions.Web.Controllers.Register
{
    using System;
    using System.Linq;
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
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement;

    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    [SetSelectedTab(nameof(NavMenuTab.Register))]
    [Route("RegisterAtNewCentre")]
    [Authorize(Policy = CustomPolicies.BasicUser)]
    public class InternalDelegateRegistrationController : Controller
    {
        private readonly ICentresDataService centresDataService;
        private readonly IFeatureManager featureManager;
        private readonly PromptsService promptsService;
        private readonly IRegistrationService registrationService;
        private readonly ISupervisorDelegateService supervisorDelegateService;
        private readonly IUserService userService;

        public InternalDelegateRegistrationController(
            ICentresDataService centresDataService,
            IFeatureManager featureManager,
            PromptsService promptsService,
            IRegistrationService registrationService,
            ISupervisorDelegateService supervisorDelegateService,
            IUserService userService
        )
        {
            this.centresDataService = centresDataService;
            this.featureManager = featureManager;
            this.promptsService = promptsService;
            this.registrationService = registrationService;
            this.supervisorDelegateService = supervisorDelegateService;
            this.userService = userService;
        }

        public IActionResult Index(int? centreId = null, string? inviteId = null)
        {
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

            TempData.Set(new InternalDelegateRegistrationData(centreId, supervisorDelegateRecord?.ID));

            return RedirectToAction("PersonalInformation");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<InternalDelegateRegistrationData>))]
        [HttpGet]
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

        [ServiceFilter(typeof(RedirectEmptySessionData<InternalDelegateRegistrationData>))]
        [HttpPost]
        public IActionResult PersonalInformation(InternalPersonalInformationViewModel model)
        {
            ValidateEmailAddress(model);

            var data = TempData.Peek<InternalDelegateRegistrationData>()!;

            if (model.Centre != null)
            {
                var delegateAccount = userService.GetUserById(User.GetUserIdKnownNotNull())!.DelegateAccounts
                    .SingleOrDefault(da => da.CentreId == model.Centre);
                if (delegateAccount != null)
                {
                    ModelState.AddModelError(
                        nameof(InternalPersonalInformationViewModel.Centre),
                        delegateAccount.Active
                            ? "You are already registered at this centre"
                            : "You already have an inactive account at this centre - you can reactivate it via the Switch centre page"
                    );
                }
            }

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

        [ServiceFilter(typeof(RedirectEmptySessionData<InternalDelegateRegistrationData>))]
        [HttpGet]
        public IActionResult LearnerInformation()
        {
            var data = TempData.Peek<InternalDelegateRegistrationData>()!;

            if (data.Centre == null)
            {
                return RedirectToAction("Index");
            }

            var model = new InternalLearnerInformationViewModel(data);
            PopulateLearnerInformationExtraFields(model, data);
            return View(model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<InternalDelegateRegistrationData>))]
        [HttpPost]
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

        [NoCaching]
        [ServiceFilter(typeof(RedirectEmptySessionData<InternalDelegateRegistrationData>))]
        [HttpGet]
        public IActionResult Summary()
        {
            var data = TempData.Peek<InternalDelegateRegistrationData>()!;
            var viewModel = new InternalSummaryViewModel(data)
            {
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

        [NoCaching]
        [ServiceFilter(typeof(RedirectEmptySessionData<InternalDelegateRegistrationData>))]
        [HttpPost]
        public async Task<IActionResult> Summary(InternalSummaryViewModel model)
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
                var (candidateNumber, approved) =
                    registrationService.CreateDelegateAccountForExistingUser(
                        RegistrationMappingHelper
                            .MapInternalDelegateRegistrationModelRegistrationToInternalDelegateRegistrationModel(data),
                        userId,
                        userIp,
                        refactoredTrackingSystemEnabled
                    );

                TempData.Clear();
                //TempData.Add("candidateNumber", candidateNumber);
                //TempData.Add("approved", approved);
                //TempData.Add("centreId", centreId);
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

        private bool CheckCentreIdValid(int? centreId)
        {
            return centreId == null
                   || centresDataService.GetCentreName(centreId.Value) != null;
        }

        private void ValidateEmailAddress(InternalPersonalInformationViewModel model)
        {
            if (model.SecondaryEmail != null && userService.EmailIsInUse(model.SecondaryEmail))
            {
                ModelState.AddModelError(
                    nameof(PersonalInformationViewModel.SecondaryEmail),
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
