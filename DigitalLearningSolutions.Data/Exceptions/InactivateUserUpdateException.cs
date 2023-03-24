namespace DigitalLearningSolutions.Data.Exceptions
{
    using System;

    public class InactivateUserUpdateException : Exception
    {
        public InactivateUserUpdateException(string message)
            : base(message)
        {
        }
    }
}
