namespace DigitalLearningSolutions.Data.Models.DelegateGroups
{
    using System;

    public class GroupDetails
    {
        public int CentreId { get; set; }
        public string GroupLabel { get; set; }
        public string? GroupDescription { get; set; }
        public int LinkedToField { get; set; }
        public bool SyncFieldChanges { get; set; }
        public bool AddNewRegistrants { get; set; }
        public bool PopulateExisting { get; set; }
        public int AdminUserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
