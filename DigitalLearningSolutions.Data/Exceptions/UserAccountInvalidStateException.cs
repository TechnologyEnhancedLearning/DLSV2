namespace DigitalLearningSolutions.Data.Exceptions
{
    using System;

    public class UserAccountInvalidStateException : Exception
    {
        public UserAccountInvalidStateException(string message)
            : base(message) { }
    }
}
