namespace DigitalLearningSolutions.Web.Attributes
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class CommonPasswordsAttribute : ValidationAttribute
    {
        private readonly string? errorMessage;

        private static readonly HashSet<string> Passwords;
        public CommonPasswordsAttribute(string? errorMessage = null)
        {
            this.errorMessage = errorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || (string)value == string.Empty)
            {
                return ValidationResult.Success;
            }
            string lowerCasePassword = value.ToString().ToLower();
            foreach (var commonPassword in CommonPasswords.passwords)
            {
                if (lowerCasePassword.Contains(commonPassword))
                {
                    return new ValidationResult(this.errorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }

    public static class CommonPasswords
    {
        public static readonly HashSet<string> passwords = new HashSet<string>();

        static CommonPasswords()
        {
            // Password list taken from here https://github.com/danielmiessler/SecLists/blob/master/Passwords/Common-Credentials/top-passwords-shortlist.txt
            passwords.Add("password");
            passwords.Add("123456");
            passwords.Add("12345678");
            passwords.Add("abc123");
            passwords.Add("querty");
            passwords.Add("monkey");
            passwords.Add("letmein");
            passwords.Add("dragon");
            passwords.Add("111111");
            passwords.Add("baseball");
            passwords.Add("iloveyou");
            passwords.Add("trustno1");
            passwords.Add("1234567");
            passwords.Add("sunshine");
            passwords.Add("master");
            passwords.Add("123123");
            passwords.Add("welcome");
            passwords.Add("shadow");
            passwords.Add("ashley");
            passwords.Add("footbal");
            passwords.Add("jesus");
            passwords.Add("michael");
            passwords.Add("ninja");
            passwords.Add("mustang");
            passwords.Add("password1");
        }
    }
}
