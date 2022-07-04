namespace DigitalLearningSolutions.Web.Controllers.Register
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    [Authorize(Policy = CustomPolicies.BasicUser)]
    public class RegisterInternalAdminController : Controller
    {
        private readonly ICentresDataService centresDataService;
        private readonly ICentresService centresService;
        private readonly IUserDataService userDataService;
        private readonly RegisterAdminHelper registerAdminHelper;

        public RegisterInternalAdminController(
            ICentresDataService centresDataService,
            ICentresService centresService,
            IUserDataService userDataService,
            RegisterAdminHelper registerAdminHelper
        )
        {
            this.centresDataService = centresDataService;
            this.centresService = centresService;
            this.userDataService = userDataService;
            this.registerAdminHelper = registerAdminHelper;
        }

        [HttpGet]
        public IActionResult Index(int? centreId = null)
        {
            if (!centreId.HasValue || centresDataService.GetCentreName(centreId.Value) == null)
            {
                return NotFound();
            }

            if (!registerAdminHelper.IsRegisterAdminAllowed(centreId.Value))
            {
                return RedirectToAction("AccessDenied", "LearningSolutions");
            }

            var model = new PersonalInformationViewModel
            {
                Centre = centreId,
                CentreName = centresDataService.GetCentreName(centreId.Value),
                PrimaryEmail = User.GetUserPrimaryEmailKnownNotNull(),
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(PersonalInformationViewModel model)
        {
            if (!CanProceedWithRegistration(model))
            {
                return new StatusCodeResult(500);
            }

            return RedirectToAction("Confirmation");
        }

        [HttpGet]
        public IActionResult Confirmation()
        {
            return View();
        }

        private bool CanProceedWithRegistration(PersonalInformationViewModel model)
        {
            return model.Centre.HasValue &&
                   registerAdminHelper.IsRegisterAdminAllowed(model.Centre.Value) &&
                   (model.CentreSpecificEmail != null &&
                    centresService.DoesEmailMatchCentre(model.CentreSpecificEmail, model.Centre.Value) ||
                    centresService.DoesEmailMatchCentre(model.PrimaryEmail!, model.Centre.Value)) &&
                   (model.CentreSpecificEmail == null || !userDataService.EmailIsInUseByOtherUser(
                       User.GetUserIdKnownNotNull(),
                       model.CentreSpecificEmail
                   ));
        }
    }
}
