namespace DigitalLearningSolutions.Web.ServiceFilter
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.Extensions.Logging;

    public class RequireProcessAgreementFilter : IActionFilter
    {
        private readonly ISelfAssessmentService selfAssessmentService;
        private readonly ILogger<RequireProcessAgreementFilter> logger;

        public RequireProcessAgreementFilter(
            ISelfAssessmentService selfAssessmentService,
            ILogger<RequireProcessAgreementFilter> logger
        )
        {
            this.selfAssessmentService = selfAssessmentService;
            this.logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Path.ToString().Contains("/LearningPortal/SelfAssessment/"))
            {
                if (!(context.Controller is Controller controller))
                {
                    return;
                }

                if (!context.ActionArguments.ContainsKey("selfAssessmentId"))
                {
                    return;
                }

                var selfAssessmentId = int.Parse(context.ActionArguments["selfAssessmentId"].ToString()!);
                var delegateUserId = controller.User.GetUserIdKnownNotNull();

                var selfAssessment = selfAssessmentService.GetSelfAssessmentForCandidateById(delegateUserId, selfAssessmentId);

                if (selfAssessment == null)
                {
                    logger.LogWarning(
                        $"Attempt to access self assessment {selfAssessmentId} by user {delegateUserId}, but no such assessment found"
                    );
                    context.Result = new RedirectToActionResult("StatusCode", "LearningSolutions", new { code = 403 });
                    return;
                }

                var actionName = context.RouteData.Values["action"]?.ToString();
                if (actionName == "AgreeSelfAssessmentProcess" || actionName == "ProcessAgreed")
                {
                    return;
                }

                if (!selfAssessment.SelfAssessmentProcessAgreed && selfAssessment.IsSupervised)
                {
                    logger.LogInformation(
                        $"Redirecting user {delegateUserId} to agree process page for self assessment {selfAssessmentId}"
                    );

                    context.Result = new RedirectToActionResult(
                        "AgreeSelfAssessmentProcess",
                        "LearningPortal",
                        new { selfAssessmentId }
                    );
                }
            }
        }
    }
}
