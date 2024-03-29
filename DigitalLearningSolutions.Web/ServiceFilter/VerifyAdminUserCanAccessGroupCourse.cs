﻿namespace DigitalLearningSolutions.Web.ServiceFilter
{
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class VerifyAdminUserCanAccessGroupCourse : IActionFilter
    {
        private readonly IGroupsService groupService;

        public VerifyAdminUserCanAccessGroupCourse(IGroupsService groupService)
        {
            this.groupService = groupService;
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!(context.Controller is Controller controller))
            {
                return;
            }

            var centreId = controller.User.GetCentreIdKnownNotNull();
            var groupId = int.Parse(context.RouteData.Values["groupId"].ToString()!);
            var groupCustomisationId = int.Parse(context.RouteData.Values["groupCustomisationId"].ToString()!);

            var groupCourse = groupService.GetUsableGroupCourseForCentre(groupCustomisationId, groupId, centreId);

            if (groupCourse == null)
            {
                context.Result = new NotFoundResult();
            }
        }
    }
}
