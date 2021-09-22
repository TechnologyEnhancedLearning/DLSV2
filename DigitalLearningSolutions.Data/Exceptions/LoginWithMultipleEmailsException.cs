namespace DigitalLearningSolutions.Data.Exceptions
{
    using System;

    public class LoginWithMultipleEmailsException : Exception
    {
        public LoginWithMultipleEmailsException(string message)
            : base(message) { }
    }
}
