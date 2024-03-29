﻿namespace DigitalLearningSolutions.Web.Controllers.Signposting
{
    using System;
    using System.Threading.Tasks;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.Signposting;
    using DigitalLearningSolutions.Web.Attributes;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.Models.Enums;
    using DigitalLearningSolutions.Web.Services;
    using DigitalLearningSolutions.Web.ViewModels.Signposting;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.UseSignposting)]
    [SetDlsSubApplication(nameof(DlsSubApplication.LearningPortal))]
    [Route("Signposting")]
    [Authorize(Policy = CustomPolicies.UserDelegateOnly)]
    public class SignpostingSsoController : Controller
    {
        private readonly ILearningHubLinkService learningHubLinkService;
        private readonly ILearningHubResourceService learningHubResourceService;
        private readonly IUserService userService;

        public SignpostingSsoController(
            ILearningHubLinkService learningHubLinkService,
            ILearningHubResourceService learningHubResourceService,
            IUserService userService
        )
        {
            this.learningHubLinkService = learningHubLinkService;
            this.learningHubResourceService = learningHubResourceService;
            this.userService = userService;
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
            return View("../LinkLearningHubSso", model);
        }

        [HttpGet("ViewResource/{resourceReferenceId}")]
        public async Task<IActionResult> ViewResource(int resourceReferenceId)
        {
            var delegateId = User.GetCandidateIdKnownNotNull();
            var learningHubAuthId = userService.GetDelegateUserLearningHubAuthId(delegateId);

            if (!learningHubAuthId.HasValue)
            {
                var sessionLinkingId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString(LinkLearningHubRequest.SessionIdentifierKey, sessionLinkingId);

                var linkingUrl = learningHubLinkService.GetLinkingUrlForResource(resourceReferenceId, sessionLinkingId);
                return Redirect(linkingUrl);
            }

            var (resource, _) = await learningHubResourceService.GetResourceByReferenceId(resourceReferenceId);
            var resourceUrl = resource?.Link;

            if (string.IsNullOrEmpty(resourceUrl))
            {
                return NotFound();
            }

            var loginUrl =
                learningHubLinkService.GetLoginUrlForDelegateAuthIdAndResourceUrl(
                    resourceUrl,
                    learningHubAuthId!.Value
                );
            return Redirect(loginUrl);
        }
    }
}
