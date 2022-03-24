namespace DigitalLearningSolutions.Data.Models
{
    using System;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public class SystemNotification : BaseSearchableItem
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

        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? SubjectLine;
            set => SearchableNameOverrideForFuzzySharp = value;
        }
    }
}
