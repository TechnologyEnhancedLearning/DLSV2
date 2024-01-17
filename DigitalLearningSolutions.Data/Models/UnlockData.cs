namespace DigitalLearningSolutions.Data.Models
{
    using System;

    public class UnlockData
    {
        public int DelegateId { get; set; }
        public string ContactForename { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
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
