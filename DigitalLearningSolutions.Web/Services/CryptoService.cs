namespace DigitalLearningSolutions.Web.Services
{
    using System;
    using System.Linq;
    using System.Security.Cryptography;

    public interface ICryptoService
    {
        bool VerifyHashedPassword(string? hashedPassword, string password);
        string GetPasswordHash(string password);
    }

    public class CryptoService : ICryptoService
    {
        private const int SaltSize = 16;
        private const int PBKDF2SubkeyLength = 32;
        private const int PBKDF2IterationCount = 1000;
        private const int CorrectStoredPasswordLength = 1 + SaltSize + PBKDF2SubkeyLength;
        private const byte CorrectStoredPasswordVersionHeader = 0x00;

        public bool VerifyHashedPassword(string? hashedPassword, string password)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword))
            {
                return false;
            }

            byte[] hashedPasswordBytes = Convert.FromBase64String(hashedPassword);

            bool storedPasswordIsIncorrectLength = hashedPasswordBytes.Length != CorrectStoredPasswordLength;
            bool storedPasswordHasIncorrectVersionHeader =
                hashedPasswordBytes[0] != CorrectStoredPasswordVersionHeader;

            if (storedPasswordIsIncorrectLength || storedPasswordHasIncorrectVersionHeader)
            {
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

        public string GetPasswordHash(string password)
        {
            var salt = new byte[SaltSize];
            new RNGCryptoServiceProvider().GetBytes(salt);

            var deriveBytes = new Rfc2898DeriveBytes(password, salt, PBKDF2IterationCount);
            var generatedSubkey = deriveBytes.GetBytes(PBKDF2SubkeyLength);

            var hashBytes = new byte[CorrectStoredPasswordLength];
            hashBytes[0] = CorrectStoredPasswordVersionHeader;
            Buffer.BlockCopy(salt, 0, hashBytes, 1, SaltSize);
            Buffer.BlockCopy(generatedSubkey, 0, hashBytes, 1 + SaltSize, PBKDF2SubkeyLength);

            return Convert.ToBase64String(hashBytes);
        }
    }
}
