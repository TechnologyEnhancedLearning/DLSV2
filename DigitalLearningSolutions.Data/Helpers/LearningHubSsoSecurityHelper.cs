namespace DigitalLearningSolutions.Data.Helpers
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using DigitalLearningSolutions.Data.Services;

    public class LearningHubSsoSecurityHelper
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
            var pwd = encoder.GetBytes(state + secondsSinceEpoch);
            return GetHash(pwd, salt);
        }

        public bool VerifyHash(string state, string secretKey, string hash)
        {
            var secondsSinceEpoch = GetSecondsSinceEpoch();
            var encoder = new UTF8Encoding();
            var salt = encoder.GetBytes(secretKey);
            var toleranceInSec = 60;

            // loop optimisation, iterates 0,-1,-2,-3 .. -60,1,2,3 .. 60
            for (var counter = 0; counter <= toleranceInSec * 2; counter++)
            {
                var step = counter > toleranceInSec ? counter - toleranceInSec : -1 * counter;
                var pwd = encoder.GetBytes(state + (secondsSinceEpoch + step));

                if (hash == GetHash(pwd, salt))
                {
                    return true;
                }
            }

            return false;
        }

        private static string GetHash(byte[] password, byte[] salt)
        {
            using var byteResult = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA512);
            var hash = Convert.ToBase64String(byteResult.GetBytes(32));
            return hash;
        }

        private long GetSecondsSinceEpoch()
        {
            return (long)(clockService.UtcNow - UnixEpoch).TotalSeconds;
        }
    }
}
