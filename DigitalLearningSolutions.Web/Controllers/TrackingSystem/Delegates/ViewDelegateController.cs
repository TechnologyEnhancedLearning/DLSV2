namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Linq;
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
        private readonly IProgressDataService progressDataService;

        public ViewDelegateController(
            IUserDataService userDataService,
            CentreCustomPromptHelper centreCustomPromptHelper,
            ICourseService courseService,
            IPasswordResetService passwordResetService,
            ICourseDataService courseDataService,
            IProgressDataService progressDataService
        )
        {
            this.userDataService = userDataService;
            this.centreCustomPromptHelper = centreCustomPromptHelper;
            this.courseService = courseService;
            this.passwordResetService = passwordResetService;
            this.courseDataService = courseDataService;
            this.progressDataService = progressDataService;
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

        [HttpGet]
        [Route("{customisationId:int}/Remove")]
        public IActionResult ConfirmRemoveCourse(int delegateId, int customisationId)
        {
            var centreId = User.GetCentreId();
            var delegateUser = userDataService.GetDelegateUserCardById(delegateId);
            if (delegateUser == null || delegateUser.CentreId != centreId)
            {
                return new NotFoundResult();
            }

            var model = new RemoveFromCourseViewModel
            {
                DelegateId = delegateId,
                CustomisationId = customisationId,
                CourseName = "test name",
                Forename = "hank",
                Surname = "hercules"
            };
            return View("ConfirmRemoveFromCourse", model);
        }

        [HttpPost]
        [Route("{customisationId:int}/Remove")]
        public IActionResult RemoveFromCourse(int delegateId, int customisationId)
        {
            var centreId = User.GetCentreId();
            var delegateUser = userDataService.GetDelegateUserCardById(delegateId);
            if (delegateUser == null || delegateUser.CentreId != centreId)
            {
                return new NotFoundResult();
            }

            var currentProgress = progressDataService.GetDelegateProgressForCourse(delegateId, customisationId)
                .FirstOrDefault(p => p.Completed != null && p.RemovedDate != null);
            if (currentProgress == null)
            {
                return new NotFoundResult();
            }

            // TODO HEEDLS-501: should I put the course data service (and for the validation step?) behind a service method?
            // TODO HEEDLS-501: hold on, which progresses am I supposed to be removing?

            courseDataService.RemoveCurrentCourse(currentProgress.ProgressId, delegateId, RemovalMethod.RemovedByAdmin);

            return RedirectToAction("Index", new { delegateId });
        }
    }
}
