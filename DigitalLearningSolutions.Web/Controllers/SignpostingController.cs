namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.ApiClients;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Signposting;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policy = CustomPolicies.UserOnly)]
    [Route("Signposting")]
    public class SignpostingController : Controller
    {
        private readonly IActionPlanService actionPlanService;
        private readonly ILearningHubApiClient learningHubApiClient;
        private readonly IUserService userService;

        public SignpostingController(
            IUserService userService,
            ILearningHubApiClient learningHubApiClient,
            IActionPlanService actionPlanService
        )
        {
            this.userService = userService;
            this.learningHubApiClient = learningHubApiClient;
            this.actionPlanService = actionPlanService;
        }

        [Route("ViewResource/{resourceReferenceId:int}")]
        public IActionResult ViewResource(int resourceReferenceId)
        {
            // TODO: HEEDLS-678 - configure this action to redirect to Learning Hub
            return RedirectToAction("Current", "LearningPortal");
        }

        [HttpGet]
        [Route("LaunchLearningResource/{resourceReferenceId:int}")]
        public async Task<IActionResult> LaunchLearningResource(int resourceReferenceId)
        {
            var delegateId = User.GetCandidateIdKnownNotNull();
            actionPlanService.UpdateActionPlanResourcesLastAccessedDateIfPresent(resourceReferenceId, delegateId);

            var delegateUser = userService.GetDelegateUserById(delegateId);

            if (delegateUser!.HasDismissedLhLoginWarning)
            {
                return RedirectToAction("ViewResource", "Signposting", new { resourceReferenceId });
            }

            // TODO: HEEDLS-707 - handle case where resource reference Id does not match any resource
            var resource = await learningHubApiClient.GetResourceByReferenceId(resourceReferenceId);
            var learningHubAccountIsLinked = userService.DelegateUserLearningHubAccountIsLinked(delegateId);

            var model = new LearningHubLoginWarningViewModel(resource, learningHubAccountIsLinked);

            return View("LearningHubLoginWarning", model);
        }

        [HttpPost]
        [Route("LaunchLearningResource/{resourceReferenceId:int}")]
        public IActionResult LaunchLearningResource(int resourceReferenceId, LearningHubLoginWarningViewModel model)
        {
            if (model.LearningHubLoginWarningDismissed)
            {
                var delegateId = User.GetCandidateIdKnownNotNull();
                userService.UpdateDelegateLhLoginWarningDismissalStatus(delegateId, true);
            }

            return RedirectToAction("ViewResource", "Signposting", new { resourceReferenceId });
        }
    }
}
