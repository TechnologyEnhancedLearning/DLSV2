namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
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
    using System;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [ServiceFilter(typeof(VerifyAdminAndDelegateUserCentre))]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [Route("TrackingSystem/Delegates/{delegateId:int}/View")]
    public class ViewDelegateController : Controller
    {
        private readonly IConfiguration config;
        private readonly ICourseService courseService;
        private readonly IPasswordResetService passwordResetService;
        private readonly PromptsService promptsService;
        private readonly IUserService userService;
        private readonly IEmailVerificationService emailVerificationService;
        private readonly ISelfAssessmentService selfAssessmentService;

        public ViewDelegateController(
            IUserService userService,
            PromptsService promptsService,
            ICourseService courseService,
            IPasswordResetService passwordResetService,
            IConfiguration config,
            IEmailVerificationService emailVerificationService,
            ISelfAssessmentService selfAssessmentService
        )
        {
            this.userService = userService;
            this.promptsService = promptsService;
            this.courseService = courseService;
            this.passwordResetService = passwordResetService;
            this.config = config;
            this.emailVerificationService = emailVerificationService;
            this.selfAssessmentService = selfAssessmentService;
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
            foreach (var course in delegateCourses)
            {
                course.Enrolled = (DateTime)DateHelper.GetLocalDateTime(course.Enrolled);
                course.LastUpdated = DateHelper.GetLocalDateTime(course.LastUpdated);
                course.Completed = course.Completed?.TimeOfDay == TimeSpan.Zero ? course.Completed : DateHelper.GetLocalDateTime(course.Completed);
            }
            

            var selfAssessments =
                selfAssessmentService.GetSelfAssessmentsForCandidate(delegateEntity.UserAccount.Id, centreId, categoryIdFilter);

            foreach (var selfassessment in selfAssessments)
            {
                selfassessment.SupervisorCount = selfAssessmentService.GetSupervisorsCountFromCandidateAssessmentId(selfassessment.CandidateAssessmentId);
                selfassessment.IsSameCentre = selfAssessmentService.CheckForSameCentre(centreId, selfassessment.CandidateAssessmentId);
                selfassessment.DelegateUserId = delegateUserCard.UserId;
                selfassessment.StartedDate = (DateTime)DateHelper.GetLocalDateTime(selfassessment.StartedDate);
                selfassessment.LastAccessed = DateHelper.GetLocalDateTime(selfassessment.LastAccessed);
            }

            var model = new ViewDelegateViewModel(delegateUserCard, customFields, delegateCourses, selfAssessments);

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


            EmailVerificationDetails emailVerificationDetails = emailVerificationService.GetEmailVerificationDetailsById(delegateEntity.UserAccount.EmailVerificationHashID ?? 0);

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
            var delegateUser = userService.GetDelegateUserCardById(delegateId)!;
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
            userService.DeactivateDelegateUser(delegateId);

            return RedirectToAction("Index", new { delegateId });
        }

        [HttpPost]
        [Route("ReactivateDelegate")]
        public IActionResult ReactivateDelegate(int delegateId)
        {
            var centreId = User.GetCentreIdKnownNotNull();
            var delegateUser = userService.GetDelegateUserCardById(delegateId);

            if (delegateUser?.CentreId != centreId)
            {
                return new NotFoundResult();
            }

            userService.ActivateDelegateUser(delegateId);

            return RedirectToAction("Index", new { delegateId });
        }

        [HttpPost]
        [Route("DeleteAccount")]
        public IActionResult DeleteAccount(int delegateId)
        {
            var userId = userService.GetUserIdFromDelegateId(delegateId);

            userService.DeleteUserAndAccounts(userId);

            return RedirectToAction("Index", "AllDelegates");
        }
    }
}
