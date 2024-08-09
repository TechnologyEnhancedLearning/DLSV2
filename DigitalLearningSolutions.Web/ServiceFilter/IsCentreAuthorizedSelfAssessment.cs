namespace DigitalLearningSolutions.Web.ServiceFilter
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Services;
    using Microsoft.Extensions.Logging;

    public class IsCentreAuthorizedSelfAssessment : IActionFilter
    {
        private readonly ISelfAssessmentService selfAssessmentService;
        private readonly ILogger<IsCentreAuthorizedSelfAssessment> logger;

        public IsCentreAuthorizedSelfAssessment(ISelfAssessmentService selfAssessmentService,
            ILogger<IsCentreAuthorizedSelfAssessment> logger)
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
            var centreId = controller.User.GetCentreIdKnownNotNull();
            var selfAssessmentId = int.Parse(context.ActionArguments["selfAssessmentId"].ToString()!);

            if (centreId > 0 && selfAssessmentId > 0)
            {
                if (!selfAssessmentService.IsCentreSelfAssessment(selfAssessmentId, centreId))
                {
                    logger.LogWarning($"Attempt to access restricted self assessment {selfAssessmentId} by user {controller.User.GetUserIdKnownNotNull()}");
                    context.Result = new RedirectToActionResult("AccessDenied", "LearningSolutions", new { code = 403 });
                }
            }
        }
    }
}
