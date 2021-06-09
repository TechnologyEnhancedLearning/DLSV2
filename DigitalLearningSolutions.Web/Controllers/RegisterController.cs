namespace DigitalLearningSolutions.Web.Controllers
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.Common;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class RegisterController : Controller
    {
        private const string CookieName = "RegistrationData";
        private readonly ICentresDataService centresDataService;
        private readonly ICryptoService cryptoService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IRegistrationService registrationService;
        private readonly IUserService userService;
        private readonly CustomPromptHelper customPromptHelper;

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

            var delegateRegistrationData = TempData.Peek<DelegateRegistrationData>();

            if (delegateRegistrationData == null || !Request.Cookies.ContainsKey(CookieName))
            {
                delegateRegistrationData = new DelegateRegistrationData();
                var id = delegateRegistrationData.Id;

                Response.Cookies.Append(
                    CookieName,
                    id.ToString(),
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(30)
                    }
                );

                TempData.Set(delegateRegistrationData);
            }

            // if no centreId param, then use general registration process
            if (centreId == null)
            {
                delegateRegistrationData.RegisterViewModel.IsCentreSpecific = false;
                TempData.Set(delegateRegistrationData);
            }
            else
            {
                var centreName = centresDataService.GetCentreName(centreId.Value);
                // if centreId invalid, then clear centre, redirect to general registration process
                if (centreName == null)
                {
                    delegateRegistrationData.RegisterViewModel.Centre = null;
                    delegateRegistrationData.RegisterViewModel.IsCentreSpecific = false;
                    TempData.Set(delegateRegistrationData);
                    return RedirectToAction("Index");
                }
                // otherwise use a centre-specific registration process
                // note: do not store the centre-specific properties until user clicks next
                ViewBag.CentreName = centreName;
                delegateRegistrationData.RegisterViewModel.Centre = centreId;
                delegateRegistrationData.RegisterViewModel.IsCentreSpecific = true;
            }

            ViewBag.CentreOptions = SelectListHelper.MapOptionsToSelectListItems(
                centresDataService.GetActiveCentresAlphabetical(),
                delegateRegistrationData.RegisterViewModel.Centre
            );

            // Check this email and centre combination doesn't already exist in case we were redirected
            // back here by the user trying to submit the final page of the form
            ValidateEmailAddress(delegateRegistrationData.RegisterViewModel);

            return View(delegateRegistrationData.RegisterViewModel);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpPost]
        public IActionResult Index(RegisterViewModel model)
        {
            ValidateEmailAddress(model);

            if (!ModelState.IsValid)
            {
                ViewBag.CentreOptions = SelectListHelper.MapOptionsToSelectListItems(
                    centresDataService.GetActiveCentresAlphabetical(),
                    model.Centre
                );
                return View(model);
            }

            var data = TempData.Peek<DelegateRegistrationData>()!;

            if (data.RegisterViewModel.Centre != model.Centre)
            {
                ClearCustomPromptAnswers(data.LearnerInformationViewModel);
            }

            data.RegisterViewModel = model;
            TempData.Set(data);

            return RedirectToAction("LearnerInformation");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpGet]
        public IActionResult LearnerInformation()
        {
            var data = TempData.Peek<DelegateRegistrationData>()!;
            var model = data.LearnerInformationViewModel;

            if (data.RegisterViewModel.Centre == null)
            {
                return RedirectToAction("Index");
            }

            var centreId = (int)data.RegisterViewModel.Centre;

            SetLearnerInformationViewBag(model, centreId);

            return View(model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpPost]
        public IActionResult LearnerInformation(LearnerInformationViewModel model)
        {
            var data = TempData.Peek<DelegateRegistrationData>()!;

            if (data.RegisterViewModel.Centre == null)
            {
                return RedirectToAction("Index");
            }

            var centreId = (int)data.RegisterViewModel.Centre;

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
                SetLearnerInformationViewBag(model, centreId);
                return View(model);
            }

            data.LearnerInformationViewModel = model;
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
            var centre = centresDataService.GetCentreName((int)data.RegisterViewModel.Centre!);
            var jobGroup = jobGroupsDataService.GetJobGroupName((int)data.LearnerInformationViewModel.JobGroup!);
            var viewModel = RegistrationMappingHelper.MapToSummary(data, centre!, jobGroup!);
            AddCustomFieldsToViewBag(data.LearnerInformationViewModel, (int)data.RegisterViewModel.Centre!);

            return View(viewModel);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpPost]
        public IActionResult Summary(SummaryViewModel model)
        {
            var data = TempData.Peek<DelegateRegistrationData>()!;

            if (data.RegisterViewModel.Centre == null || data.LearnerInformationViewModel.JobGroup == null)
            {
                return RedirectToAction("Index");
            }

            var centreId = (int)data.RegisterViewModel.Centre;
            var jobGroupId = (int)data.LearnerInformationViewModel.JobGroup;

            if (!ModelState.IsValid)
            {
                var centre = centresDataService.GetCentreName(centreId);
                var jobGroup = jobGroupsDataService.GetJobGroupName(jobGroupId);
                var viewModel = RegistrationMappingHelper.MapToSummary(data, centre!, jobGroup!);
                viewModel.Terms = model.Terms;
                AddCustomFieldsToViewBag(data.LearnerInformationViewModel, centreId);
                return View(viewModel);
            }

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

        private void SetLearnerInformationViewBag(LearnerInformationViewModel model, int centreId)
        {
            AddCustomFieldsToViewBag(model, centreId);
            ViewBag.JobGroupOptions = SelectListHelper.MapOptionsToSelectListItems(
                jobGroupsDataService.GetJobGroupsAlphabetical(),
                model.JobGroup
            );
        }

        private void AddCustomFieldsToViewBag(LearnerInformationViewModel model, int centreId)
        {
            var customFields = customPromptHelper.GetCustomFieldViewModelsForCentre(
                centreId,
                model.Answer1,
                model.Answer2,
                model.Answer3,
                model.Answer4,
                model.Answer5,
                model.Answer6
            );
            ViewBag.CustomFields = customFields;
        }

        private static void ClearCustomPromptAnswers(LearnerInformationViewModel model)
        {
            model.Answer1 = null;
            model.Answer2 = null;
            model.Answer3 = null;
            model.Answer4 = null;
            model.Answer5 = null;
            model.Answer6 = null;
        }
    }
}
