
namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    public static class ErrorMessagesTestHelper
    {
        public const string InvalidFormatError =
            @"The format you entered isn’t recognised. Please check and try again.
            Valid formats include
            7 digits - example, 1234567
            1-2 letters followed by 6 digits - example, AB123456
            ‘P’ followed by 5-6 digits - example, P12345, P123456
            ‘C’ or ‘P’ followed by 6 digits - example, C123456, P123456
            Optional letter followed by 5-6 digits - example, A12345, B123456
            ‘L’ followed by 4-6 digits - example, L1234, L123456
            2 digits, hyphen, then 4-5 alphanumeric characters - example, 12-AB123";

        public const string MissingNumberError = "Enter a professional registration number";
        public const string LengthError = "Professional registration number must be between 4 and 8 characters";

    }
}
