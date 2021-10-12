namespace DigitalLearningSolutions.Data.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    using DigitalLearningSolutions.Data.Enums;

    public class AdminCreationFailedException : Exception
    {
        public readonly AdminCreationError Error;

        public AdminCreationFailedException(AdminCreationError error)
        {
            Error = error;
        }

        protected AdminCreationFailedException(
            SerializationInfo info,
            StreamingContext context,
            AdminCreationError error
        ) : base(info, context)
        {
            Error = error;
        }

        public AdminCreationFailedException(string? message, AdminCreationError error) : base(message)
        {
            Error = error;
        }

        public AdminCreationFailedException(string? message, Exception? innerException, AdminCreationError error) :
            base(message, innerException)
        {
            Error = error;
        }
    }
}
