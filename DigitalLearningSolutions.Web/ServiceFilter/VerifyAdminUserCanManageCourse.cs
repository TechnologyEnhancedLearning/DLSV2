namespace DigitalLearningSolutions.Web.ServiceFilter
{
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class VerifyAdminUserCanManageCourse : IActionFilter
    {
        private readonly ICourseService courseService;

        public VerifyAdminUserCanManageCourse(
            ICourseService courseService
        )
        {
            this.courseService = courseService;
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!(context.Controller is Controller controller))
            {
                return;
            }

            var centreId = controller.User.GetCentreId();
            var categoryId = controller.User.GetAdminCourseCategoryFilter();
            var customisationId = int.Parse(context.RouteData.Values["customisationId"].ToString()!);

            var validationResult =
                courseService.VerifyAdminUserCanManageCourse(customisationId, centreId, categoryId);

            if (!validationResult.HasValue)
            {
                context.Result = new NotFoundResult();
            }
            else if (!validationResult.Value)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "LearningSolutions", new { });
            }
        }
    }
}
