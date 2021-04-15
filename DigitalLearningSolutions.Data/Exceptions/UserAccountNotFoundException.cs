namespace DigitalLearningSolutions.Data.Exceptions
{
    using System;

    public class UserAccountNotFoundException : Exception
    {
        public UserAccountNotFoundException(string message)
            : base(message) { }
    }
}
