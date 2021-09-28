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
            var categoryId = controller.User.GetAdminCategoryId()!;
            var customisationId = int.Parse(context.RouteData.Values["customisationId"].ToString()!);

            if (!courseService.VerifyAdminUserCanAccessCourse(customisationId, centreId, categoryId.Value))
            {
                context.Result = new NotFoundResult();
            }
        }
    }
}
