namespace DigitalLearningSolutions.Web.Controllers.Signposting
{
    using System;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.Signposting.LinkLearningHubSso;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Signposting.LinkLearningHubSso;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Route("Signposting/[Controller]")]
    [Authorize(Policy = CustomPolicies.UserOnly)]
    public class LinkLearningHubSsoController : Controller
    {
        private readonly ILearningHubSsoSecurityService learningHubSsoSecurityService;
        private readonly IUserService userService;

        public LinkLearningHubSsoController(
            ILearningHubSsoSecurityService learningHubSsoSecurityService,
            IUserService userService
        )
        {
            this.learningHubSsoSecurityService = learningHubSsoSecurityService;
            this.userService = userService;
        }

        public IActionResult Index([FromQuery] LinkLearningHubRequest linkLearningHubRequest)
        {
            if (!ModelState.IsValid)
            {
                throw new LearningHubLinkingRequestException("Invalid Learning Hub request.");
            }

            linkLearningHubRequest.SessionIdentifier = (Guid?)TempData.Peek(LinkLearningHubRequest.SessionIdentifierKey);
            var learningHubResourcedId =
                learningHubSsoSecurityService.ParseSsoAccountLinkingRequest(linkLearningHubRequest);

            userService.SetDelegateUserLearningHubAuthId(
                User.GetCandidateIdKnownNotNull(),
                linkLearningHubRequest.UserId
            );

            var model = new LinkLearningHubViewModel(learningHubResourcedId);
            return View("Index", model);
        }
    }
}
