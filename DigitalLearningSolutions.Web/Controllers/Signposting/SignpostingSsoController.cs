namespace DigitalLearningSolutions.Web.Controllers.Signposting
{
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.Signposting;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.SignpostingSso;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.UseSignposting)]
    [Route("Signposting")]
    [Authorize(Policy = CustomPolicies.UserOnly)]
    public class SignpostingSsoController : Controller
    {
        private readonly ILearningHubLinkService learningHubLinkService;

        public SignpostingSsoController(
            ILearningHubLinkService learningHubLinkService
        )
        {
            this.learningHubLinkService = learningHubLinkService;
        }

        [HttpGet("LinkLearningHubSso")]
        public IActionResult LinkLearningHubSso([FromQuery] LinkLearningHubRequest linkLearningHubRequest)
        {
            if (!ModelState.IsValid)
            {
                throw new LearningHubLinkingRequestException("Invalid Learning Hub request.");
            }

            var learningHubResourcedId = learningHubLinkService.ValidateLinkingRequestAndExtractDestinationResourceId(
                linkLearningHubRequest,
                HttpContext.Session.GetString(LinkLearningHubRequest.SessionIdentifierKey)
            );

            var delegateId = User.GetCandidateIdKnownNotNull();
            var isAccountAlreadyLinked = learningHubLinkService.IsLearningHubAccountLinked(delegateId);
            learningHubLinkService.LinkLearningHubAccountIfNotLinked(delegateId, linkLearningHubRequest.UserId);

            var model = new LinkLearningHubViewModel(isAccountAlreadyLinked, learningHubResourcedId);
            return View(model);
        }
    }
}
