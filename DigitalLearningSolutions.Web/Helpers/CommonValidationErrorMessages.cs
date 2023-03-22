namespace DigitalLearningSolutions.Web.Helpers
{
    public static class CommonValidationErrorMessages
    {
        public const string IncorrectPassword = "The password you have entered is incorrect";
        public const string TooLongFirstName = "First name must be 250 characters or fewer";
        public const string TooLongLastName = "Last name must be 250 characters or fewer";
        public const string TooLongEmail = "Email must be 255 characters or fewer";
        public const string InvalidEmail = "Enter an email in the correct format, like name@example.com";
        public const string WhitespaceInEmail = "Email must not contain any whitespace characters";
        public const string EmailsRegexWithNewLineSeparator = @"(([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)(\s*\r\n\s*|\s*$))*";
        public const string InvalidMultiLineEmail = "Enter an email in the correct format (like name@example.com). Note: Each email address should be on a separate line";
        public const string EmailInUse = "This email is already in use";
        public const string EmailInUseAtCentre = "This email is already in use by another user at the centre";

        public const string EmailInUseDuringDelegateRegistration =
            "A user with this email address is already registered; if this is you, please log in and register at this centre via the My Account page";

        public const string EmailInUseDuringAdminRegistration =
            "A user with this email address is already registered; if this is you, please log in using the button below";

        public const string WrongEmailForCentreDuringAdminRegistration =
            "This email address does not match the one held by the centre; either your primary email or centre email must match the one held by the centre";

        public const string PasswordRegex = @"(?=.*?[^\w\s])(?=.*?[0-9])(?=.*?[A-Z])(?=.*?[a-z]).*";
        public const string PasswordInvalidCharacters = "Password must contain at least 1 uppercase and 1 lowercase letter, 1 number and 1 symbol";
        public const string PasswordRequired = "Enter a password";
        public const string PasswordMinLength = "Password must be 8 characters or more";
        public const string PasswordMaxLength = "Password must be 100 characters or fewer";
        public const string StringMaxLengthValidation = "{0} must be {1} characters or fewer";
        public const string CenterEmailIsSameAsPrimary = "Centre email is the same as primary email";
        public const string PasswordTooCommon = "Enter a less common password";
        public const string PasswordSimilarUsername = "Enter a password which is less similar to your user name";
    }
}
