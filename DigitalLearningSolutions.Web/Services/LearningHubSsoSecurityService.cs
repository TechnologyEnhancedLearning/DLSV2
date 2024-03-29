﻿namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using DigitalLearningSolutions.Data.Utilities;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using ConfigurationExtensions = DigitalLearningSolutions.Data.Extensions.ConfigurationExtensions;

    public interface ILearningHubSsoSecurityService
    {
        string GenerateHash(string state);

        bool VerifyHash(string state, string hash);
    }

    public class LearningHubSsoSecurityService : ILearningHubSsoSecurityService
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private readonly IClockUtility clockUtility;
        private readonly IConfiguration config;

        private ILogger<ILearningHubSsoSecurityService> logger;

        public LearningHubSsoSecurityService(IClockUtility clockUtility, IConfiguration config, ILogger<ILearningHubSsoSecurityService> logger)
        {
            this.clockUtility = clockUtility;
            this.config = config;
            this.logger = logger;
        }

        public string GenerateHash(string state)
        {
            var secondsSinceEpoch = GetSecondsSinceEpoch();
            var salt = GetSecretKeyBytes();

            var encoder = new UTF8Encoding();
            var timedState = encoder.GetBytes(state + secondsSinceEpoch);
            return GetHash(timedState, salt);
        }

        public bool VerifyHash(string state, string hash)
        {
            var secondsSinceEpoch = GetSecondsSinceEpoch();
            var salt = GetSecretKeyBytes();

            var encoder = new UTF8Encoding();
            var toleranceInSec = ConfigurationExtensions.GetLearningHubSsoHashTolerance(config);
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

        private byte[] GetSecretKeyBytes()
        {
            var encoder = new UTF8Encoding();
            var secretKeyBytes = encoder.GetBytes(ConfigurationExtensions.GetLearningHubSsoSecretKey(config));

            if (secretKeyBytes.Length < 8)
            {
                logger.LogError("Secret key invalid. The secret key must have a length of at least 8 characters.");
                throw new CryptographicException();
            }

            return secretKeyBytes;
        }

        private string GetHash(byte[] input, byte[] salt)
        {
            using var byteResult = new Rfc2898DeriveBytes(
                input,
                salt,
                ConfigurationExtensions.GetLearningHubSsoHashIterations(config),
                HashAlgorithmName.SHA512
            );
            var hash = Convert.ToBase64String(
                byteResult.GetBytes(ConfigurationExtensions.GetLearningHubSsoByteLength(config))
            );
            return hash;
        }

        private long GetSecondsSinceEpoch()
        {
            return (long)(clockUtility.UtcNow - UnixEpoch).TotalSeconds;
        }
    }
}
