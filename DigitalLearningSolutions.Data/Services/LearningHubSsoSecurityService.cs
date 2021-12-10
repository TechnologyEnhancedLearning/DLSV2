namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using DigitalLearningSolutions.Data.Exceptions;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Signposting.LinkLearningHubSso;
    using Microsoft.Extensions.Configuration;

    public interface ILearningHubSsoSecurityService
    {
        string GenerateHash(string state);

        bool VerifyHash(string state, string hash);

        int? ParseSsoAccountLinkingRequest(LinkLearningHubRequest linkLearningHubRequest);
    }

    public class LearningHubSsoSecurityService : ILearningHubSsoSecurityService
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private readonly IClockService clockService;
        private readonly IConfiguration config;

        public LearningHubSsoSecurityService(IClockService clockService, IConfiguration config)
        {
            this.clockService = clockService;
            this.config = config;
        }

        public string GenerateHash(string state)
        {
            var secondsSinceEpoch = GetSecondsSinceEpoch();
            var encoder = new UTF8Encoding();
            var salt = encoder.GetBytes(config.GetLearningHubSsoSecretKey());
            var timedState = encoder.GetBytes(state + secondsSinceEpoch);
            return GetHash(timedState, salt);
        }

        public bool VerifyHash(string state, string hash)
        {
            var secondsSinceEpoch = GetSecondsSinceEpoch();
            var encoder = new UTF8Encoding();
            var salt = encoder.GetBytes(config.GetLearningHubSsoSecretKey());
            var toleranceInSec = config.GetLearningHubSsoHashTolerance();

            for (var counter = 0; counter <= toleranceInSec * 2; counter++)
            {
                var step = counter > toleranceInSec ? counter - toleranceInSec : -1 * counter;
                var timedState = encoder.GetBytes(state + (secondsSinceEpoch + step));

                if (hash == GetHash(timedState, salt))
                {
                    return true;
                }
            }

            return false;
        }

        public int? ParseSsoAccountLinkingRequest(LinkLearningHubRequest linkLearningHubRequest)
        {
            ValidateUserIdFromHash(linkLearningHubRequest);

            var parsedState = ParseState(linkLearningHubRequest);

            ValidateUserState(parsedState.sessionIdentifier, linkLearningHubRequest.SessionIdentifier);

            return parsedState.resourcesId;
        }

        private void ValidateUserIdFromHash(LinkLearningHubRequest linkLearningHubRequest)
        {
            var isVerified = VerifyHash(
                linkLearningHubRequest.UserId.ToString(),
                linkLearningHubRequest.Hash
            );

            if (!isVerified)
            {
                throw new LearningHubLinkingRequestException("Invalid Learning Hub UserId.");
            }
        }

        private void ValidateUserState(Guid receivedSessionIdentifier, Guid? storedSessionIdentifier)
        {
            if (!storedSessionIdentifier.HasValue || receivedSessionIdentifier != storedSessionIdentifier.Value)
            {
                throw new LearningHubLinkingRequestException("Invalid Learning Hub linking session.");
            }
        }

        private (Guid sessionIdentifier, int? resourcesId) ParseState(LinkLearningHubRequest linkLearningHubRequest)
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

            if (!int.TryParse(stateItems.ElementAt(1), out var validReferenceId))
            {
                return (validSessionIdentifier, null);
            }

            return (validSessionIdentifier, validReferenceId);
        }

        private string GetHash(byte[] input, byte[] salt)
        {
            using var byteResult = new Rfc2898DeriveBytes(
                input,
                salt,
                config.GetLearningHubSsoHashIterations(),
                HashAlgorithmName.SHA512
            );
            var hash = Convert.ToBase64String(
                byteResult.GetBytes(config.GetLearningHubSsoByteLength())
            );
            return hash;
        }

        private long GetSecondsSinceEpoch()
        {
            return (long)(clockService.UtcNow - UnixEpoch).TotalSeconds;
        }
    }
}
