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
    using DigitalLearningSolutions.Web.ViewModels.RegisterDelegateByCentre;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using PasswordViewModel = ViewModels.RegisterDelegateByCentre.PasswordViewModel;

    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("/TrackingSystem/Delegates/Register/{action}")]
    public class RegisterDelegateByCentreController : Controller
    {
        private const string CookieName = "DelegateRegistrationByCentreData";
        private readonly CustomPromptHelper customPromptHelper;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IUserService userService;
        private readonly ICryptoService cryptoService;

        public RegisterDelegateByCentreController(
            IJobGroupsDataService jobGroupsDataService,
            IUserService userService,
            CustomPromptHelper customPromptHelper,
            ICryptoService cryptoService
        )
        {
            this.jobGroupsDataService = jobGroupsDataService;
            this.userService = userService;
            this.customPromptHelper = customPromptHelper;
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

            ValidateEmailAddress(model);

            return View(model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationByCentreData>))]
        [HttpPost]
        public IActionResult PersonalInformation(PersonalInformationViewModel model)
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

            if (!ValidateWelcomeEmail(model))
            {
                return View(model);
            }

            data.SetWelcomeEmail(model);
            TempData.Set(data);

            if (!data.ShouldSendEmail)
            {
                return RedirectToAction("Password");
            }

            return View(model);
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
            data.PasswordHash = cryptoService.GetPasswordHash(model.Password!);
            TempData.Set(data);

            return View(model);
        }

        private void SetCentreDelegateRegistrationData(int centreId)
        {
            var centreDelegateRegistrationData = new DelegateRegistrationByCentreData
            {
                IsCentreSpecificRegistration = true,
                Centre = centreId
            };
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

        private void ValidateEmailAddress(PersonalInformationViewModel model)
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
        }

        private bool ValidateWelcomeEmail(WelcomeEmailViewModel model)
        {
            if (!model.ShouldSendEmail)
            {
                model.Day = null;
                model.Month = null;
                model.Year = null;
                return true;
            }

            var validationResult = DateValidator.ValidateRequiredDate(model.Day, model.Month, model.Year, "Email delivery date");
            model.DateValidationResult = validationResult;
            return validationResult.DateValid;
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
    }
}
