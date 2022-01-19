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
        private const string LoginEndpointRelativePath = "/login";
        private const string CreateUserEndpointRelativePath = "/create-user";
        private readonly IConfiguration config;
        private readonly IGuidService guidService;
        private readonly ILearningHubSsoSecurityService learningHubSsoSecurityService;
        private readonly ILearningResourceReferenceDataService learningResourceReferenceDataService;

        private readonly IUserService userService;

        public ViewResourceController(
            IUserService userService,
            ILearningResourceReferenceDataService learningResourceReferenceDataService,
            ILearningHubSsoSecurityService learningHubSsoSecurityService,
            IGuidService guidService,
            IConfiguration config
        )
        {
            this.userService = userService;
            this.learningResourceReferenceDataService = learningResourceReferenceDataService;
            this.learningHubSsoSecurityService = learningHubSsoSecurityService;
            this.guidService = guidService;
            this.config = config;
        }

        [Route("Signposting/ViewResource/{resourceReferenceId}")]
        public IActionResult Index(int resourceReferenceId)
        {
            var delegateId = User.GetCandidateIdKnownNotNull();

            var learningHubAuthId = userService.GetDelegateUserLearningHubAuthId(delegateId);

            var authEndpoint = config.GetLearningHubAuthApiBaseUrl();

            var clientCode = config.GetLearningHubAuthApiClientCode();

            if (learningHubAuthId.HasValue)
            {
                var resourceUrl =
                    learningResourceReferenceDataService.GetLearningHubResourceLinkById(resourceReferenceId);

                if (string.IsNullOrEmpty(resourceUrl))
                {
                    return new NotFoundResult();
                }

                var idHash = learningHubSsoSecurityService.GenerateHash(learningHubAuthId.ToString());

                var loginQueryString =
                    ComposeLoginQueryString(clientCode, learningHubAuthId, idHash, resourceUrl);

                return Redirect(authEndpoint + LoginEndpointRelativePath + loginQueryString);
            }

            var state = ComposeCreateUserState(resourceReferenceId);

            var stateHash = learningHubSsoSecurityService.GenerateHash(state);

            var createUserQueryString = ComposeCreateUserQueryString(clientCode, state, stateHash);

            return Redirect(authEndpoint + CreateUserEndpointRelativePath + createUserQueryString);
        }

        private string ComposeCreateUserState(int resourceReferenceId)
        {
            return guidService.NewGuid() + $"_refId:{resourceReferenceId}";
        }

        private static string ComposeLoginQueryString(
            string clientCode,
            int? learningHubAuthId,
            string idHash,
            string resourceUrl
        )
        {
            var encodedUrl = HttpUtility.UrlEncode(resourceUrl);

            return $"?clientcode={clientCode}&userid={learningHubAuthId}&hash={idHash}&endclientUrl={encodedUrl}";
        }

        private static string ComposeCreateUserQueryString(string clientCode, string state, string stateHash)
        {
            return $"?clientcode={clientCode}&state={state}&hash={stateHash}";
        }
    }
}
