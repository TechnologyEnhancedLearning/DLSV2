namespace DigitalLearningSolutions.Web.ServiceFilter
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Services;
    using Microsoft.Extensions.Logging;

    public class VerifyAdminUserCanAccessSelfAssessment : IActionFilter
    {
        private readonly ISelfAssessmentService selfAssessmentService;
        private readonly ILogger<VerifyAdminUserCanAccessSelfAssessment> logger;

        public VerifyAdminUserCanAccessSelfAssessment(ISelfAssessmentService selfAssessmentService,
            ILogger<VerifyAdminUserCanAccessSelfAssessment> logger)
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
            var adminCategoryId = controller.User.GetAdminCategoryId();
            var selfAssessmentId = int.Parse(context.ActionArguments["selfAssessmentId"].ToString()!);
            var selfAssessmentCategoryId = selfAssessmentService.GetSelfAssessmentCategoryId((selfAssessmentId));
            if (adminCategoryId > 0 && adminCategoryId != selfAssessmentCategoryId)
            {
                logger.LogWarning($"Attempt to access restricted self assessment {selfAssessmentId} by user {controller.User.GetUserIdKnownNotNull()}");
                context.Result = new RedirectToActionResult("StatusCode", "LearningSolutions", new { code = 403 });
            }
        }
    }
}
