using System;
using DigitalLearningSolutions.Data.Exceptions;
using DigitalLearningSolutions.Data.Models.Signposting;
using DigitalLearningSolutions.Web.Attributes;
using DigitalLearningSolutions.Web.Helpers;
using DigitalLearningSolutions.Web.Models.Enums;
using DigitalLearningSolutions.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DigitalLearningSolutions.Web.Controllers
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

        [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
        public IActionResult Index()
        {
            var learningHubAuthId = GetLearningHubAuthId();

            if (!learningHubAuthId.HasValue)
            {
                return View();
            }
            return RedirectToAction(
                "Index",
                "Home");
        }

        [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
        public IActionResult LinkAccount()
        {
            var learningHubAuthId = GetLearningHubAuthId();

            if (!learningHubAuthId.HasValue)
            {
                var sessionLinkingId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString(
                    LinkLearningHubRequest.SessionIdentifierKey,
                    sessionLinkingId);

                var linkingUrl = learningHubLinkService.GetLinkingUrl(sessionLinkingId);
                return Redirect(linkingUrl);
            }
            return RedirectToAction(
                "Index",
                "Home");
        }

        [SetDlsSubApplication(nameof(DlsSubApplication.Main))]
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

            var delegateId = User.GetCandidateId();
            if (delegateId.HasValue)
            {
                learningHubLinkService.LinkLearningHubAccountIfNotLinked(
                delegateId.Value,
                linkLearningHubRequest.UserId
                );
            }
            else
            {
                learningHubLinkService.LinkLearningHubUserAccountIfNotLinked(
                   User.GetUserId().Value,
                   linkLearningHubRequest.UserId
                   );
            }
            return View();
        }

        private int? GetLearningHubAuthId()
        {
            var delegateId = User.GetCandidateId();
            int? learningHubAuthId = null;
            if (delegateId != null)
            {
                learningHubAuthId = userService.GetDelegateUserLearningHubAuthId(delegateId.Value);
            }
            else
            {
                var userId = User.GetUserId();
                learningHubAuthId = userService.GetUserLearningHubAuthId(userId.Value);
            }
            return learningHubAuthId;
        }
    }
}
