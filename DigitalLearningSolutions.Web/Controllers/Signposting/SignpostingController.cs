namespace DigitalLearningSolutions.Web.Controllers.Signposting
{
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Signposting;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [Authorize(Policy = CustomPolicies.UserOnly)]
    [FeatureGate(FeatureFlags.UseSignposting)]
    [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
    [Route("Signposting/LaunchLearningResource/{resourceReferenceId:int}")]
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

        [HttpGet]
        public async Task<IActionResult> LaunchLearningResource(int resourceReferenceId)
        {
            var delegateId = User.GetCandidateIdKnownNotNull();
            actionPlanService.UpdateActionPlanResourcesLastAccessedDateIfPresent(resourceReferenceId, delegateId);

            var delegateUser = userService.GetDelegateUserById(delegateId);

            var (resource, apiIsAccessible) =
                await learningHubResourceService.GetResourceByReferenceIdAndPopulateDeletedDetailsFromDatabase(
                    resourceReferenceId
                );

            if (resource == null || resource.AbsentInLearningHub)
            {
                return NotFound();
            }

            if (delegateUser!.HasDismissedLhLoginWarning)
            {
                return RedirectToAction("ViewResource", "SignpostingSso", new { resourceReferenceId });
            }

            var learningHubAccountIsLinked = userService.DelegateUserLearningHubAccountIsLinked(delegateId);

            var model = new LearningHubLoginWarningViewModel(
                resource,
                learningHubAccountIsLinked,
                apiIsAccessible
            );

            return View("LearningHubLoginWarning", model);
        }

        [HttpPost]
        public IActionResult LaunchLearningResource(int resourceReferenceId, LearningHubLoginWarningViewModel model)
        {
            if (model.LearningHubLoginWarningDismissed)
            {
                var delegateId = User.GetCandidateIdKnownNotNull();
                userService.UpdateDelegateLhLoginWarningDismissalStatus(delegateId, true);
            }

            return RedirectToAction("ViewResource", "SignpostingSso", new { resourceReferenceId });
        }
    }
}
