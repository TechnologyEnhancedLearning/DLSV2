namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Linq;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Signposting.LinkLearningHubSso;

    public interface ILearningHubSsoService
    {
        int? ParseSsoAccountLinkingRequest(LinkLearningHubRequest linkLearningHubRequest);
    }

    public class LearningHubSsoService : ILearningHubSsoService
    {
        private readonly ILearningHubSsoSecurityHelper learningHubSsoSecurityHelper;

        public LearningHubSsoService(ILearningHubSsoSecurityHelper learningHubSsoSecurityHelper)
        {
            this.learningHubSsoSecurityHelper = learningHubSsoSecurityHelper;
        }

        public int? ParseSsoAccountLinkingRequest(LinkLearningHubRequest linkLearningHubRequest)
        {
            ValidateUserIdFromHash(linkLearningHubRequest);

            var parsedState = ParseState(linkLearningHubRequest);

            ValidateUserState(parsedState.sessionIdentifier, linkLearningHubRequest.SessionIdentifier);

            return parsedState.resourcesId;
        }

        public void ValidateUserIdFromHash(LinkLearningHubRequest linkLearningHubRequest)
        {
            var isVerified = learningHubSsoSecurityHelper.VerifyHash(
                linkLearningHubRequest.UserId.ToString(),
                linkLearningHubRequest.Hash
            );

            if (!isVerified)
            {
                throw new LearningHubLinkingRequestException("Invalid Learning Hub UserId.");
            }
        }

        public void ValidateUserState(Guid receivedSessionIdentifier, Guid? storedSessionIdentifier)
        {
            if (!storedSessionIdentifier.HasValue || receivedSessionIdentifier != storedSessionIdentifier.Value)
            {
                throw new LearningHubLinkingRequestException("Invalid Learning Hub linking session.");
            }
        }

        public (Guid sessionIdentifier, int? resourcesId) ParseState(LinkLearningHubRequest linkLearningHubRequest)
        {
            var stateItems = linkLearningHubRequest.State.Split("_refId:");

            if (stateItems.Length != 2)
            {
                throw new LearningHubLinkingRequestException("Invalid Learning Hub linking state.");
            }

            if (!Guid.TryParse(stateItems.ElementAt(0), out var validSessionIdentifier))
            {
                throw new LearningHubLinkingRequestException("Invalid Learning Hub linking session.");
            }

            if(!int.TryParse(stateItems.ElementAt(1), out var validReferenceId))
            {
                return (validSessionIdentifier, null);
            }

            return (validSessionIdentifier, validReferenceId);
        }
    }
}
