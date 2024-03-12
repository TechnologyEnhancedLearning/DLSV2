namespace DigitalLearningSolutions.Web.Tests.TestHelpers
{
    using DigitalLearningSolutions.Data.Models.Email;
    using MimeKit;

    public static class EmailTestHelper
    {
        public const string DefaultHtmlBody = "<body style= 'font-family: Calibri; font-size: small;'\r\n>" +
                                              "   <p>Test Body</p>\r\n" +
                                              "</body>";

        public static Email GetDefaultEmail(
            string[]? to = null,
            string[]? cc = null,
            string[]? bcc = null,
            string subject = "Test Subject Line",
            BodyBuilder? body = null
        )
        {
            return new Email(
                to: to ?? new string[1] { "recipient@example.com" },
                cc: cc ?? new string[1] { "cc@example.com" },
                bcc: bcc ?? new string[1] { "bcc@example.com" },
                subject: subject,
                body: body ?? new BodyBuilder
                {
                    TextBody = "Test body",
                    HtmlBody = DefaultHtmlBody
                }
            );
        }

        public static Email GetDefaultEmailToSingleRecipient(
            string to = "recipient@example.com",
            string subject = "Test Subject Line",
            BodyBuilder? body = null
        )
        {
            body ??= new BodyBuilder
            {
                TextBody = "Test body",
                HtmlBody = DefaultHtmlBody
            };
            return new Email(subject, body, to);
        }
    }
}
