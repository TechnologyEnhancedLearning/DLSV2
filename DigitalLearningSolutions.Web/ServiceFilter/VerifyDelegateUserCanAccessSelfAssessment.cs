namespace DigitalLearningSolutions.Web.ServiceFilter
{
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;

    public class VerifyDelegateUserCanAccessSelfAssessment : IActionFilter
    {
        private readonly ILogger<VerifyDelegateUserCanAccessSelfAssessment> logger;
        private readonly ISelfAssessmentService selfAssessmentService;

        public VerifyDelegateUserCanAccessSelfAssessment(
            ISelfAssessmentService selfAssessmentService,
            ILogger<VerifyDelegateUserCanAccessSelfAssessment> logger
        )
        {
            this.selfAssessmentService = selfAssessmentService;
            this.logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!(context.Controller is Controller controller))
            {
                return;
            }

            var selfAssessmentId = int.Parse(context.RouteData.Values["selfAssessmentId"].ToString()!);
            var userId = controller.User.GetUserIdKnownNotNull();
            var canAccessSelfAssessment =
                selfAssessmentService.CanDelegateAccessSelfAssessment(userId, selfAssessmentId);

            if (!canAccessSelfAssessment)
            {
                logger.LogWarning(
                    $"Attempt to display self assessment results for user {userId} with no self assessment"
                );
                context.Result = new RedirectToActionResult("AccessDenied", "LearningSolutions", new { });
            }
        }
    }
}
