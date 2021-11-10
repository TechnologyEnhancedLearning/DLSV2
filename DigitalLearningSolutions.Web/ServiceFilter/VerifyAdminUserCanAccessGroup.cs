namespace DigitalLearningSolutions.Web.ServiceFilter
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class VerifyAdminUserCanAccessGroup : IActionFilter
    {
        private readonly IGroupsDataService groupsDataService;

        public VerifyAdminUserCanAccessGroup(IGroupsDataService groupsDataService)
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

            var groupId = int.Parse(context.RouteData.Values["groupId"].ToString()!);
            var groupCentreId = groupsDataService.GetGroupCentreId(groupId);

            if (controller.User.GetCentreId() != groupCentreId)
            {
                context.Result = new NotFoundResult();
            }
        }
    }
}
