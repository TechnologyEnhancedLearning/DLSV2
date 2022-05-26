namespace DigitalLearningSolutions.Web.Helpers
{
    public static class CommonValidationErrorMessages
    {
        public const string IncorrectPassword = "The password you have entered is incorrect";
        public const string TooLongFirstName = "First name must be 250 characters or fewer";
        public const string TooLongLastName = "Last name must be 250 characters or fewer";
        public const string TooLongAlias = "Alias must be 250 characters or fewer";
        public const string TooLongEmail = "Email address must be 255 characters or fewer";
        public const string InvalidEmail = "Enter an email address in the correct format, like name@example.com";
        public const string WhitespaceInEmail = "Email address must not contain any whitespace characters";
        public const string EmailAlreadyInUse = "This email address is already in use";

        public const string PasswordRegex = @"(?=.*?[^\w\s])(?=.*?[0-9])(?=.*?[A-Za-z]).*";
        public const string PasswordInvalidCharacters = "Password must contain at least 1 letter, 1 number and 1 symbol";
        public const string PasswordRequired = "Enter a password";
        public const string PasswordMinLength = "Password must be 8 characters or more";
        public const string PasswordMaxLength = "Password must be 100 characters or fewer";
        public const string StringMaxLengthValidation = "{0} must be {1} characters or fewer";
    }
}
