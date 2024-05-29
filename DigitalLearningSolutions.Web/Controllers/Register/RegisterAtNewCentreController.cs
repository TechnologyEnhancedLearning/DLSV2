﻿namespace DigitalLearningSolutions.Web.Controllers.Register
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement;

    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    [SetSelectedTab(nameof(NavMenuTab.Register))]
    [Authorize(Policy = CustomPolicies.BasicUser)]
    [ServiceFilter(typeof(VerifyUserHasVerifiedPrimaryEmail))]
    public class RegisterAtNewCentreController : Controller
    {
        private readonly ICentresDataService centresDataService;
        private readonly IConfiguration config;
        private readonly IEmailVerificationService emailVerificationService;
        private readonly IFeatureManager featureManager;
        private readonly PromptsService promptsService;
        private readonly IRegistrationService registrationService;
        private readonly ISupervisorDelegateService supervisorDelegateService;
        private readonly IUserDataService userDataService;
        private readonly IUserService userService;
        private readonly ISupervisorService supervisorService;

        public RegisterAtNewCentreController(
            ICentresDataService centresDataService,
            IConfiguration config,
            IEmailVerificationService emailVerificationService,
            IFeatureManager featureManager,
            PromptsService promptsService,
            IRegistrationService registrationService,
            ISupervisorDelegateService supervisorDelegateService,
            IUserService userService,
            IUserDataService userDataService,
            ISupervisorService supervisorService
        )
        {
            this.centresDataService = centresDataService;
            this.config = config;
            this.emailVerificationService = emailVerificationService;
            this.featureManager = featureManager;
            this.promptsService = promptsService;
            this.registrationService = registrationService;
            this.supervisorDelegateService = supervisorDelegateService;
            this.userService = userService;
            this.userDataService = userDataService;
            this.supervisorService = supervisorService;
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
            if (model.Centre != null)
            {
                ValidateEmailAddress(model);

                var delegateAccount = userService.GetUserById(User.GetUserIdKnownNotNull())!
                    .GetCentreAccountSet(model.Centre.Value)?.DelegateAccount;

                if (delegateAccount?.Active == true)
                {
                    ModelState.AddModelError(
                        nameof(InternalPersonalInformationViewModel.Centre),
                        "You are already registered at this centre"
                    );
                }

                int? approvedDelegateId = supervisorService.ValidateDelegate(model.Centre.Value, model.CentreSpecificEmail);
                if (approvedDelegateId != null && approvedDelegateId > 0)
                {
                    ModelState.AddModelError("CentreSpecificEmail", "A user with this email address is already registered.");
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

            bool userHasDelAccAtAdminCentre = false;

            var userEntity = userService.GetUserById(userId);

            if (userEntity.AdminAccounts.Any())
            {
                var adminAccountAtCentre = userEntity.AdminAccounts.Where(a => a.CentreId == data.Centre).ToList();
                if (adminAccountAtCentre.Any())
                {
                    var delegateAccount = userEntity.DelegateAccounts.Where(da => da.CentreId == data.Centre).ToList();
                    if (!delegateAccount.Any())
                    {
                        userHasDelAccAtAdminCentre = true;
                    }
                }
            }

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

                if (data.CentreSpecificEmail != null &&
                    !emailVerificationService.AccountEmailIsVerifiedForUser(userId, data.CentreSpecificEmail))
                {
                    var userAccount = userService.GetUserAccountById(userId);

                    emailVerificationService.CreateEmailVerificationHashesAndSendVerificationEmails(
                        userAccount!,
                        new List<string> { data.CentreSpecificEmail },
                        config.GetAppRootPath()
                    );
                }

                TempData.Clear();

                return RedirectToAction(
                    "Confirmation",
                    new { candidateNumber, approved, userHasAdminAccountAtCentre, centreId = data.Centre, userHasDelAccAtAdminCentre }
                );
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
        public IActionResult Confirmation(
            string candidateNumber,
            bool approved,
            bool userHasAdminAccountAtCentre,
            int? centreId,
            bool userHasDelAccAtAdminCentre = false
        )
        {
            if (centreId == null)
            {
                return RedirectToAction("Index");
            }

            var userId = User.GetUserIdKnownNotNull();

            var (_, unverifiedCentreEmails) =
                userService.GetUnverifiedEmailsForUser(userId);
            var (_, centreName, unverifiedCentreEmail) =
                unverifiedCentreEmails.SingleOrDefault(uce => uce.centreId == centreId);

            var model = new InternalConfirmationViewModel(
                candidateNumber,
                approved,
                userHasAdminAccountAtCentre,
                centreId,
                unverifiedCentreEmail,
                centreName,
                userHasDelAccAtAdminCentre
            );

            return View(model);
        }

        private bool CheckCentreIdValid(int? centreId)
        {
            return centreId == null || centresDataService.GetCentreName(centreId.Value) != null;
        }

        private void ValidateEmailAddress(InternalPersonalInformationViewModel model)
        {
            if (model.Centre != null)
            {
                RegistrationEmailValidator.ValidateCentreEmailWithUserIdIfNecessary(
                    model.CentreSpecificEmail,
                    model.Centre,
                    User.GetUserIdKnownNotNull(),
                    nameof(PersonalInformationViewModel.CentreSpecificEmail),
                    ModelState,
                    userService
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
