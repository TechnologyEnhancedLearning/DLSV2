namespace DigitalLearningSolutions.Data.Exceptions
{
    using System;

    public class LearningHubLinkingRequestException : Exception
    {
        public LearningHubLinkingRequestException(string message)
            : base(message) { }
    }
}
