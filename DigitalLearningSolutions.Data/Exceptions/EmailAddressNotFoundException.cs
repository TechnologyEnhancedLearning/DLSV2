namespace DigitalLearningSolutions.Data.Exceptions
{
    using System;

    public class EmailAddressNotFoundException : Exception
    {
        public EmailAddressNotFoundException(string message)
            : base(message)
        {
        }
    }
}
