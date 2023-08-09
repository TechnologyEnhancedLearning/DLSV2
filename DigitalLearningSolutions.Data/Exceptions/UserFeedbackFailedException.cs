namespace DigitalLearningSolutions.Data.Exceptions
{
    using System;

    public class UserFeedbackFailedException : Exception
    {
        public UserFeedbackFailedException(string message)
            : base(message)
        {
        }
    }
}
