namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.ViewDelegate;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [ServiceFilter(typeof(VerifyAdminUserCanAccessDelegateUser))]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    [SetSelectedTab(nameof(NavMenuTab.Delegates))]
    [Route("TrackingSystem/Delegates/{delegateId:int}/View")]
    public class ViewDelegateController : Controller
    {
        private readonly CentreCustomPromptHelper centreCustomPromptHelper;
        private readonly ICourseDataService courseDataService;
        private readonly ICourseService courseService;
        private readonly IPasswordResetService passwordResetService;
        private readonly IUserDataService userDataService;

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
            var delegateUser = userDataService.GetDelegateUserCardById(delegateId)!;
            var categoryIdFilter = User.GetAdminCourseCategoryFilter();

            var customFields = centreCustomPromptHelper.GetCustomFieldViewModelsForCentre(centreId, delegateUser);
            var delegateCourses =
                courseService.GetAllCoursesInCategoryForDelegate(delegateId, centreId, categoryIdFilter);

            var model = new ViewDelegateViewModel(delegateUser, customFields, delegateCourses);

            return View(model);
        }

        [Route("SendWelcomeEmail")]
        public IActionResult SendWelcomeEmail(int delegateId)
        {
            var delegateUser = userDataService.GetDelegateUserCardById(delegateId)!;

            string baseUrl = ConfigHelper.GetAppConfig().GetAppRootPath();

            passwordResetService.GenerateAndSendDelegateWelcomeEmail(
                delegateUser.EmailAddress!,
                delegateUser.CandidateNumber,
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

        [HttpGet]
        [Route("{customisationId:int}/Remove")]
        [ServiceFilter(typeof(VerifyAdminUserCanManageCourse))]
        public IActionResult ConfirmRemoveFromCourse(int delegateId, int customisationId)
        {
            var delegateUser = userDataService.GetDelegateUserCardById(delegateId);
            var course = courseDataService.GetCourseNameAndApplication(customisationId);

            var model = new RemoveFromCourseViewModel
            {
                DelegateId = delegateUser!.Id,
                CustomisationId = customisationId,
                CourseName = course!.CourseName,
                Name = delegateUser.FullName,
                Confirm = false,
            };
            return View("ConfirmRemoveFromCourse", model);
        }

        [HttpPost]
        [Route("{customisationId:int}/Remove")]
        [ServiceFilter(typeof(VerifyAdminUserCanManageCourse))]
        public IActionResult ExecuteRemoveFromCourse(
            int delegateId,
            int customisationId,
            RemoveFromCourseViewModel model
        )
        {
            if (!ModelState.IsValid)
            {
                return View("ConfirmRemoveFromCourse", model);
            }

            if (!courseService.RemoveDelegateFromCourseIfDelegateHasCurrentProgress(
                delegateId,
                customisationId,
                RemovalMethod.RemovedByAdmin
            ))
            {
                return new NotFoundResult();
            }

            return RedirectToAction("Index", new { delegateId });
        }

        [HttpPost]
        [Route("ReactivateDelegate")]
        public IActionResult ReactivateDelegate(int delegateId)
        {
            var centreId = User.GetCentreId();
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
