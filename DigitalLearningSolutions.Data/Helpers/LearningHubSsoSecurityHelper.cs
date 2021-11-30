namespace DigitalLearningSolutions.Data.Helpers
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using DigitalLearningSolutions.Data.Services;

    public interface ILearningHubSsoSecurityHelper
    {
        string GenerateHash(string state, string secretKey);

        bool VerifyHash(string state, string secretKey, string hash);
    }

    public class LearningHubSsoSecurityHelper : ILearningHubSsoSecurityHelper
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private readonly IClockService clockService;

        public LearningHubSsoSecurityHelper(IClockService clockService)
        {
            this.clockService = clockService;
        }

        public string GenerateHash(string state, string secretKey)
        {
            var secondsSinceEpoch = GetSecondsSinceEpoch();
            var encoder = new UTF8Encoding();
            var salt = encoder.GetBytes(secretKey);
            var timedState = encoder.GetBytes(state + secondsSinceEpoch);
            return GetHash(timedState, salt);
        }

        public bool VerifyHash(string state, string secretKey, string hash)
        {
            var secondsSinceEpoch = GetSecondsSinceEpoch();
            var encoder = new UTF8Encoding();
            var salt = encoder.GetBytes(secretKey);
            var toleranceInSec = 60;

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

        private static string GetHash(byte[] input, byte[] salt)
        {
            using var byteResult = new Rfc2898DeriveBytes(input, salt, 10000, HashAlgorithmName.SHA512);
            var hash = Convert.ToBase64String(byteResult.GetBytes(32));
            return hash;
        }

        private long GetSecondsSinceEpoch()
        {
            return (long)(clockService.UtcNow - UnixEpoch).TotalSeconds;
        }
    }
}
