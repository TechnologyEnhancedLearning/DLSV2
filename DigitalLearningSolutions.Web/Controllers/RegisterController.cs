namespace DigitalLearningSolutions.Web.Controllers
{
    using System;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Extensions;
    using DigitalLearningSolutions.Web.Models;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class RegisterController : Controller
    {
        private const string CookieName = "RegistrationData";
        private readonly ICentresService centresService;
        private readonly IJobGroupsService jobGroupsService;

        public RegisterController(ICentresService centresService, IJobGroupsService jobGroupsService)
        {
            this.centresService = centresService;
            this.jobGroupsService = jobGroupsService;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            var delegateRegistrationData = TempData.Get<DelegateRegistrationData>();

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
                    });
                TempData.Set(delegateRegistrationData);

                return View();
            }

            return View(delegateRegistrationData.RegisterViewModel);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpPost]
        public IActionResult Index(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var data = TempData.Get<DelegateRegistrationData>();
            data!.RegisterViewModel = model;
            TempData.Set(data);

            return RedirectToAction("LearnerInformation");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpGet]
        public IActionResult LearnerInformation()
        {
            var data = TempData.Get<DelegateRegistrationData>();
            var viewModel = data!.LearnerInformationViewModel;
            ViewBag.Centres = centresService.GetActiveCentres();
            ViewBag.JobGroups = jobGroupsService.GetJobGroups();

            return View(viewModel);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpPost]
        public IActionResult LearnerInformation(LearnerInformationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Centres = centresService.GetActiveCentres();
                ViewBag.JobGroups = jobGroupsService.GetJobGroups();
                return View(model);
            }

            var data = TempData.Get<DelegateRegistrationData>();
            data!.LearnerInformationViewModel = model;
            TempData.Set(data);

            return RedirectToAction("Password");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpGet]
        public IActionResult Password()
        {
            var data = TempData.Get<DelegateRegistrationData>();
            var viewModel = data!.PasswordViewModel;

            return View(viewModel);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpPost]
        public IActionResult Password(PasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var data = TempData.Get<DelegateRegistrationData>();
            data!.PasswordViewModel = model;
            TempData.Set(data);

            return RedirectToAction("Summary");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpGet]
        public IActionResult Summary()
        {
            var data = TempData.Get<DelegateRegistrationData>();
            var viewModel = MapToSummary(data!);

            return View(viewModel);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [HttpPost]
        public IActionResult Summary(SummaryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var data = TempData.Get<DelegateRegistrationData>();
                var viewModel = MapToSummary(data!);
                viewModel.Terms = model.Terms;
                return View(viewModel);
            }

            // TODO HEEDLS-396 create new user and redirect to confirmation page
            return View(model);
        }

        private SummaryViewModel MapToSummary(DelegateRegistrationData data)
        {
            var centre = centresService.GetCentreName((int)data.LearnerInformationViewModel.Centre!);
            var jobGroup = jobGroupsService.GetJobGroupName((int)data.LearnerInformationViewModel.JobGroup!);
            return new SummaryViewModel
            {
                FirstName = data.RegisterViewModel.FirstName,
                LastName = data.RegisterViewModel.LastName,
                Email = data.RegisterViewModel.Email,
                Centre = centre,
                JobGroup = jobGroup
            };
        }
    }
}
