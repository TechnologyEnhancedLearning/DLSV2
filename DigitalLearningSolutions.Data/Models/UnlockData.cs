namespace DigitalLearningSolutions.Data.Models
{
    using System;

    public class UnlockData
    {
        public int DelegateId { get; set; }
        public string ContactForename { get; set; }
        public string ContactEmail { get; set; }
        public string CourseName { get; set; }
        public int CustomisationId { get; set; }
        public string CentreName { get; set; }
    }

    public class NotificationDataException : Exception
    {
        public NotificationDataException(string message)
            : base(message)
        {
        }
    }
}
