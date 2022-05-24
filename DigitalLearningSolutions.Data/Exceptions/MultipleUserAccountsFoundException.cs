namespace DigitalLearningSolutions.Data.Exceptions
{
    using System;

    public class MultipleUserAccountsFoundException : Exception
    {
        public MultipleUserAccountsFoundException(string message)
            : base(message) { }
    }
}
