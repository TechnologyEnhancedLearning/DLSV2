namespace DigitalLearningSolutions.Web.Controllers.Signposting
{
    using System.Web;
    using DigitalLearningSolutions.Data.DataServices;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Services;
    using DigitalLearningSolutions.Web.Helpers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.FeatureManagement.Mvc;

    [FeatureGate(FeatureFlags.UseSignposting)]
    public class ViewResourceController : Controller
    {
        private readonly ILearningResourceReferenceDataService learningResourceReferenceDataService;
        private readonly ILearningHubAuthApiUrlService learningHubAuthApiUrlService;

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

            if (learningHubAuthId.HasValue)
            {
                var resourceUrl =
                    learningResourceReferenceDataService.GetLearningHubResourceLinkById(resourceReferenceId);

                if (string.IsNullOrEmpty(resourceUrl))
                {
                    return new NotFoundResult();
                }

                var loginUrl =
                    learningHubAuthApiUrlService.GetLoginUrlForDelegateAuthIdAndResourceUrl(
                        resourceUrl,
                        learningHubAuthId!.Value
                    );
                return Redirect(loginUrl);
            }

            var linkingUrl = learningHubAuthApiUrlService.GetLinkingUrlForResource(resourceReferenceId);
            return Redirect(linkingUrl);
        }
    }
}
