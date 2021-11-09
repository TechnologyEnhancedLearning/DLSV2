namespace DigitalLearningSolutions.Web.ServiceFilter
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class VerifyAdminUserCanAccessGroupCourse : IActionFilter
    {
        private readonly IGroupsDataService groupsDataService;

        public VerifyAdminUserCanAccessGroupCourse(IGroupsDataService groupsDataService)
        {
            this.groupsDataService = groupsDataService;
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!(context.Controller is Controller controller))
            {
                return;
            }

            var centerId = controller.User.GetCentreId();
            var groupId = int.Parse(context.RouteData.Values["groupId"].ToString()!);
            var groupCustomisationId = int.Parse(context.RouteData.Values["groupCustomisationId"].ToString()!);

            var groupCourse = groupsDataService.GetGroupCourse(groupCustomisationId, groupId, centerId);

            if (groupCourse == null)
            {
                context.Result = new NotFoundResult();
            }
        }
    }
}
