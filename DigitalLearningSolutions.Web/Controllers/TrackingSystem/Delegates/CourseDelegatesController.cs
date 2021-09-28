namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.Delegates
{
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.CourseDelegates;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("TrackingSystem/Delegates/CourseDelegates")]
    public class CourseDelegatesController : Controller
    {
        private readonly ICourseDelegatesService courseDelegatesService;
        private readonly ICourseService courseService;

        public CourseDelegatesController(
            ICourseDelegatesService courseDelegatesService,
            ICourseService courseService
        )
        {
            this.courseDelegatesService = courseDelegatesService;
            this.courseService = courseService;
        }

        public IActionResult Index(int? customisationId = null)
        {
            var centreId = User.GetCentreId();
            int? categoryId = User.GetAdminCategoryId()!.Value;
            // admins have a non-nullable category ID where 0 = all categories
            categoryId = categoryId == 0 ? null : categoryId;
            var courseDelegatesData =
                courseDelegatesService.GetCoursesAndCourseDelegatesForCentre(centreId, categoryId, customisationId);

            var model = new CourseDelegatesViewModel(courseDelegatesData);

            return View(model);
        }

        [Route("DelegateProgress/{progressId:int}")]
        public IActionResult DelegateProgress(int progressId)
        {
            var centreId = User.GetCentreId();
            var courseDelegatesData =
                courseService.GetDelegateCourseProgress(progressId, centreId);

            if (!ProgressRecordIsValidAndAccessibleToUser(courseDelegatesData?.DelegateCourseInfo, centreId))
            {
                return NotFound();
            }

            var model = new DelegateProgressViewModel(courseDelegatesData!);
            return View(model);
        }

        private bool ProgressRecordIsValidAndAccessibleToUser(DelegateCourseInfo? details, int centreId)
        {
            if (details == null)
            {
                return false;
            }

            if (details.DelegateCentreId != centreId)
            {
                return false;
            }

            if (details.CustomisationCentreId != centreId && !details.AllCentresCourse)
            {
                return false;
            }

            var categoryId = User.GetAdminCategoryId()!.Value;

            if (details.CourseCategoryId != categoryId && categoryId != 0)
            {
                return false;
            }

            return true;
        }
    }
}
