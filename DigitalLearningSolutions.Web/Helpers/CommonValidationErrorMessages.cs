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
    }
}
