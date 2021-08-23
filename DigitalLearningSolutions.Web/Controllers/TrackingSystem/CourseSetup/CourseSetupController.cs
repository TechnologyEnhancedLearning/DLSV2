namespace DigitalLearningSolutions.Web.Controllers.TrackingSystem.CourseSetup
{
    using System.Linq;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Helpers.FilterOptions;
    using DigitalLearningSolutions.Web.ViewModels.Common.SearchablePage;
    using DigitalLearningSolutions.Web.ViewModels.TrackingSystem.CourseSetup;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.RefactoredTrackingSystem)]
    [Authorize(Policy = CustomPolicies.UserCentreAdmin)]
    [Route("/TrackingSystem/CourseSetup")]
    public class CourseSetupController : Controller
    {
        private const string CourseFilterCookieName = "CourseFilter";
        private readonly ICourseCategoriesDataService courseCategoriesDataService;
        private readonly ICourseService courseService;
        private readonly ICourseTopicsDataService courseTopicsDataService;

        public CourseSetupController(
            ICourseService courseService,
            ICourseCategoriesDataService courseCategoriesDataService,
            ICourseTopicsDataService courseTopicsDataService
        )
        {
            this.courseService = courseService;
            this.courseCategoriesDataService = courseCategoriesDataService;
            this.courseTopicsDataService = courseTopicsDataService;
        }

        [Route("{page=1:int}")]
        public IActionResult Index(
            string? searchString = null,
            string? sortBy = null,
            string sortDirection = BaseSearchablePageViewModel.Ascending,
            string? filterBy = null,
            string? filterValue = null,
            int page = 1
        )
        {
            if (filterBy == null && filterValue == null)
            {
                filterBy = Request.Cookies[CourseFilterCookieName];
                if (filterBy == null)
                {
                    // Only show Active courses by default
                    filterValue = CourseStatusFilterOptions.IsActive.FilterValue;
                }
            }
            else if (filterBy?.ToUpper() == FilteringHelper.ClearString)
            {
                filterBy = null;
            }

            sortBy ??= DefaultSortByOptions.Name.PropertyName;
            filterBy = FilteringHelper.AddNewFilterToFilterBy(filterBy, filterValue);

            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCategoryId()!;
            var centreCourses = courseService.GetCentreSpecificCourseStatistics(centreId, categoryId.Value);
            var categories = courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId)
                .Select(c => c.CategoryName);
            var topics = courseTopicsDataService.GetCourseTopicsAvailableAtCentre(centreId).Select(c => c.CourseTopic);

            var model = new CourseSetupViewModel(
                centreCourses,
                categories,
                topics,
                searchString,
                sortBy,
                sortDirection,
                filterBy,
                page
            );

            Response.UpdateOrDeleteFilterCookie(CourseFilterCookieName, filterBy);

            return View(model);
        }

        [Route("AllCourseStatistics")]
        public IActionResult AllCourseStatistics()
        {
            var centreId = User.GetCentreId();
            var categoryId = User.GetAdminCategoryId()!;
            var centreCourses = courseService.GetCentreSpecificCourseStatistics(centreId, categoryId.Value);
            var categories = courseCategoriesDataService.GetCategoriesForCentreAndCentrallyManagedCourses(centreId)
                .Select(c => c.CategoryName);
            var topics = courseTopicsDataService.GetCourseTopicsAvailableAtCentre(centreId).Select(c => c.CourseTopic);

            var model = new AllCourseStatisticsViewModel(centreCourses, categories, topics);
            return View(model);
        }
    }
}
