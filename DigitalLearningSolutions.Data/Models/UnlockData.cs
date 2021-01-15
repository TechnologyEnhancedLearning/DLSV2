namespace DigitalLearningSolutions.Data.Models
{
    using System;

    public class UnlockData
    {
        public string ContactForename { get; set; }
        public string ContactEmail { get; set; }
        public string DelegateName { get; set; }
        public string DelegateEmail { get; set; }
        public string CourseName { get; set; }
        public int CustomisationId { get; set; }
    }

    public class NotificationDataException : Exception
    {
        public NotificationDataException(string message)
            : base(message)
        {
        }
    }
}
