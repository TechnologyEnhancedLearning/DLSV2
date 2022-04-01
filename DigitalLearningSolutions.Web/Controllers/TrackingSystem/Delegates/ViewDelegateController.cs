namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Collections.Generic;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Extensions;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ServiceFilter;
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
        private readonly PromptsService promptsService;
        private readonly ICourseDataService courseDataService;
        private readonly ICourseService courseService;
        private readonly IPasswordResetService passwordResetService;
        private readonly IUserDataService userDataService;
        private readonly IConfiguration config;

        public ViewDelegateController(
            IUserDataService userDataService,
            PromptsService promptsService,
            ICourseService courseService,
            IPasswordResetService passwordResetService,
            ICourseDataService courseDataService,
            IConfiguration config
        )
        {
            this.userDataService = userDataService;
            this.promptsService = promptsService;
            this.courseService = courseService;
            this.passwordResetService = passwordResetService;
            this.courseDataService = courseDataService;
            this.config = config;
        }

        public IActionResult Index(int delegateId)
        {
            var centreId = User.GetCentreId();
            var delegateUser = userDataService.GetDelegateUserCardById(delegateId)!;
            var categoryIdFilter = User.GetAdminCourseCategoryFilter();

            var customFields = promptsService.GetDelegateRegistrationPromptsForCentre(centreId, delegateUser);
            var delegateCourses =
                courseService.GetAllCoursesInCategoryForDelegate(delegateId, centreId, categoryIdFilter);

            var model = new ViewDelegateViewModel(delegateUser, customFields, delegateCourses);

            return View(model);
        }

        [Route("SendWelcomeEmail")]
        public IActionResult SendWelcomeEmail(int delegateId)
        {
            var delegateUser = userDataService.GetDelegateUserCardById(delegateId)!;

            var baseUrl = config.GetAppRootPath();

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
        [Route("{customisationId:int}/{accessedVia}/Remove")]
        [ServiceFilter(typeof(VerifyDelegateAccessedViaValidRoute))]
        [ServiceFilter(typeof(VerifyAdminUserCanViewCourse))]
        public IActionResult ConfirmRemoveFromCourse(
            int delegateId,
            int customisationId,
            DelegateAccessRoute accessedVia,
            string? returnPageQuery = null
        )
        {
            if (!courseService.DelegateHasCurrentProgress(delegateId, customisationId))
            {
                return new NotFoundResult();
            }

            var delegateUser = userDataService.GetDelegateUserCardById(delegateId);
            var course = courseDataService.GetCourseNameAndApplication(customisationId);

            var model = new RemoveFromCourseViewModel(
                delegateId,
                DisplayStringHelper.GetNonSortableFullNameForDisplayOnly(
                    delegateUser!.FirstName,
                    delegateUser.LastName
                ),
                customisationId,
                course!.CourseName,
                false,
                accessedVia,
                returnPageQuery
            );

            return View("ConfirmRemoveFromCourse", model);
        }

        [HttpPost]
        [Route("{customisationId:int}/Remove")]
        [ServiceFilter(typeof(VerifyAdminUserCanViewCourse))]
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

            if (!courseService.DelegateHasCurrentProgress(delegateId, customisationId))
            {
                return new NotFoundResult();
            }

            courseService.RemoveDelegateFromCourse(
                delegateId,
                customisationId,
                RemovalMethod.RemovedByAdmin
            );

            return model.AccessedVia.Equals(DelegateAccessRoute.CourseDelegates)
                ? RedirectToAction(
                    "Index",
                    "CourseDelegates",
                    GetCourseDelegatesRouteValues(customisationId, model.ReturnPageQuery!.Value)
                )
                : RedirectToAction("Index", "ViewDelegate", new { delegateId });
        }

        private static Dictionary<string, string> GetCourseDelegatesRouteValues(int customisationId, ReturnPageQuery returnPageQuery)
        {
            var dict = returnPageQuery.ToRouteDataDictionary();
            dict.Add("customisationId", customisationId.ToString());
            return dict;
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
