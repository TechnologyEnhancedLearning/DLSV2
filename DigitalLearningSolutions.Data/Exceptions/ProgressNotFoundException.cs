namespace DigitalLearningSolutions.Data.Exceptions
{
    using System;

    public class ProgressNotFoundException : Exception
    {
        public ProgressNotFoundException(string message)
            : base(message) { }
    }
}
