﻿namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Web;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.Signposting;
    using Microsoft.Extensions.Configuration;
    using ConfigurationExtensions = DigitalLearningSolutions.Data.Extensions.ConfigurationExtensions;

    public interface ILearningHubLinkService
    {
        int? ValidateLinkingRequestAndExtractDestinationResourceId(
            LinkLearningHubRequest linkLearningHubRequest,
            string linkRequestSessionIdentifier
        );
        void ValidateLinkingRequest(
            LinkLearningHubRequest linkLearningHubRequest,
            string linkRequestSessionIdentifier
        );

        bool IsLearningHubAccountLinked(int delegateId);

        bool IsLearningHubUserAccountLinked(int userId);

        void LinkLearningHubAccountIfNotLinked(int delegateId, int learningHubUserId);

        void LinkLearningHubUserAccountIfNotLinked(int userId, int learningHubUserId);

        string GetLoginUrlForDelegateAuthIdAndResourceUrl(string resourceUrl, int authId);

        string GetLinkingUrlForResource(int resourceReferenceId, string sessionLinkingId);

        string GetLinkingUrl(string sessionLinkingId);
    }

    public class LearningHubLinkService : ILearningHubLinkService
    {
        private readonly IConfiguration config;
        private readonly ILearningHubSsoSecurityService learningHubSsoSecurityService;
        private readonly IUserDataService userDataService;

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

        private string AuthBaseUrl => ConfigurationExtensions.GetLearningHubAuthApiBaseUrl(config);
        private string ClientCode => ConfigurationExtensions.GetLearningHubAuthApiClientCode(config);
        private string ClientCodeSso => ConfigurationExtensions.GetLearningHubAuthApiSsoClientCode(config);

        public bool IsLearningHubAccountLinked(int delegateId)
        {
            return userDataService.GetDelegateUserLearningHubAuthId(delegateId).HasValue;
        }

        public bool IsLearningHubUserAccountLinked(int userId)
        {
            return userDataService.GetUserLearningHubAuthId(userId).HasValue;
        }

        public void LinkLearningHubAccountIfNotLinked(int delegateId, int learningHubUserId)
        {
            var isAccountAlreadyLinked = IsLearningHubAccountLinked(delegateId);
            if (!isAccountAlreadyLinked)
            {
                userDataService.SetDelegateUserLearningHubAuthId(delegateId, learningHubUserId);
            }
        }

        public void LinkLearningHubUserAccountIfNotLinked(int userId, int learningHubUserId)
        {
            var isAccountAlreadyLinked = IsLearningHubUserAccountLinked(userId);
            if (!isAccountAlreadyLinked)
            {
                userDataService.SetUserLearningHubAuthId(userId, learningHubUserId);
            }
        }

        public int? ValidateLinkingRequestAndExtractDestinationResourceId(
            LinkLearningHubRequest linkLearningHubRequest,
            string linkRequestSessionIdentifier
        )
        {
            ParseIdentifierValidateId(linkLearningHubRequest,
                linkRequestSessionIdentifier,
                out var storedSessionIdentifier);

            var parsedState = ParseAccountLinkingRequest(linkLearningHubRequest, storedSessionIdentifier);

            return parsedState.resourceId;
        }

        public void ValidateLinkingRequest(
            LinkLearningHubRequest linkLearningHubRequest,
            string linkRequestSessionIdentifier
            )
        {
            ParseIdentifierValidateId(linkLearningHubRequest,
                linkRequestSessionIdentifier,
                out var storedSessionIdentifier);
        }

        private void ParseIdentifierValidateId(LinkLearningHubRequest linkLearningHubRequest,
            string linkRequestSessionIdentifier,
            out Guid storedSessionIdentifier)
        {
            if (!Guid.TryParse(linkRequestSessionIdentifier, out storedSessionIdentifier))
            {
                throw new LearningHubLinkingRequestException(
                    "Invalid Learning Hub linking request session identifier."
                );
            }

            ValidateLearningHubUserId(linkLearningHubRequest);
        }

        public string GetLoginUrlForDelegateAuthIdAndResourceUrl(string resourceUrl, int authId)
        {
            var idHash = learningHubSsoSecurityService.GenerateHash(authId.ToString());
            var loginQueryString =
                ComposeLoginQueryString(ClientCode, authId, idHash, resourceUrl);

            var loginEndpoint = ConfigurationExtensions.GetLearningHubAuthApiLoginEndpoint(config);

            return AuthBaseUrl + loginEndpoint + loginQueryString;
        }

        public string GetLinkingUrlForResource(int resourceReferenceId, string sessionLinkingId)
        {
            return CreateLinkingUrl(resourceReferenceId, sessionLinkingId, ClientCode);
        }

        public string GetLinkingUrl(string sessionLinkingId)
        {
            return CreateLinkingUrl(0, sessionLinkingId, ClientCodeSso);
        }

        private string CreateLinkingUrl(int resourceReferenceId, string sessionLinkingId, string clientCode)
        {
            var state = ComposeCreateUserState(resourceReferenceId, sessionLinkingId);
            var stateHash = learningHubSsoSecurityService.GenerateHash(state);
            return AuthBaseUrl
                + ConfigurationExtensions.GetLearningHubAuthApiLinkingEndpoint(config)
                + ComposeCreateUserQueryString(clientCode, state, stateHash);
        }

        private static string ComposeCreateUserState(int resourceReferenceId, string sessionLinkingId)
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
            var encodedHash = HttpUtility.UrlEncode(idHash);
            return $"?clientCode={clientCode}&userId={learningHubAuthId}&hash={encodedHash}&endClientUrl={encodedUrl}";
        }

        private static string ComposeCreateUserQueryString(string clientCode, string state, string stateHash)
        {
            var encodedState = HttpUtility.UrlEncode(state);
            var encodedHash = HttpUtility.UrlEncode(stateHash);

            return $"?clientCode={clientCode}&state={encodedState}&hash={encodedHash}";
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
