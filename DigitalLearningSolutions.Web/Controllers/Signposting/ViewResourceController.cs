namespace DigitalLearningSolutions.Web.Controllers.Signposting
{
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.UseSignposting)]
    [Authorize(Policy = CustomPolicies.UserOnly)]
    public class ViewResourceController : Controller
    {
        private readonly ILearningHubAuthApiUrlService learningHubAuthApiUrlService;
        private readonly ILearningResourceReferenceDataService learningResourceReferenceDataService;

        private readonly IUserService userService;

        public ViewResourceController(
            IUserService userService,
            ILearningResourceReferenceDataService learningResourceReferenceDataService,
            ILearningHubAuthApiUrlService learningHubAuthApiUrlService
        )
        {
            this.userService = userService;
            this.learningResourceReferenceDataService = learningResourceReferenceDataService;
            this.learningHubAuthApiUrlService = learningHubAuthApiUrlService;
        }

        [Route("Signposting/ViewResource/{resourceReferenceId}")]
        public IActionResult Index(int resourceReferenceId)
        {
            var delegateId = User.GetCandidateIdKnownNotNull();
            var learningHubAuthId = userService.GetDelegateUserLearningHubAuthId(delegateId);

            if (!learningHubAuthId.HasValue)
            {
                var linkingUrl = learningHubAuthApiUrlService.GetLinkingUrlForResource(resourceReferenceId);
                return Redirect(linkingUrl);
            }

            var resourceUrl =
                learningResourceReferenceDataService.GetLearningHubResourceLinkByResourceRefId(resourceReferenceId);

            if (string.IsNullOrEmpty(resourceUrl))
            {
                return NotFound();
            }

            var loginUrl =
                learningHubAuthApiUrlService.GetLoginUrlForDelegateAuthIdAndResourceUrl(
                    resourceUrl,
                    learningHubAuthId!.Value
                );
            return Redirect(loginUrl);
        }
    }
}
