namespace DigitalLearningSolutions.Data.Exceptions
{
    using System;

    public class DelegateUserNotApprovedException : Exception
    {
        public DelegateUserNotApprovedException(string message)
            : base(message) { }
    }
}
