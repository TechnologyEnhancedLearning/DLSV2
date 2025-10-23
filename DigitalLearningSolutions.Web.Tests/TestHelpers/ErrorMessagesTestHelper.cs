
namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    public static class ErrorMessagesTestHelper
    {
        public const string InvalidFormatError =
           "Invalid professional registration number format. " +
           "Valid formats include: 7 digits (e.g., 1234567), 1–2 letters followed by 6 digits (e.g., AB123456), " +
           "4–8 digits, an optional 'P' plus 5–6 digits, 'C' or 'P' plus 6 digits, " +
           "an optional letter plus 5–6 digits, 'L' plus 4–6 digits, " +
           "or 2 digits followed by a hyphen and 4–5 alphanumeric characters (e.g., 12-AB123).";

        public const string MissingNumberError = "Enter a professional registration number";
        public const string LengthError = "Professional registration number must be between 5 and 20 characters";

    }
}
