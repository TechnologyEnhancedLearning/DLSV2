namespace DigitalLearningSolutions.Data.Exceptions
{
    using System;

    class UserAccountInvalidStateException : Exception
    {
        public UserAccountInvalidStateException(string message)
            : base(message) { }
    }
}
