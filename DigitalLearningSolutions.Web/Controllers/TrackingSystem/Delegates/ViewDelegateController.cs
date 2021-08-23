namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("TrackingSystem/Delegates/View/{delegateId:int}")]
    public class ViewDelegateController : Controller
    {
        private readonly CentreCustomPromptHelper centreCustomPromptHelper;
        private readonly ICourseService courseService;
        private readonly IPasswordResetService passwordResetService;
        private readonly IUserDataService userDataService;

        public ViewDelegateController(
            IUserDataService userDataService,
            CentreCustomPromptHelper centreCustomPromptHelper,
            ICourseService courseService,
            IPasswordResetService passwordResetService
        )
        {
            this.userDataService = userDataService;
            this.centreCustomPromptHelper = centreCustomPromptHelper;
            this.courseService = courseService;
            this.passwordResetService = passwordResetService;
        }

        public IActionResult Index(int delegateId)
        {
            var centreId = User.GetCentreId();

            var delegateUser = userDataService.GetDelegateUserCardById(delegateId);
            if (delegateUser == null || delegateUser.CentreId != centreId)
            {
                return new NotFoundResult();
            }

            var customFields = centreCustomPromptHelper.GetCustomFieldViewModelsForCentre(centreId, delegateUser);
            var delegateInfoViewModel = new DelegateInfoViewModel(delegateUser, customFields);

            var courseInfoViewModels = courseService.GetAllCoursesForDelegate(delegateId, centreId)
                .Select(x => new DelegateCourseInfoViewModel(x));

            var tags = FilterableTagHelper.GetCurrentTagsForDelegateUser(delegateUser);
            var model = new ViewDelegateViewModel(delegateInfoViewModel, courseInfoViewModels, tags);

            return View(model);
        }

        [Route("SendWelcomeEmail")]
        public IActionResult SendWelcomeEmail(int delegateId)
        {
            var centreId = User.GetCentreId();

            var delegateUser = userDataService.GetDelegateUserCardById(delegateId);
            if (delegateUser == null || delegateUser.CentreId != centreId)
            {
                return new NotFoundResult();
            }

            string baseUrl = ConfigHelper.GetAppConfig().GetAppRootPath();

            passwordResetService.GenerateAndSendDelegateWelcomeEmail(delegateUser.EmailAddress!, baseUrl);

            var model = new WelcomeEmailSentViewModel(delegateUser);

            return View("WelcomeEmailSent", model);
        }
    }
}
