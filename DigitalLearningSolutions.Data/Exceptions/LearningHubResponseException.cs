namespace DigitalLearningSolutions.Data.Exceptions
{
    using System;
    using System.Net;

    public class LearningHubResponseException : Exception
    {
        public LearningHubResponseException(string message, HttpStatusCode httpStatusCode) : base(message)
        {
            HttpStatusCode = httpStatusCode;
        }

        public HttpStatusCode HttpStatusCode { get; set; }
    }
}
