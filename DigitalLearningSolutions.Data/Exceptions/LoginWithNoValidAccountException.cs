namespace DigitalLearningSolutions.Data.Exceptions
{
    using System;

    public class LoginWithNoValidAccountException : Exception
    {
        public LoginWithNoValidAccountException(string message)
            : base(message) { }
    }
}
