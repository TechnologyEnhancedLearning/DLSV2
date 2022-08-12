namespace DigitalLearningSolutions.Web.Controllers.Register
{
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement;

    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    [Authorize(Policy = CustomPolicies.BasicUser)]
    [ServiceFilter(typeof(VerifyUserHasVerifiedPrimaryEmail))]
    public class RegisterInternalAdminController : Controller
    {
        private readonly ICentresDataService centresDataService;
        private readonly ICentresService centresService;
        private readonly IDelegateApprovalsService delegateApprovalsService;
        private readonly IFeatureManager featureManager;
        private readonly IRegisterAdminService registerAdminService;
        private readonly IRegistrationService registrationService;
        private readonly IUserDataService userDataService;
        private readonly IUserService userService;

        public RegisterInternalAdminController(
            ICentresDataService centresDataService,
            ICentresService centresService,
            IUserDataService userDataService,
            IUserService userService,
            IRegistrationService registrationService,
            IDelegateApprovalsService delegateApprovalsService,
            IFeatureManager featureManager,
            IRegisterAdminService registerAdminService
        )
        {
            this.centresDataService = centresDataService;
            this.centresService = centresService;
            this.userDataService = userDataService;
            this.userService = userService;
            this.registrationService = registrationService;
            this.delegateApprovalsService = delegateApprovalsService;
            this.featureManager = featureManager;
            this.registerAdminService = registerAdminService;
        }

        [HttpGet]
        public IActionResult Index(int? centreId = null)
        {
            var centreName = centreId == null ? null : centresDataService.GetCentreName(centreId.Value);

            if (centreName == null)
            {
                return NotFound();
            }

            var userId = User.GetUserIdKnownNotNull();

            if (!registerAdminService.IsRegisterAdminAllowed(centreId.Value, userId))
            {
                return RedirectToAction("AccessDenied", "LearningSolutions");
            }

            var model = new InternalAdminInformationViewModel
            {
                Centre = centreId,
                CentreName = centreName,
                PrimaryEmail = User.GetUserPrimaryEmailKnownNotNull(),
                CentreSpecificEmail = userDataService.GetCentreEmail(User.GetUserIdKnownNotNull(), centreId.Value),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(InternalAdminInformationViewModel model)
        {
            var userId = User.GetUserIdKnownNotNull();

            if (!registerAdminService.IsRegisterAdminAllowed(model.Centre!.Value, userId))
            {
                return RedirectToAction("AccessDenied", "LearningSolutions");
            }

            RegistrationEmailValidator.ValidateCentreEmailWithUserIdIfNecessary(
                model.CentreSpecificEmail,
                model.Centre,
                userId,
                nameof(InternalAdminInformationViewModel.CentreSpecificEmail),
                ModelState,
                userDataService
            );

            RegistrationEmailValidator.ValidateEmailsForCentreManagerIfNecessary(
                model.PrimaryEmail,
                model.CentreSpecificEmail,
                model.Centre,
                nameof(InternalAdminInformationViewModel.CentreSpecificEmail),
                ModelState,
                centresService
            );

            if (!ModelState.IsValid)
            {
                model.CentreName = centresDataService.GetCentreName(model.Centre!.Value);
                model.PrimaryEmail = User.GetUserPrimaryEmailKnownNotNull();
                return View(model);
            }

            registrationService.CreateCentreManagerForExistingUser(
                userId,
                model.Centre!.Value,
                model.CentreSpecificEmail
            );

            var delegateAccount = userDataService.GetDelegateAccountsByUserId(userId)
                .SingleOrDefault(da => da.CentreId == model.Centre);

            if (delegateAccount == null)
            {
                registrationService.CreateDelegateAccountForExistingUser(
                    new InternalDelegateRegistrationModel(
                        model.Centre!.Value,
                        model.CentreSpecificEmail,
                        null,
                        null,
                        null,
                        null,
                        null,
                        null
                    ),
                    userId,
                    Request.GetUserIpAddressFromRequest(),
                    await featureManager.IsEnabledAsync("RefactoredTrackingSystem")
                );
            }
            else
            {
                if (!delegateAccount.Approved)
                {
                    delegateApprovalsService.ApproveDelegate(delegateAccount.Id, delegateAccount.CentreId);
                }
            }

            if (model.CentreSpecificEmail != null)
            {
                // TODO: 915 - Send verification link to centre email if not already verified
            }

            return RedirectToAction("Confirmation", new { centreId = model.Centre });
        }

        [HttpGet]
        public IActionResult Confirmation(int centreId)
        {
            var userId = User.GetUserIdKnownNotNull();
            var (_, unverifiedCentreEmails) = userService.GetUnverifiedEmailsForUser(userId);
            var (_, centreName, unverifiedCentreEmail) = unverifiedCentreEmails.SingleOrDefault(uce => uce.centreId == centreId);

            var model = new AdminConfirmationViewModel(
                null,
                unverifiedCentreEmail,
                centreName
            );

            return View(model);
        }
    }
}
