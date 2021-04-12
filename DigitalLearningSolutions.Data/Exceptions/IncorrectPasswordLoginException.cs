namespace DigitalLearningSolutions.Data.Exceptions
{
    using System;

    public class IncorrectPasswordLoginException : Exception
    {
        public IncorrectPasswordLoginException(string message)
            : base(message) { }
    }
}
