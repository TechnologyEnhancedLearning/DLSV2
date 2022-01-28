namespace DigitalLearningSolutions.Data.Exceptions
{
    using System;

    public class ConfigValueMissingException : Exception
    {
        public ConfigValueMissingException(string message) : base(message) { }
    }
}
