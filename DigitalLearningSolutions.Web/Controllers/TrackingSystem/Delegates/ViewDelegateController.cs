namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.ViewDelegate;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("TrackingSystem/Delegates/{delegateId:int}/View")]
    public class ViewDelegateController : Controller
    {
        private readonly CentreCustomPromptHelper centreCustomPromptHelper;
        private readonly ICourseService courseService;
        private readonly IPasswordResetService passwordResetService;
        private readonly IUserDataService userDataService;
        private readonly ICourseDataService courseDataService;

        public ViewDelegateController(
            IUserDataService userDataService,
            CentreCustomPromptHelper centreCustomPromptHelper,
            ICourseService courseService,
            IPasswordResetService passwordResetService,
            ICourseDataService courseDataService
        )
        {
            this.userDataService = userDataService;
            this.centreCustomPromptHelper = centreCustomPromptHelper;
            this.courseService = courseService;
            this.passwordResetService = passwordResetService;
            this.courseDataService = courseDataService;
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
            var delegateCourses = courseService.GetAllCoursesForDelegate(delegateId, centreId);

            var model = new ViewDelegateViewModel(delegateUser, customFields, delegateCourses);

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

        [HttpPost]
        [Route("DeactivateDelegate")]
        public IActionResult DeactivateDelegate(int delegateId)
        {
            var centreId = User.GetCentreId();
            var delegateUser = userDataService.GetDelegateUserCardById(delegateId);
            if (delegateUser == null || delegateUser.CentreId != centreId)
            {
                return new NotFoundResult();
            }

            userDataService.DeactivateDelegateUser(delegateId);

            return RedirectToAction("Index", new { delegateId } );
        }

        [HttpPost]
        [Route("RemoveCourse")]
        public IActionResult RemoveCourse(int delegateId, int courseId)
        {
            var centreId = User.GetCentreId();
            var delegateUser = userDataService.GetDelegateUserCardById(delegateId);
            if (delegateUser == null || delegateUser.CentreId != centreId)
            {
                return new NotFoundResult();
            }

            // get progress id
            // validate progress entry
            // TODO HEEDLS-501: should I put the course data service behind a course service method?

            courseDataService.RemoveCurrentCourse(0, delegateId, RemovalMethod.RemovedByAdmin);

            return RedirectToAction("Index", new { delegateId });
        }
    }
}
