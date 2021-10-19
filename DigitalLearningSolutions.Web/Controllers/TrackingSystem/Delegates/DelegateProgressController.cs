namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using System.Security.Claims;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("TrackingSystem/Delegates/{accessedVia}/DelegateProgress/{progressId:int}")]
    [SetDlsSubApplication(nameof(DlsSubApplication.TrackingSystem))]
    public class DelegateProgressController : Controller
    {
        private readonly ICourseService courseService;

        public DelegateProgressController(ICourseService courseService)
        {
            this.courseService = courseService;
        }

        public IActionResult Index(int progressId, DelegateProgressAccessRoute accessedVia)
        {
            if (!DelegateProgressAccessRoute.CourseDelegates.Equals(accessedVia) &&
                !DelegateProgressAccessRoute.ViewDelegate.Equals(accessedVia))
            {
                return NotFound();
            }

            var centreId = User.GetCentreId();
            var courseDelegatesData =
                courseService.GetDelegateCourseProgress(progressId, centreId);

            if (courseDelegatesData == null ||
                !ProgressRecordIsAccessibleToUser(courseDelegatesData.DelegateCourseInfo, User))
            {
                return NotFound();
            }

            var model = new DelegateProgressViewModel(accessedVia, courseDelegatesData!);
            return View(model);
        }

        private static bool ProgressRecordIsAccessibleToUser(DelegateCourseInfo details, ClaimsPrincipal user)
        {
            var centreId = user.GetCentreId();

            if (details.DelegateCentreId != centreId)
            {
                return false;
            }

            if (details.CustomisationCentreId != centreId && !details.AllCentresCourse)
            {
                return false;
            }

            var categoryId = user.GetAdminCategoryId()!.Value;

            if (details.CourseCategoryId != categoryId && categoryId != 0)
            {
                return false;
            }

            return true;
        }
    }
}
