﻿namespace DigitalLearningSolutions.Web.Controllers.Signposting
{
    using System;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.Signposting.LinkLearningHubSso;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using DigitalLearningSolutions.Web.ViewModels.Signposting.LinkLearningHubSso;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.UseSignposting)]
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

            linkLearningHubRequest.SessionIdentifier = Guid.Parse(
                HttpContext.Session.GetString(LinkLearningHubRequest.SessionIdentifierKey)
            );
            var learningHubResourcedId =
                learningHubSsoSecurityService.ParseSsoAccountLinkingRequest(linkLearningHubRequest);

            var delegateId = User.GetCandidateIdKnownNotNull();
            var isAccountAlreadyLinked = userService.DelegateUserLearningHubAccountIsLinked(delegateId);
            if (!isAccountAlreadyLinked)
            {
                userService.SetDelegateUserLearningHubAuthId(
                    delegateId,
                    linkLearningHubRequest.UserId
                );
            }

            var model = new LinkLearningHubViewModel(isAccountAlreadyLinked, learningHubResourcedId);
            return View("Index", model);
        }
    }
}
