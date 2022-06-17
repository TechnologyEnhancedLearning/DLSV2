namespace DigitalLearningSolutions.Data.Enums
{
    using System;

    public class EmailVerificationReason : Enumeration
    {
        public static readonly EmailVerificationReason EmailNotVerified =
            new EmailVerificationReason(0, nameof(EmailNotVerified));

        public static readonly EmailVerificationReason EmailChanged =
            new EmailVerificationReason(1, nameof(EmailChanged));

        private EmailVerificationReason(int id, string name) : base(id, name) { }

        public static implicit operator EmailVerificationReason(string value)
        {
            try
            {
                return FromName<EmailVerificationReason>(value);
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidCastException(e.Message);
            }
        }
    }
}
