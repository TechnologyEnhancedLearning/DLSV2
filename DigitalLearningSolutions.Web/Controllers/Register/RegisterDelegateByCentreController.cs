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
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using DigitalLearningSolutions.Web.ViewModels.Register.RegisterDelegateByCentre;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.BulkUpload;
    using GDS.MultiPageFormData;
    using GDS.MultiPageFormData.Enums;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement.Mvc;
    using ConfirmationViewModel = ViewModels.Register.RegisterDelegateByCentre.ConfirmationViewModel;
    using SummaryViewModel = ViewModels.Register.RegisterDelegateByCentre.SummaryViewModel;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [Route("/TrackingSystem/Delegates/Register/{action}")]
    public class RegisterDelegateByCentreController : Controller
    {
        private readonly IConfiguration config;
        private readonly ICryptoService cryptoService;
        private readonly IJobGroupsService jobGroupsService;
        private readonly PromptsService promptsService;
        private readonly IRegistrationService registrationService;
        private readonly IUserDataService userDataService;
        private readonly IClockUtility clockUtility;
        private readonly IUserService userService;
        private readonly IMultiPageFormService multiPageFormService;
        private readonly IGroupsService groupsService;

        public RegisterDelegateByCentreController(
            IJobGroupsService jobGroupsService,
            PromptsService promptsService,
            ICryptoService cryptoService,
            IUserDataService userDataService,
            IRegistrationService registrationService,
            IConfiguration config,
            IClockUtility clockUtility,
            IUserService userService,
            IMultiPageFormService multiPageFormService,
            IGroupsService groupsService
        )
        {
            this.jobGroupsService = jobGroupsService;
            this.promptsService = promptsService;
            this.userDataService = userDataService;
            this.registrationService = registrationService;
            this.cryptoService = cryptoService;
            this.config = config;
            this.clockUtility = clockUtility;
            this.userService = userService;
            this.multiPageFormService = multiPageFormService;
            this.groupsService = groupsService;
        }

        [NoCaching]
        [Route("/TrackingSystem/Delegates/Register")]
        public IActionResult Index()
        {
            var centreId = User.GetCentreIdKnownNotNull();

            SetupDelegateRegistrationByCentreData(centreId);

            return RedirectToAction("PersonalInformation");
        }

        [NoCaching]
        [HttpGet]
        public IActionResult PersonalInformation()
        {
            var data = GetDelegateRegistrationByCentreData()!;
            var delegateRegistered = TempData.Peek("delegateRegistered")!;
            if (Convert.ToBoolean(delegateRegistered))
            {
                TempData.Clear();
                return RedirectToAction("StatusCode", "LearningSolutions", new { code = 410 });
            }
            var model = new RegisterDelegatePersonalInformationViewModel(data);
            ValidateEmailAddress(model);

            return View(model);
        }

        [HttpPost]
        public IActionResult PersonalInformation(RegisterDelegatePersonalInformationViewModel model)
        {
            var data = GetDelegateRegistrationByCentreData()!;

            ValidateEmailAddress(model);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            data.SetPersonalInformation(model);
            SetDelegateRegistrationByCentreData(data);

            return RedirectToAction("LearnerInformation");
        }

        [HttpGet]
        public IActionResult LearnerInformation()
        {
            var data = GetDelegateRegistrationByCentreData()!;

            var model = new LearnerInformationViewModel(data, false);

            PopulateLearnerInformationExtraFields(model, data);

            return View(model);
        }

        [HttpPost]
        public IActionResult LearnerInformation(LearnerInformationViewModel model)
        {
            var data = GetDelegateRegistrationByCentreData()!;

            var centreId = User.GetCentreIdKnownNotNull();

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
            SetDelegateRegistrationByCentreData(data);

            return RedirectToAction("AddToGroup");
        }

        [Route("AddToGroup")]
        public IActionResult AddToGroup()
        {
            var data = GetDelegateRegistrationByCentreData();
            var centreId = User.GetCentreIdKnownNotNull();
            var groupSelect = groupsService.GetUnlinkedGroupsSelectListForCentre(centreId, data.ExistingGroupId);
            var model = new AddToGroupViewModel(data.AddToGroupOption, existingGroups: groupSelect, data.ExistingGroupId, data.NewGroupName, data.NewGroupDescription, 1, 0, 0, 0);
            return View(model);
        }

        [HttpPost]
        [Route("SubmitAddToGroup")]
        public IActionResult SubmitAddToGroup(AddToGroupViewModel model)
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var data = GetDelegateRegistrationByCentreData();
            if (model.AddToGroupOption == 2)
            {
                if (!string.IsNullOrEmpty(model.NewGroupName))
                {
                    if (groupsService.IsDelegateGroupExist(model.NewGroupName.Trim(), centreId))
                    {
                        ModelState.AddModelError(nameof(model.NewGroupName), "A group with the same name already exists (if it does not appear in the list of groups, it may be linked to a centre registration field)");
                    }
                }
            }
            if (!ModelState.IsValid)
            {
                var groupSelect = groupsService.GetUnlinkedGroupsSelectListForCentre(centreId, data.ExistingGroupId);
                model.ExistingGroups = groupSelect;
                model.RegisteringActiveDelegates = 1;
                model.UpdatingActiveDelegates = 0;
                model.RegisteringInactiveDelegates = 0;
                model.UpdatingInactiveDelegates = 0;
                return View("AddToGroup", model);
            }
            data.AddToGroupOption = model.AddToGroupOption;
            if (model.AddToGroupOption == 1)
            {
                data.ExistingGroupId = model.ExistingGroupId;
            }
            if (model.AddToGroupOption == 2)
            {
                data.NewGroupName = model.NewGroupName;
                data.NewGroupDescription = model.NewGroupDescription;
            }
            SetDelegateRegistrationByCentreData(data);
            return RedirectToAction("WelcomeEmail");
        }

        [HttpGet]
        public IActionResult WelcomeEmail()
        {
            var data = GetDelegateRegistrationByCentreData()!;

            var model = new WelcomeEmailViewModel(data, 1);

            return View(model);
        }

        [HttpPost]
        public IActionResult WelcomeEmail(WelcomeEmailViewModel model)
        {
            var data = GetDelegateRegistrationByCentreData()!;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            data.SetWelcomeEmail(model);
            SetDelegateRegistrationByCentreData(data);

            return RedirectToAction("Password");
        }

        [HttpGet]
        public IActionResult Password()
        {
            var model = new PasswordViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Password(PasswordViewModel model)
        {
            var data = GetDelegateRegistrationByCentreData()!;
            RegistrationPasswordValidator.ValidatePassword(model.Password, data.FirstName, data.LastName, ModelState);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            data.PasswordHash = model.Password != null ? cryptoService.GetPasswordHash(model.Password) : null;

            SetDelegateRegistrationByCentreData(data);

            return RedirectToAction("Summary");
        }

        [NoCaching]
        [HttpGet]
        public IActionResult Summary()
        {
            var data = GetDelegateRegistrationByCentreData()!;
            var centreId = User.GetCentreIdKnownNotNull();
            string? groupName = data.NewGroupName;
            if(data.AddToGroupOption == 1)
            {
                groupName = groupsService.GetGroupName(
                    (int)data.ExistingGroupId,
                    centreId
                    );
            }
            var jobGroup = jobGroupsService.GetJobGroupName((int)data.JobGroup);
            var registrationFieldGroups = groupsService.GetGroupsForRegistrationResponse(
                centreId,
                data.Answer1,
                data.Answer2,
                data.Answer3,
                jobGroup,
                data.Answer4,
                data.Answer5,
                data.Answer6
                );
            var viewModel = new SummaryViewModel(data);
            viewModel.GroupName = groupName;
            viewModel.RegistrationFieldGroups = registrationFieldGroups;
            PopulateSummaryExtraFields(viewModel, data);
            SetDelegateRegistrationByCentreData(data);
            return View(viewModel);
        }

        [NoCaching]
        [HttpPost]
        public IActionResult Summary(SummaryViewModel model)
        {
            var data = GetDelegateRegistrationByCentreData()!;
            var baseUrl = config.GetAppRootPath();

            try
            {
                var adminId = User.GetAdminIdKnownNotNull();
                var centreId = User.GetCentreIdKnownNotNull();
                if (data.AddToGroupOption == 2 && data.NewGroupName != null)
                {
                    data.ExistingGroupId = groupsService.AddDelegateGroup(centreId, data.NewGroupName, data.NewGroupDescription, adminId);
                    SetDelegateRegistrationByCentreData(data);
                }
                var candidateNumber = registrationService.RegisterDelegateByCentre(
                    RegistrationMappingHelper.MapCentreRegistrationToDelegateRegistrationModel(data),
                    baseUrl,
                    false,
                    adminId,
                    data.ExistingGroupId
                );

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
            var data = GetDelegateRegistrationByCentreData()!;
            var delegateNumber = (string?)TempData.Peek("delegateNumber");
            if (delegateNumber == null)
            {
                return RedirectToAction("Index");
            }

            var viewModel = new ConfirmationViewModel(delegateNumber, data.WelcomeEmailDate);
            TempData.Clear();
            return View(viewModel);
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
                jobGroupsService.GetJobGroupsAlphabetical(),
                model.JobGroup
            );
        }

        private void PopulateSummaryExtraFields(SummaryViewModel model, DelegateRegistrationData data)
        {
            model.JobGroup = jobGroupsService.GetJobGroupName((int)data.JobGroup!);
            model.DelegateRegistrationPrompts = GetCustomFieldsFromData(data);
        }

        private void SetupDelegateRegistrationByCentreData(int centreId)
        {
            TempData.Clear();
            multiPageFormService.ClearMultiPageFormData(MultiPageFormDataFeature.AddCustomWebForm("DelegateRegistrationByCentreCWF"), TempData);
            var delegateRegistrationByCentreData = new DelegateRegistrationByCentreData(centreId, clockUtility.UtcToday);
            SetDelegateRegistrationByCentreData(delegateRegistrationByCentreData);
        }
        private void SetDelegateRegistrationByCentreData(DelegateRegistrationByCentreData delegateRegistrationByCentreData)
        {
            multiPageFormService.SetMultiPageFormData(
                 delegateRegistrationByCentreData,
                 MultiPageFormDataFeature.AddCustomWebForm("DelegateRegistrationByCentreCWF"),
                 TempData
             );
        }

        private DelegateRegistrationByCentreData GetDelegateRegistrationByCentreData()
        {
            var data = multiPageFormService.GetMultiPageFormData<DelegateRegistrationByCentreData>(
               MultiPageFormDataFeature.AddCustomWebForm("DelegateRegistrationByCentreCWF"),
               TempData
           ).GetAwaiter().GetResult();
            return data;
        }
    }
}
