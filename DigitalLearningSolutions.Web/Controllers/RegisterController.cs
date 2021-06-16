namespace DigitalLearningSolutions.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.MyAccount;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class RegisterController : Controller
    {
        private const string CookieName = "RegistrationData";
        private readonly ICentresDataService centresDataService;
        private readonly ICryptoService cryptoService;
        private readonly CustomPromptHelper customPromptHelper;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IRegistrationService registrationService;
        private readonly IUserService userService;

        public RegisterController(
            ICentresDataService centresDataService,
            IJobGroupsDataService jobGroupsDataService,
            IRegistrationService registrationService,
            ICryptoService cryptoService,
            IUserService userService,
            CustomPromptHelper customPromptHelper
        )
        {
            this.centresDataService = centresDataService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.registrationService = registrationService;
            this.cryptoService = cryptoService;
            this.userService = userService;
            this.customPromptHelper = customPromptHelper;
        }

        public IActionResult Index(int? centreId)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!CheckCentreIdValid(centreId))
            {
                return NotFound();
            }

            var delegateRegistrationData = CreateDelegateRegistrationData(centreId);
            delegateRegistrationData.IsCentreSpecificRegistration = centreId.HasValue;
            delegateRegistrationData.Centre = centreId;
            TempData.Set(delegateRegistrationData);

            return RedirectToAction("PersonalInformation");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpGet]
        public IActionResult PersonalInformation()
        {
            var data = TempData.Peek<DelegateRegistrationData>()!;

            var model = RegistrationMappingHelper.MapDataToRegisterViewModel(data);
            PopulatePersonalInformationExtraFields(model);

            // Check this email and centre combination doesn't already exist in case we were redirected
            // back here by the user trying to submit the final page of the form
            ValidateEmailAddress(model);

            return View(model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpPost]
        public IActionResult PersonalInformation(RegisterViewModel model)
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
                ClearCustomPromptAnswers(data);
            }

            data = RegistrationMappingHelper.MapRegisterViewModelToData(model, data);
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

            var model = RegistrationMappingHelper.MapDataToLearnerInformationViewModel(data);
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

            customPromptHelper.ValidateCustomPrompts(
                centreId,
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

            data = RegistrationMappingHelper.MapLearnerInformationViewModelToData(model, data);
            TempData.Set(data);

            return RedirectToAction("Password");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpGet]
        public IActionResult Password()
        {
            return View();
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpPost]
        public IActionResult Password(PasswordViewModel model)
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

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpGet]
        public IActionResult Summary()
        {
            var data = TempData.Peek<DelegateRegistrationData>()!;
            var viewModel = RegistrationMappingHelper.MapDataToSummaryViewModel(data);
            PopulateSummaryExtraFields(viewModel, data);
            return View(viewModel);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpPost]
        public IActionResult Summary(SummaryViewModel model)
        {
            var data = TempData.Peek<DelegateRegistrationData>()!;

            if (data.Centre == null || data.JobGroup == null)
            {
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                var viewModel = RegistrationMappingHelper.MapDataToSummaryViewModel(data);
                PopulateSummaryExtraFields(viewModel, data);
                viewModel.Terms = model.Terms;
                return View(viewModel);
            }

            var centreId = (int)data.Centre;
            var baseUrl = ConfigHelper.GetAppConfig()["CurrentSystemBaseUrl"];
            var userIp = Request.GetUserIpAddressFromRequest();
            var (candidateNumber, approved) =
                registrationService.RegisterDelegate(
                    RegistrationMappingHelper.MapToDelegateRegistrationModel(data),
                    baseUrl,
                    userIp
                );

            if (candidateNumber == "-1")
            {
                return RedirectToAction("Error", "LearningSolutions");
            }

            if (candidateNumber == "-4")
            {
                return RedirectToAction("Index");
            }

            TempData.Clear();
            TempData.Add("candidateNumber", candidateNumber);
            TempData.Add("approved", approved);
            TempData.Add("centreId", centreId);
            return RedirectToAction("Confirmation");
        }

        [HttpGet]
        public IActionResult Confirmation()
        {
            var candidateNumber = (string?)TempData.Peek("candidateNumber");
            var approvedNullable = (bool?)TempData.Peek("approved");
            var centreIdNullable = (int?)TempData.Peek("centreId");
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

        private DelegateRegistrationData CreateDelegateRegistrationData(int? centreId)
        {
            var delegateRegistrationData = new DelegateRegistrationData
            {
                IsCentreSpecificRegistration = centreId.HasValue,
                Centre = centreId
            };
            var id = delegateRegistrationData.Id;

            Response.Cookies.Append(
                CookieName,
                id.ToString(),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(30)
                }
            );

            return delegateRegistrationData;
        }

        private bool CheckCentreIdValid(int? centreId)
        {
            if (centreId == null)
            {
                return true;
            }

            return centresDataService.GetCentreName(centreId.Value) != null;
        }

        private void ValidateEmailAddress(RegisterViewModel model)
        {
            if (model.Email == null)
            {
                return;
            }

            var duplicateUsers = userService.GetUsersByEmailAddress(model.Email).delegateUsers
                .Where(u => u.CentreId == model.Centre);

            if (duplicateUsers.Count() != 0)
            {
                ModelState.AddModelError(
                    nameof(RegisterViewModel.Email),
                    "A user with this email address already exists at this centre"
                );
            }
        }

        private List<EditCustomFieldViewModel> GetCustomFieldsFromModel(
            DelegateRegistrationData data,
            LearnerInformationViewModel model
        )
        {
            var customFields = customPromptHelper.GetCustomFieldViewModelsForCentre(
                data.Centre!.Value,
                model.Answer1,
                model.Answer2,
                model.Answer3,
                model.Answer4,
                model.Answer5,
                model.Answer6
            );
            return customFields;
        }

        private List<EditCustomFieldViewModel> GetCustomFieldsFromData(DelegateRegistrationData data)
        {
            var customFields = customPromptHelper.GetCustomFieldViewModelsForCentre(
                data.Centre!.Value,
                data.Answer1,
                data.Answer2,
                data.Answer3,
                data.Answer4,
                data.Answer5,
                data.Answer6
            );
            return customFields;
        }

        private static void ClearCustomPromptAnswers(DelegateRegistrationData data)
        {
            data.Answer1 = null;
            data.Answer2 = null;
            data.Answer3 = null;
            data.Answer4 = null;
            data.Answer5 = null;
            data.Answer6 = null;
        }

        private void PopulatePersonalInformationExtraFields(RegisterViewModel model)
        {
            model.CentreName = model.Centre.HasValue ? centresDataService.GetCentreName(model.Centre.Value) : null;
            model.CentreOptions = SelectListHelper.MapOptionsToSelectListItems(
                centresDataService.GetActiveCentresAlphabetical(),
                model.Centre
            );
        }

        private void PopulateLearnerInformationExtraFields(
            LearnerInformationViewModel model,
            DelegateRegistrationData data
        )
        {
            model.CustomFields = GetCustomFieldsFromModel(data, model);
            model.JobGroupOptions = SelectListHelper.MapOptionsToSelectListItems(
                jobGroupsDataService.GetJobGroupsAlphabetical(),
                model.JobGroup
            );
        }

        private void PopulateSummaryExtraFields(SummaryViewModel model, DelegateRegistrationData data)
        {
            model.Centre = centresDataService.GetCentreName((int)data.Centre!);
            model.JobGroup = jobGroupsDataService.GetJobGroupName((int)data.JobGroup!);
            model.CustomFields = GetCustomFieldsFromData(data);
        }
    }
}
