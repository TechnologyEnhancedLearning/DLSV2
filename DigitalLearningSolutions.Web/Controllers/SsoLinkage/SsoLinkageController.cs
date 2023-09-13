using DigitalLearningSolutions.Data.Models.Signposting;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.Services;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DigitalLearningSolutions.Data.Exceptions;
using DigitalLearningSolutions.Web.ViewModels.Signposting;

namespace DigitalLearningSolutions.Web.Controllers.SsoLinkage
{
    public class SsoLinkageController : Controller
    {
        private readonly ILogger<SsoLinkageController> logger;
        private readonly IUserService userService;
        private readonly ILearningHubLinkService learningHubLinkService;

        public SsoLinkageController(
            ILogger<SsoLinkageController> logger,
            IUserService userService,
            ILearningHubLinkService learningHubLinkService)
        {
            this.logger = logger;
            this.userService = userService;
            this.learningHubLinkService = learningHubLinkService;
        }

        public IActionResult Index()
        {
            var delegateId = User.GetCandidateIdKnownNotNull();
            var learningHubAuthId = userService.GetDelegateUserLearningHubAuthId(delegateId);

            if (!learningHubAuthId.HasValue)
            {
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult LinkAccount()
        {
            var delegateId = User.GetCandidateIdKnownNotNull();
            var learningHubAuthId = userService.GetDelegateUserLearningHubAuthId(delegateId);

            if (!learningHubAuthId.HasValue)
            {
                var sessionLinkingId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString(LinkLearningHubRequest.SessionIdentifierKey, sessionLinkingId);

                var linkingUrl = learningHubLinkService.GetLinkingUrl(sessionLinkingId);
                return Redirect(linkingUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccountLinked([FromQuery] LinkLearningHubRequest linkLearningHubRequest)
        {
            if (!ModelState.IsValid)
            {
                throw new LearningHubLinkingRequestException("Invalid Learning Hub request.");
            }

            learningHubLinkService.ValidateLinkingRequest(
                linkLearningHubRequest,
                HttpContext.Session.GetString(LinkLearningHubRequest.SessionIdentifierKey)
            );

            learningHubLinkService.LinkLearningHubAccountIfNotLinked(
                User.GetCandidateIdKnownNotNull(),
                linkLearningHubRequest.UserId
                );

            return View();
        }
    }
}
