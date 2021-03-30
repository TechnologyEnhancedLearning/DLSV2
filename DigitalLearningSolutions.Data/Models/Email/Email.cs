namespace DigitalLearningSolutions.Data.Models.Email
{
    using MimeKit;

    public class Email
    {
        public BodyBuilder Body;
        public string Subject;
        public string[] To, Cc, Bcc;

        public Email(string subject, BodyBuilder body, string to, string? cc = null, string? bcc = null)
        {
            Subject = subject;
            Body = body;
            To = new string[1] { to };
            Cc = cc is null ? new string[0] : new string[1] { cc };
            Bcc = bcc is null ? new string[0] : new string[1] { bcc };
        }

        public Email(string subject, BodyBuilder body, string[] to, string[]? cc = null, string[]? bcc = null)
        {
            Subject = subject;
            Body = body;
            To = to;
            Cc = cc ?? new string[0];
            Bcc = bcc ?? new string[0];
        }
    }
}
