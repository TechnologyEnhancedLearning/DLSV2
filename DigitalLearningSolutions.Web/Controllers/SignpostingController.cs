namespace DigitalLearningSolutions.Web.Controllers
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.ViewModels.Signposting;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policy = CustomPolicies.UserOnly)]
    [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
    [Route("Signposting")]
    public class SignpostingController : Controller
    {
        private readonly IActionPlanService actionPlanService;
        private readonly ILearningHubResourceService learningHubResourceService;
        private readonly IUserService userService;

        public SignpostingController(
            IUserService userService,
            ILearningHubResourceService learningHubResourceService,
            IActionPlanService actionPlanService
        )
        {
            this.userService = userService;
            this.learningHubResourceService = learningHubResourceService;
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

            var (resource, _) =
                await learningHubResourceService.GetResourceByReferenceId(resourceReferenceId);

            if (resource == null)
            {
                return NotFound();
            }

            var learningHubAccountIsLinked = userService.DelegateUserLearningHubAccountIsLinked(delegateId);

            // TODO in HEEDLS-744 add warning to user about out of date data
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
