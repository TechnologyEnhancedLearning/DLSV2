namespace DigitalLearningSolutions.Data.Exceptions
{
    using System;
    using System.Runtime.Serialization;
    using DigitalLearningSolutions.Data.Enums;

    public class DelegateCreationFailedException : Exception
    {
        public readonly DelegateCreationError Error;

        public DelegateCreationFailedException(DelegateCreationError error)
        {
            Error = error;
        }

        protected DelegateCreationFailedException(
            SerializationInfo info,
            StreamingContext context,
            DelegateCreationError error
        ) : base(info, context)
        {
            Error = error;
        }

        public DelegateCreationFailedException(string? message, DelegateCreationError error) : base(message)
        {
            Error = error;
        }

        public DelegateCreationFailedException(string? message, Exception? innerException, DelegateCreationError error) :
            base(message, innerException)
        {
            Error = error;
        }
    }
}
