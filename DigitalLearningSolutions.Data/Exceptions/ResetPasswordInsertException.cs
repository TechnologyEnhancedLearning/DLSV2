namespace DigitalLearningSolutions.Data.Exceptions
{
    using System;

    public class ResetPasswordInsertException : Exception
    {
        public ResetPasswordInsertException(string message)
            : base(message)
        {
        }
    }
}
