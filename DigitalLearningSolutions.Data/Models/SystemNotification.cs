namespace DigitalLearningSolutions.Data.Models
{
    using System;

    public class SystemNotification
    {
        public SystemNotification(
            int systemNotificationId,
            string subjectLine,
            string bodyHtml,
            DateTime? expiryDate,
            DateTime dateAdded,
            int targetUserRoleId
        )
        {
            SystemNotificationId = systemNotificationId;
            SubjectLine = subjectLine;
            BodyHtml = bodyHtml;
            ExpiryDate = expiryDate;
            DateAdded = dateAdded;
            TargetUserRoleId = targetUserRoleId;
        }

        public int SystemNotificationId { get; set; }

        public string SubjectLine { get; set; }

        public string BodyHtml { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public DateTime DateAdded { get; set; }

        public int TargetUserRoleId { get; set; }
    }
}
