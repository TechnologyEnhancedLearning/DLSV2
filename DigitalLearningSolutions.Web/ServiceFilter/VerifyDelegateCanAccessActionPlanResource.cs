namespace DigitalLearningSolutions.Web.ServiceFilter
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    public class VerifyDelegateCanAccessActionPlanResource : IActionFilter
    {
        private readonly IActionPlanService actionPlanService;

        public VerifyDelegateCanAccessActionPlanResource(
            IActionPlanService actionPlanService
        )
        {
            this.actionPlanService = actionPlanService;
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!(context.Controller is Controller controller))
            {
                return;
            }

            // Candidate Id will be non-null as Authorize(User.Only) attribute will always be executed first
            // because https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/filters?view=aspnetcore-3.1#filter-types-1
            var delegateId = controller.User.GetCandidateIdKnownNotNull();
            var learningLogItemId = int.Parse(context.RouteData.Values["learningLogItemId"].ToString()!);

            var validationResult =
                actionPlanService.VerifyDelegateCanAccessActionPlanResource(learningLogItemId, delegateId);

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
