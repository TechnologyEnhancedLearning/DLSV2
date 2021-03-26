using System;
using System.Collections.Generic;
using System.Text;

namespace DigitalLearningSolutions.Data.Models.Email
{
    using MimeKit;

    public class Email
    {
        public string[] To, Cc, Bcc;
        public string Subject;
        public BodyBuilder Body;

        public Email()
        {
            this.To = new string[0];
            this.Cc = new string[0];
            this.Bcc = new string[0];
            this.Subject = string.Empty;
            this.Body = new BodyBuilder();
        }

        public Email(string? to, string? subject, BodyBuilder? body, string? cc = null, string? bcc = null)
        {
            this.To = (to is null) ? new string[0] : new string[1] {to};
            this.Cc = (cc is null) ? new string[0] : new string[1] {cc};
            this.Bcc = (bcc is null) ? new string[0] : new string[1] {bcc};
            this.Subject = subject ?? string.Empty;
            this.Body = body ?? new BodyBuilder();
        }

        public Email(string[]? to, string? subject, BodyBuilder? body, string[]? cc = null, string[]? bcc = null)
        {
            this.To = to ?? new string[0];
            this.Cc = cc ?? new string[0];
            this.Bcc = bcc ?? new string[0];
            this.Subject = subject ?? string.Empty;
            this.Body = body ?? new BodyBuilder();
        }
    }
}
