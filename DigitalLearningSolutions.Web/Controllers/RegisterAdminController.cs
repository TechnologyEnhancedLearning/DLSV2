namespace DigitalLearningSolutions.Web.Controllers
{
    using System;
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

    public class RegisterAdminController : Controller
    {
        private const string CookieName = "RegistrationData";
        private readonly ICentresDataService centresDataService;
        private readonly ICryptoService cryptoService;
        private readonly IJobGroupsDataService jobGroupsDataService;
        private readonly IUserDataService userDataService;

        public RegisterAdminController(
            ICentresDataService centresDataService,
            ICryptoService cryptoService,
            IJobGroupsDataService jobGroupsDataService,
            IUserDataService userDataService
        )
        {
            this.centresDataService = centresDataService;
            this.cryptoService = cryptoService;
            this.jobGroupsDataService = jobGroupsDataService;
            this.userDataService = userDataService;
        }

        public IActionResult Index(int? centreId = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!centreId.HasValue || !CheckRegisterAdminAllowed(centreId.Value))
            {
                return NotFound();
            }

            SetAdminRegistrationData(centreId.Value);

            return RedirectToAction("PersonalInformation");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<RegistrationData>))]
        [HttpGet]
        public IActionResult PersonalInformation()
        {
            var data = TempData.Peek<RegistrationData>()!;

            var model = RegistrationMappingHelper.MapDataToPersonalInformation(data);
            PopulatePersonalInformationExtraFields(model);

            ValidateEmailAddress(model.Email, model.Centre!.Value);

            return View(model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<RegistrationData>))]
        [HttpPost]
        public IActionResult PersonalInformation(PersonalInformationViewModel model)
        {
            var data = TempData.Peek<RegistrationData>()!;

            ValidateEmailAddress(model.Email, model.Centre!.Value);

            if (!ModelState.IsValid)
            {
                PopulatePersonalInformationExtraFields(model);
                return View(model);
            }

            data.SetPersonalInformation(model);
            TempData.Set(data);

            return RedirectToAction("LearnerInformation");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<RegistrationData>))]
        [HttpGet]
        public IActionResult LearnerInformation()
        {
            var data = TempData.Peek<RegistrationData>()!;

            var model = RegistrationMappingHelper.MapDataToLearnerInformation(data);
            PopulateLearnerInformationExtraFields(model);

            return View(model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<RegistrationData>))]
        [HttpPost]
        public IActionResult LearnerInformation(LearnerInformationViewModel model)
        {
            var data = TempData.Peek<RegistrationData>()!;

            if (!ModelState.IsValid)
            {
                PopulateLearnerInformationExtraFields(model);
                return View(model);
            }

            data.SetLearnerInformation(model);
            TempData.Set(data);

            return RedirectToAction("Password");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<RegistrationData>))]
        [HttpGet]
        public IActionResult Password()
        {
            return View(new PasswordViewModel());
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<RegistrationData>))]
        [HttpPost]
        public IActionResult Password(PasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var data = TempData.Peek<RegistrationData>()!;
            data.PasswordHash = cryptoService.GetPasswordHash(model.Password!);
            TempData.Set(data);

            return RedirectToAction("Summary");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<RegistrationData>))]
        [HttpGet]
        public IActionResult Summary()
        {
            var data = TempData.Peek<RegistrationData>()!;
            var model = RegistrationMappingHelper.MapDataToSummary(data);
            PopulateSummaryExtraFields(model, data);
            return View(model);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<RegistrationData>))]
        [HttpPost]
        public IActionResult Summary(SummaryViewModel model)
        {
            var data = TempData.Peek<RegistrationData>()!;

            if (!ModelState.IsValid)
            {
                var viewModel = RegistrationMappingHelper.MapDataToSummary(data);
                PopulateSummaryExtraFields(viewModel, data);
                viewModel.Terms = model.Terms;
                return View(viewModel);
            }

            // TODO: (HEEDLS-527) register admin details and notification preferences in database

            return RedirectToAction("Confirmation");
        }

        [HttpGet]
        public IActionResult Confirmation()
        {
            return View();
        }

        private bool CheckRegisterAdminAllowed(int centreId)
        {
            if (centresDataService.GetCentreName(centreId) == null)
            {
                return false;
            }

            var (autoRegistered, autoRegisterManagerEmail) = centresDataService.GetCentreAutoRegisterValues(centreId);
            return !autoRegistered && !string.IsNullOrWhiteSpace(autoRegisterManagerEmail);
        }

        private void SetAdminRegistrationData(int centreId)
        {
            var adminRegistrationData = new RegistrationData
            {
                Centre = centreId
            };
            var id = adminRegistrationData.Id;

            Response.Cookies.Append(
                CookieName,
                id.ToString(),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(30)
                }
            );

            TempData.Clear();
            TempData.Set(adminRegistrationData);
        }

        private void ValidateEmailAddress(string? email, int centreId)
        {
            if (email == null)
            {
                return;
            }

            var (_, autoRegisterManagerEmail) = centresDataService.GetCentreAutoRegisterValues(centreId);
            if (!email.Equals(autoRegisterManagerEmail))
            {
                ModelState.AddModelError(
                    nameof(PersonalInformationViewModel.Email),
                    "This email address does not match the one held by the centre"
                );
            }

            var adminUser = userDataService.GetAdminUserByEmailAddress(email);

            if (adminUser != null)
            {
                ModelState.AddModelError(
                    nameof(PersonalInformationViewModel.Email),
                    "An admin user with this email address is already registered"
                );
            }
        }

        private void PopulatePersonalInformationExtraFields(PersonalInformationViewModel model)
        {
            model.CentreName = centresDataService.GetCentreName(model.Centre!.Value);
        }

        private void PopulateLearnerInformationExtraFields(
            LearnerInformationViewModel model
        )
        {
            model.JobGroupOptions = SelectListHelper.MapOptionsToSelectListItems(
                jobGroupsDataService.GetJobGroupsAlphabetical(),
                model.JobGroup
            );
        }

        private void PopulateSummaryExtraFields(SummaryViewModel model, RegistrationData data)
        {
            model.Centre = centresDataService.GetCentreName((int)data.Centre!);
            model.JobGroup = jobGroupsDataService.GetJobGroupName((int)data.JobGroup!);
        }
    }
}
