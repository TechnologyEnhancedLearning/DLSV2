namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Web;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Signposting;
    using Microsoft.Extensions.Configuration;

    public interface ILearningHubLinkService
    {
        int? ValidateLinkingRequestAndExtractDestinationResourceId(
            LinkLearningHubRequest linkLearningHubRequest,
            string linkRequestSessionIdentifier
        );

        bool IsLearningHubAccountLinked(int delegateId);

        void LinkLearningHubAccountIfNotLinked(int delegateId, int learningHubUserId);

        string GetLoginUrlForDelegateAuthIdAndResourceUrl(string resourceUrl, int delegateAuthId);

        string GetLinkingUrlForResource(int resourceReferenceId, string sessionLinkingId);
    }

    public class LearningHubLinkService : ILearningHubLinkService
    {
        private readonly IConfiguration config;
        private readonly ILearningHubSsoSecurityService learningHubSsoSecurityService;
        private readonly IUserDataService userDataService;

        private string AuthBaseUrl => config.GetLearningHubAuthApiBaseUrl();
        private string ClientCode => config.GetLearningHubAuthApiClientCode();

        public LearningHubLinkService(
            ILearningHubSsoSecurityService learningHubSsoSecurityService,
            IUserDataService userDataService,
            IConfiguration config
        )
        {
            this.learningHubSsoSecurityService = learningHubSsoSecurityService;
            this.userDataService = userDataService;
            this.config = config;
        }

        public bool IsLearningHubAccountLinked(int delegateId)
        {
            return userDataService.GetDelegateUserLearningHubAuthId(delegateId).HasValue;
        }

        public void LinkLearningHubAccountIfNotLinked(int delegateId, int learningHubUserId)
        {
            var isAccountAlreadyLinked = IsLearningHubAccountLinked(delegateId);
            if (!isAccountAlreadyLinked)
            {
                userDataService.SetDelegateUserLearningHubAuthId(delegateId, learningHubUserId);
            }
        }

        public int? ValidateLinkingRequestAndExtractDestinationResourceId(
            LinkLearningHubRequest linkLearningHubRequest,
            string linkRequestSessionIdentifier
        )
        {
            if (!Guid.TryParse(linkRequestSessionIdentifier, out var storedSessionIdentifier))
            {
                throw new LearningHubLinkingRequestException(
                    "Invalid Learning Hub linking request session identifier."
                );
            }

            ValidateLearningHubUserId(linkLearningHubRequest);

            var parsedState = ParseAccountLinkingRequest(linkLearningHubRequest, storedSessionIdentifier);

            return parsedState.resourceId;
        }

        public string GetLoginUrlForDelegateAuthIdAndResourceUrl(string resourceUrl, int delegateAuthId)
        {
            var idHash = learningHubSsoSecurityService.GenerateHash(delegateAuthId.ToString());
            var loginQueryString =
                ComposeLoginQueryString(ClientCode, delegateAuthId, idHash, resourceUrl);

            var loginEndpoint = config.GetLearningHubAuthApiLoginEndpoint();

            return AuthBaseUrl + loginEndpoint + loginQueryString;
        }

        public string GetLinkingUrlForResource(int resourceReferenceId, string sessionLinkingId)
        {
            var state = ComposeCreateUserState(resourceReferenceId, sessionLinkingId);
            var stateHash = learningHubSsoSecurityService.GenerateHash(state);
            var createUserQueryString = ComposeCreateUserQueryString(ClientCode, state, stateHash);

            var linkingEndpoint = config.GetLearningHubAuthApiLinkingEndpoint();

            return AuthBaseUrl + linkingEndpoint + createUserQueryString;
        }

        private string ComposeCreateUserState(int resourceReferenceId, string sessionLinkingId)
        {
            return $"{sessionLinkingId}_refId:{resourceReferenceId}";
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

        private void ValidateLearningHubUserId(LinkLearningHubRequest linkLearningHubRequest)
        {
            var isVerified = learningHubSsoSecurityService.VerifyHash(
                linkLearningHubRequest.UserId.ToString(),
                linkLearningHubRequest.Hash
            );

            if (!isVerified)
            {
                throw new LearningHubLinkingRequestException("Invalid Learning Hub UserId hash.");
            }
        }

        private (Guid sessionIdentifier, int? resourceId) ParseAccountLinkingRequest(
            LinkLearningHubRequest linkLearningHubRequest,
            Guid storedSessionIdentifier
        )
        {
            var stateItems = linkLearningHubRequest.State.Split("_refId:");

            if (stateItems.Length != 2)
            {
                throw new LearningHubLinkingRequestException("Invalid Learning Hub linking state.");
            }

            if (!Guid.TryParse(stateItems[0], out var validSessionIdentifier) ||
                validSessionIdentifier != storedSessionIdentifier)
            {
                throw new LearningHubLinkingRequestException("Invalid Learning Hub linking session.");
            }

            if (!int.TryParse(stateItems[1], out var validReferenceId))
            {
                return (validSessionIdentifier, null);
            }

            return (validSessionIdentifier, validReferenceId);
        }
    }
}
