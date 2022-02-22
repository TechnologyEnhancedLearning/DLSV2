namespace DigitalLearningSolutions.Data.Exceptions
{
    using System;

    public class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException(string message, bool apiIsAccessible) : base(
            message
        )
        {
            ApiIsAccessible = apiIsAccessible;
        }

        public bool ApiIsAccessible { get; set; }
    }
}
