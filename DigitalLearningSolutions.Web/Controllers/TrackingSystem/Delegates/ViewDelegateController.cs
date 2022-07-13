namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models.User;
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

        public ViewDelegateController(
            IUserDataService userDataService,
            IUserService userService,
            PromptsService promptsService,
            ICourseService courseService,
            IPasswordResetService passwordResetService,
            IConfiguration config
        )
        {
            this.userDataService = userDataService;
            this.userService = userService;
            this.promptsService = promptsService;
            this.courseService = courseService;
            this.passwordResetService = passwordResetService;
            this.config = config;
        }

        public IActionResult Index(int delegateId)
        {
            var centreId = User.GetCentreIdKnownNotNull();

            var delegateEntity = userService.GetDelegateById(delegateId)!;

            if (delegateEntity == null)
            {
                return NotFound();
            }

            var delegateUserCard = new DelegateUserCard(delegateEntity);
            var categoryIdFilter = User.GetAdminCategoryId();

            var customFields = promptsService.GetDelegateRegistrationPromptsForCentre(centreId, delegateUserCard);
            var delegateCourses =
                courseService.GetAllCoursesInCategoryForDelegate(delegateId, centreId, categoryIdFilter);

            var model = new ViewDelegateViewModel(delegateUserCard, customFields, delegateCourses);

            return View(model);
        }

        [Route("SendWelcomeEmail")]
        public IActionResult SendWelcomeEmail(int delegateId)
        {
            var delegateUser = userDataService.GetDelegateUserCardById(delegateId)!;

            var baseUrl = config.GetAppRootPath();

            passwordResetService.GenerateAndSendDelegateWelcomeEmail(
                delegateId,
                baseUrl
            );

            var model = new WelcomeEmailSentViewModel(delegateUser);

            return View("WelcomeEmailSent", model);
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
    }
}
