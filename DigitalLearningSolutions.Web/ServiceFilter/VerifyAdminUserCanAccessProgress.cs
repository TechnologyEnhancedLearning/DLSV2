﻿namespace DigitalLearningSolutions.Web.ServiceFilter
{
    using System.Security.Claims;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class VerifyAdminUserCanAccessProgress : IActionFilter
    {
        private readonly ICourseService courseService;

        public VerifyAdminUserCanAccessProgress(
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

            var progressId = int.Parse(context.RouteData.Values["progressId"].ToString()!);
            var centreId = controller.User.GetCentreId();
            var courseDelegatesData =
                courseService.GetDelegateCourseProgress(progressId, centreId);

            if (courseDelegatesData == null)
            {
                context.Result = new NotFoundResult();
            }
            else if (!ProgressRecordIsAccessibleToUser(courseDelegatesData.DelegateCourseInfo, controller.User))
            {
                context.Result = new NotFoundResult();
            }
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

            var categoryId = user.GetAdminCourseCategoryFilter();

            if (details.CourseCategoryId != categoryId && categoryId != null)
            {
                return false;
            }

            return true;
        }
    }
}
