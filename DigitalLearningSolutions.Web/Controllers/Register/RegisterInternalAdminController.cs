namespace DigitalLearningSolutions.Web.Controllers.Register
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models.Register;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Register;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement;

    [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
    [Authorize(Policy = CustomPolicies.BasicUser)]
    [ServiceFilter(typeof(VerifyUserHasVerifiedPrimaryEmail))]
    public class RegisterInternalAdminController : Controller
    {
        private readonly ICentresService centresService;
        private readonly IConfiguration config;
        private readonly IDelegateApprovalsService delegateApprovalsService;
        private readonly IEmailVerificationService emailVerificationService;
        private readonly IFeatureManager featureManager;
        private readonly IRegisterAdminService registerAdminService;
        private readonly IRegistrationService registrationService;
        private readonly IUserService userService;

        public RegisterInternalAdminController(
            ICentresService centresService,
            IUserService userService,
            IRegistrationService registrationService,
            IDelegateApprovalsService delegateApprovalsService,
            IFeatureManager featureManager,
            IRegisterAdminService registerAdminService,
            IEmailVerificationService emailVerificationService,
            IConfiguration config
        )
        {
            this.centresService = centresService;
            this.userService = userService;
            this.registrationService = registrationService;
            this.delegateApprovalsService = delegateApprovalsService;
            this.featureManager = featureManager;
            this.registerAdminService = registerAdminService;
            this.emailVerificationService = emailVerificationService;
            this.config = config;
        }

        [HttpGet]
        public IActionResult Index(int? centreId = null)
        {
            var centreName = centreId == null ? null : centresService.GetCentreName(centreId.Value);

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
                CentreSpecificEmail = userService.GetCentreEmail(User.GetUserIdKnownNotNull(), centreId.Value),
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
                userService
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
                model.CentreName = centresService.GetCentreName(model.Centre!.Value);
                model.PrimaryEmail = User.GetUserPrimaryEmailKnownNotNull();
                return View(model);
            }

            registrationService.CreateCentreManagerForExistingUser(
                userId,
                model.Centre!.Value,
                model.CentreSpecificEmail
            );

            var delegateAccount = userService.GetDelegateAccountsByUserId(userId)
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

            if (model.CentreSpecificEmail != null &&
                !emailVerificationService.AccountEmailIsVerifiedForUser(userId, model.CentreSpecificEmail))
            {
                var userAccount = userService.GetUserAccountById(userId);

                emailVerificationService.CreateEmailVerificationHashesAndSendVerificationEmails(
                    userAccount!,
                    new List<string> { model.CentreSpecificEmail },
                    config.GetAppRootPath()
                );
            }

            return RedirectToAction("Confirmation", new { centreId = model.Centre });
        }

        [HttpGet]
        public IActionResult Confirmation(int centreId)
        {
            var userId = User.GetUserIdKnownNotNull();
            var (_, unverifiedCentreEmails) = userService.GetUnverifiedEmailsForUser(userId);
            var (_, centreName, unverifiedCentreEmail) =
                unverifiedCentreEmails.SingleOrDefault(uce => uce.centreId == centreId);

            var model = new AdminConfirmationViewModel(
                null,
                unverifiedCentreEmail,
                centreName
            );

            return View(model);
        }
    }
}
