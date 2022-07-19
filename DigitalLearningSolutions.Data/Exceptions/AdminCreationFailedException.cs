namespace DigitalLearningSolutions.Data.Exceptions
{
    using System;

    public class AdminCreationFailedException : Exception
    {
        public AdminCreationFailedException(string message) : base(message) { }

        public AdminCreationFailedException() { }
    }
}
