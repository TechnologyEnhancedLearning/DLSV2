namespace DigitalLearningSolutions.Web.ServiceFilter
{
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class VerifyAdminUserCanAccessGroup : IActionFilter
    {
        private readonly IGroupsService groupsService;

        public VerifyAdminUserCanAccessGroup(IGroupsService groupsService)
        {
            this.groupsService = groupsService;
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!(context.Controller is Controller controller))
            {
                return;
            }

            var groupId = int.Parse(context.RouteData.Values["groupId"].ToString()!);
            var groupCentreId = groupsService.GetGroupCentreId(groupId);

            if (controller.User.GetCentreIdKnownNotNull() != groupCentreId)
            {
                context.Result = new NotFoundResult();
            }
        }
    }
}
