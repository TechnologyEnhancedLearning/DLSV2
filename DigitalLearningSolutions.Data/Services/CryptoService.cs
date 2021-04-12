namespace DigitalLearningSolutions.Data.Services
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;

    public interface ICryptoService
    {
        public bool VerifyHashedPassword(string? hashedPassword, string password);
    }

    public class CryptoService : ICryptoService
    {
        private const int SaltSize = 16;
        private const int PBKDF2SubkeyLength = 32;
        private const int PBKDF2IterationCount = 1000;

        public bool VerifyHashedPassword(string? hashedPassword, string password)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword))
            {
                return false;
            }

            var hashedPasswordBytes = Convert.FromBase64String(hashedPassword);

            if (hashedPasswordBytes.Length != 1 + SaltSize + PBKDF2SubkeyLength || hashedPasswordBytes[0] != 0x00)
            {
                // Wrong length or version header
                return false;
            }

            var salt = new byte[SaltSize];
            Buffer.BlockCopy(hashedPasswordBytes, 1, salt, 0, SaltSize);

            var storedSubkey = new byte[PBKDF2SubkeyLength];
            Buffer.BlockCopy(hashedPasswordBytes, 1 + SaltSize, storedSubkey, 0, PBKDF2SubkeyLength);

            var deriveBytes = new Rfc2898DeriveBytes(password, salt, PBKDF2IterationCount);
            var generatedSubkey = deriveBytes.GetBytes(PBKDF2SubkeyLength);

            return generatedSubkey.SequenceEqual(storedSubkey);
        }
    }
}
