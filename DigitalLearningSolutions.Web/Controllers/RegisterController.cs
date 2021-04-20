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
                var id = Guid.NewGuid();
                delegateRegistrationData = new DelegateRegistrationData { Id = id };

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

            return View("Index", delegateRegistrationData.RegisterViewModel);
        }

        [HttpPost]
        public IActionResult Index(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var data = TempData.Get<DelegateRegistrationData>();
            data.RegisterViewModel = model;
            TempData.Set(data);

            return RedirectToAction("LearnerInformation");
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [Route("Register/learner-information")]
        [HttpGet]
        public IActionResult LearnerInformation()
        {
            var data = TempData.Get<DelegateRegistrationData>();
            var viewModel = data.LearnerInformationViewModel;
            ViewBag.Centres = centresService.GetActiveCentres();
            ViewBag.JobGroups = jobGroupsService.GetJobGroups();

            return View(viewModel);
        }

        [ServiceFilter(typeof(RedirectEmptySessionData<DelegateRegistrationData>))]
        [Route("Register/learner-information")]
        [HttpPost]
        public IActionResult LearnerInformation(LearnerInformationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // QQ is this line necessary
                ViewBag.Centres = centresService.GetActiveCentres();
                ViewBag.JobGroups = jobGroupsService.GetJobGroups();
                return View("LearnerInformation", model);
            }

            var data = TempData.Get<DelegateRegistrationData>();
            data.LearnerInformationViewModel = model;
            TempData.Set(data);

            // QQ link to password page
            return View();
        }
    }
}
