namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using DigitalLearningSolutions.Data.DataServices.UserDataService;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Models.Signposting;

    public interface ILearningHubLinkService
    {
        int? ValidateLinkingRequestAndExtractDestinationResourceId(
            LinkLearningHubRequest linkLearningHubRequest,
            string linkRequestSessionIdentifier
        );

        bool IsLearningHubAccountLinked(int delegateId);

        void LinkLearningHubAccountIfNotLinked(int delegateId, int learningHubUserId);
    }

    public class LearningHubLinkService : ILearningHubLinkService
    {
        private readonly ILearningHubSsoSecurityService learningHubSsoSecurityService;
        private readonly IUserDataService userDataService;

        public LearningHubLinkService(
            ILearningHubSsoSecurityService learningHubSsoSecurityService,
            IUserDataService userDataService
        )
        {
            this.learningHubSsoSecurityService = learningHubSsoSecurityService;
            this.userDataService = userDataService;
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
