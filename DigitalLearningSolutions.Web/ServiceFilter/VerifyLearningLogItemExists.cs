namespace DigitalLearningSolutions.Web.ServiceFilter
{
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Controllers.LearningPortalController;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;

    public class VerifyLearningLogItemExists : IActionFilter
    {
        private readonly ILearningLogItemsService learningLogItemsService;
        private readonly ILogger<LearningPortalController> logger;

        public VerifyLearningLogItemExists(
            ILearningLogItemsService learningLogItemsService,
            ILogger<LearningPortalController> logger
        )
        {
            this.learningLogItemsService = learningLogItemsService;
            this.logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context) { }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!(context.Controller is Controller controller))
            {
                return;
            }

            var learningLogItemId = int.Parse(context.RouteData.Values["learningLogItemId"].ToString()!);
            var learningLogItem = learningLogItemsService.SelectLearningLogItemById(learningLogItemId);

            if (learningLogItem == null)
            {
                logger.LogWarning(
                    $"Attempt to access learning log item with id {learningLogItemId} however found no item with that id"
                );

                context.Result = new NotFoundResult();
            }
        }
    }
}
