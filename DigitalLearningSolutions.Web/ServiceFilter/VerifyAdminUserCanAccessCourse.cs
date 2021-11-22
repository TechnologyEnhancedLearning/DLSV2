namespace DigitalLearningSolutions.Web.ServiceFilter
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class VerifyAdminUserCanAccessCourse : IActionFilter
    {
        private readonly ICourseService courseService;

        public VerifyAdminUserCanAccessCourse(
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
                courseService.VerifyAdminUserCanAccessCourse(customisationId, centreId, categoryId);

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
