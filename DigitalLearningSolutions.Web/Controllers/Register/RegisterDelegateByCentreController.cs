namespace DigitalLearningSolutions.Web.Controllers.Register
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.ControllerHelpers;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using DigitalLearningSolutions.Web.ViewModels.Register.RegisterDelegateByCentre;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;
    using SummaryViewModel = DigitalLearningSolutions.Web.ViewModels.Register.RegisterDelegateByCentre.SummaryViewModel;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("/TrackingSystem/Delegates/Register/{action}")]
    public class RegisterDelegateByCentreController : Controller
    {
        private const string CookieName = "DelegateRegistrationByCentreData";
        private readonly ICryptoService cryptoService;
        private readonly CustomPromptHelper customPromptHelper;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IUserDataService userDataService;
        private readonly IUserService userService;

        public RegisterDelegateByCentreController(
            IJobGroupsDataService jobGroupsDataService,
            IUserService userService,
            CustomPromptHelper customPromptHelper,
            ICryptoService cryptoService,
            IUserDataService userDataService
        )
        {
            this.jobGroupsDataService = jobGroupsDataService;
            this.userService = userService;
            this.customPromptHelper = customPromptHelper;
            this.userDataService = userDataService;
            this.cryptoService = cryptoService;
        }

        [Route("/TrackingSystem/Delegates/Register")]
        public IActionResult Index()
        {
            var centreId = User.GetCentreId();

            SetCentreDelegateRegistrationData(centreId);

            return RedirectToAction("PersonalInformation");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationByCentreData>))]
        [HttpGet]
        public IActionResult PersonalInformation()
        {
            var data = TempData.Peek<DelegateRegistrationByCentreData>()!;

            var model = new PersonalInformationViewModel(data);

            ValidatePersonalInformation(model);

            return View(model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationByCentreData>))]
        [HttpPost]
        public IActionResult PersonalInformation(PersonalInformationViewModel model)
        {
            var data = TempData.Peek<DelegateRegistrationByCentreData>()!;

            ValidatePersonalInformation(model);

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

            var model = new LearnerInformationViewModel(data);

            PopulateLearnerInformationExtraFields(model, data);

            return View(model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationByCentreData>))]
        [HttpPost]
        public IActionResult LearnerInformation(LearnerInformationViewModel model)
        {
            var data = TempData.Peek<DelegateRegistrationByCentreData>()!;

            var centreId = data.Centre!.Value;

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

            model.CleanDate();
            SetWelcomeEmailValidationResult(model);
            if (model.DateValidationResult is { DateValid: false })
            {
                return View(model);
            }

            data.SetWelcomeEmail(model);
            TempData.Set(data);

            return RedirectToAction(data.ShouldSendEmail ? "Summary" : "Password");
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
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var data = TempData.Peek<DelegateRegistrationByCentreData>()!;

            data.PasswordHash = model.Password != null ? cryptoService.GetPasswordHash(model.Password) : null;

            TempData.Set(data);

            return RedirectToAction("Summary");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationByCentreData>))]
        [HttpGet]
        public IActionResult Summary()
        {
            var data = TempData.Peek<DelegateRegistrationByCentreData>()!;
            var viewModel = new SummaryViewModel(data);
            PopulateSummaryExtraFields(viewModel, data);
            return View(viewModel);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationByCentreData>))]
        [HttpPost]
        public IActionResult Summary(SummaryViewModel model)
        {
            return new OkResult();
        }

        private void SetCentreDelegateRegistrationData(int centreId)
        {
            var centreDelegateRegistrationData = new DelegateRegistrationByCentreData(centreId);
            var id = centreDelegateRegistrationData.Id;

            Response.Cookies.Append(
                CookieName,
                id.ToString(),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(30)
                }
            );

            TempData.Set(centreDelegateRegistrationData);
        }

        private void ValidatePersonalInformation(PersonalInformationViewModel model)
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
                    nameof(PersonalInformationViewModel.Email),
                    "A user with this email address is already registered at this centre"
                );
            }

            if (model.Alias == null)
            {
                return;
            }

            duplicateUsers = userDataService.GetAllDelegateUsersByUsername(model.Alias)
                .Where(u => u.CentreId == model.Centre);

            if (duplicateUsers.Count() != 0)
            {
                ModelState.AddModelError(
                    nameof(PersonalInformationViewModel.Alias),
                    "A user with this alias is already registered at this centre"
                );
            }
        }

        private void SetWelcomeEmailValidationResult(WelcomeEmailViewModel model)
        {
            if (!model.ShouldSendEmail)
            {
                return;
            }

            var validationResult = DateValidator.ValidateRequiredDate(
                model.Day,
                model.Month,
                model.Year,
                "Email delivery date"
            );
            model.DateValidationResult = validationResult;
        }

        private IEnumerable<EditCustomFieldViewModel> GetEditCustomFieldsFromModel(
            LearnerInformationViewModel model,
            int centreId
        )
        {
            return customPromptHelper.GetEditCustomFieldViewModelsForCentre(
                centreId,
                model.Answer1,
                model.Answer2,
                model.Answer3,
                model.Answer4,
                model.Answer5,
                model.Answer6
            );
        }

        private IEnumerable<CustomFieldViewModel> GetCustomFieldsFromData(DelegateRegistrationData data)
        {
            return customPromptHelper.GetCustomFieldViewModelsForCentre(
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
            model.CustomFields = GetEditCustomFieldsFromModel(model, data.Centre!.Value);
            model.JobGroupOptions = SelectListHelper.MapOptionsToSelectListItems(
                jobGroupsDataService.GetJobGroupsAlphabetical(),
                model.JobGroup
            );
        }

        private void PopulateSummaryExtraFields(SummaryViewModel model, DelegateRegistrationData data)
        {
            model.JobGroup = jobGroupsDataService.GetJobGroupName((int)data.JobGroup!);
            model.CustomFields = GetCustomFieldsFromData(data);
        }
    }
}
