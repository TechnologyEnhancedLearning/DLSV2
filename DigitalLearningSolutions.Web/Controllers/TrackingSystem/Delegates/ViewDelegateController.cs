namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models;
    using DigitalLearningSolutions.Data.Models.Email;
    using DigitalLearningSolutions.Data.Models.User;
    using DigitalLearningSolutions.Data.Utilities;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.ViewDelegate;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [ServiceFilter(typeof(VerifyAdminUserCanAccessDelegateUser))]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [Route("TrackingSystem/Delegates/{delegateId:int}/View")]
    public class ViewDelegateController : Controller
    {
        private readonly IConfiguration config;
        private readonly ICourseService courseService;
        private readonly IPasswordResetService passwordResetService;
        private readonly PromptsService promptsService;
        private readonly IUserDataService userDataService;
        private readonly IUserService userService;
        private readonly IEmailVerificationService emailVerificationService;
        private readonly IEmailVerificationDataService emailVerificationDataService;

        public ViewDelegateController(
            IUserDataService userDataService,
            IUserService userService,
            PromptsService promptsService,
            ICourseService courseService,
            IPasswordResetService passwordResetService,
            IConfiguration config,
            IEmailVerificationService emailVerificationService,
            IEmailVerificationDataService emailVerificationDataService
        )
        {
            this.userDataService = userDataService;
            this.userService = userService;
            this.promptsService = promptsService;
            this.courseService = courseService;
            this.passwordResetService = passwordResetService;
            this.config = config;
            this.emailVerificationService = emailVerificationService;
            this.emailVerificationDataService = emailVerificationDataService;
        }

        public IActionResult Index(int delegateId, string? callType)
        {
            var centreId = User.GetCentreIdKnownNotNull();

            var delegateEntity = userService.GetDelegateById(delegateId)!;

            if (delegateEntity == null)
            {
                return NotFound();
            }

            if (string.IsNullOrEmpty(callType) && TempData["IsDelegatePromoted"] != null)
            {
                TempData.Remove("IsDelegatePromoted");
            }

            var delegateUserCard = new DelegateUserCard(delegateEntity);
            var categoryIdFilter = User.GetAdminCategoryId();

            var customFields = promptsService.GetDelegateRegistrationPromptsForCentre(centreId, delegateUserCard);
            var delegateCourses =
                courseService.GetAllCoursesInCategoryForDelegate(delegateId, centreId, categoryIdFilter);

            var model = new ViewDelegateViewModel(delegateUserCard, customFields, delegateCourses);

            if (DisplayStringHelper.IsGuid(model.DelegateInfo.Email))
                model.DelegateInfo.Email = null;

            var baseUrl = config.GetAppRootPath();
            IClockUtility clockUtility = new ClockUtility();
            if ((model.DelegateInfo?.IsActive ?? false) && (model.DelegateInfo.RegistrationConfirmationHash != null)
                )
            {
                Email welcomeEmail = passwordResetService.GenerateDelegateWelcomeEmail(delegateId, baseUrl);
                model.WelcomeEmail = "mailto:" + string.Join(",", welcomeEmail.To) + "?subject=" + welcomeEmail.Subject + "&body=" + welcomeEmail.Body.TextBody.Replace("&", "%26");
            }


            EmailVerificationDetails emailVerificationDetails = emailVerificationDataService.GetEmailVerificationDetailsById(delegateEntity.UserAccount.EmailVerificationHashID ?? 0);

            if (delegateEntity.UserAccount.EmailVerified == null
                && delegateEntity.UserAccount.EmailVerificationHashID != null
                && (emailVerificationDetails.EmailVerificationHashCreatedDate.AddDays(2) > clockUtility.UtcNow))
            {
                var userEntity = userService.GetUserById(delegateEntity.DelegateAccount.UserId);
                Email verificationEmail = emailVerificationService.GenerateVerificationEmail(userEntity.UserAccount, emailVerificationDetails.EmailVerificationHash,
                   delegateEntity.UserAccount.PrimaryEmail, baseUrl);
                model.VerificationEmail = "mailto:" + string.Join(",", verificationEmail.To) + "?subject=" + verificationEmail.Subject + "&body=" + verificationEmail.Body.TextBody.Replace("&", "%26");
            }
            return View(model);
        }

        [Route("SendWelcomeEmail")]
        public IActionResult SendWelcomeEmail(int delegateId)
        {
            var delegateUser = userDataService.GetDelegateUserCardById(delegateId)!;
            var model = new WelcomeEmailSentViewModel(delegateUser);

            if (delegateUser.RegistrationConfirmationHash != null)
            {
                var baseUrl = config.GetAppRootPath();

                passwordResetService.GenerateAndSendDelegateWelcomeEmail(
                    delegateId,
                    baseUrl,
                    delegateUser.RegistrationConfirmationHash
                );
                return View("WelcomeEmailSent", model);
            }
            else
            {
                return View("DelegateAccountAlreadyClaimed", model);
            }
        }

        [HttpPost]
        [Route("DeactivateDelegate")]
        public IActionResult DeactivateDelegate(int delegateId)
        {
            userDataService.DeactivateDelegateUser(delegateId);

            return RedirectToAction("Index", new { delegateId });
        }

        [HttpPost]
        [Route("ReactivateDelegate")]
        public IActionResult ReactivateDelegate(int delegateId)
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var delegateUser = userDataService.GetDelegateUserCardById(delegateId);

            if (delegateUser?.CentreId != centreId)
            {
                return new NotFoundResult();
            }

            userDataService.ActivateDelegateUser(delegateId);

            return RedirectToAction("Index", new { delegateId });
        }

        [HttpPost]
        [Route("DeleteAccount")]
        public IActionResult DeleteAccount(int delegateId)
        {
            var userId = userDataService.GetUserIdFromDelegateId(delegateId);

            userDataService.DeleteUserAndAccounts(userId);

            return RedirectToAction("Index", "AllDelegates");
        }
    }
}
