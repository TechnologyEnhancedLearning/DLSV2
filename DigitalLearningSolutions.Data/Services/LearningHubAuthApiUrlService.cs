namespace DigitalLearningSolutions.Data.Services
{
    using System.Web;
    using DigitalLearningSolutions.Data.Helpers;
    using Microsoft.Extensions.Configuration;

    public interface ILearningHubAuthApiUrlService
    {
        string GetLoginUrlForDelegateAuthIdAndResourceUrl(string resourceUrl, int delegateAuthId);

        string GetLinkingUrlForResource(int resourceReferenceId);
    }

    public class LearningHubAuthApiUrlService : ILearningHubAuthApiUrlService
    {
        private readonly IConfiguration config;
        private readonly IGuidService guidService;
        private readonly ILearningHubSsoSecurityService learningHubSsoSecurityService;

        public LearningHubAuthApiUrlService(
            IConfiguration config,
            IGuidService guidService,
            ILearningHubSsoSecurityService learningHubSsoSecurityService
        )
        {
            this.config = config;
            this.guidService = guidService;
            this.learningHubSsoSecurityService = learningHubSsoSecurityService;
        }

        private string AuthEndpoint => config.GetLearningHubAuthApiBaseUrl();
        private string ClientCode => config.GetLearningHubAuthApiClientCode();

        public string GetLoginUrlForDelegateAuthIdAndResourceUrl(string resourceUrl, int delegateAuthId)
        {
            var idHash = learningHubSsoSecurityService.GenerateHash(delegateAuthId.ToString());
            var loginQueryString =
                ComposeLoginQueryString(ClientCode, delegateAuthId, idHash, resourceUrl);

            var loginEndpoint = config.GetLearningHubAuthApiLoginEndpoint();
            return AuthEndpoint + loginEndpoint + loginQueryString;
        }

        public string GetLinkingUrlForResource(int resourceReferenceId)
        {
            var state = ComposeCreateUserState(resourceReferenceId);
            var stateHash = learningHubSsoSecurityService.GenerateHash(state);

            var createUserQueryString = ComposeCreateUserQueryString(ClientCode, state, stateHash);

            var linkingEndpoint = config.GetLearningHubAuthApiLinkingEndpoint();

            return AuthEndpoint + linkingEndpoint + createUserQueryString;
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
