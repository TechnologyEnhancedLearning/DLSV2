namespace DigitalLearningSolutions.Data.Helpers
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using DigitalLearningSolutions.Data.Services;
    using Microsoft.Extensions.Configuration;

    public interface ILearningHubSsoSecurityHelper
    {
        string GenerateHash(string state);

        bool VerifyHash(string state, string hash);
    }

    public class LearningHubSsoSecurityHelper : ILearningHubSsoSecurityHelper
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private readonly IClockService clockService;
        private readonly IConfiguration config;

        public LearningHubSsoSecurityHelper(IClockService clockService, IConfiguration config)
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
