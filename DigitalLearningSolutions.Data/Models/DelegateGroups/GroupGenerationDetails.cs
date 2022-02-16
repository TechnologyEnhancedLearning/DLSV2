namespace DigitalLearningSolutions.Data.Models.DelegateGroups
{
    public class GroupGenerationDetails
    {
        public GroupGenerationDetails(
            int adminId,
            int centreId,
            int linkedToField,
            bool prefixGroupName,
            bool populateExisting,
            bool addNewRegistrants,
            bool syncFieldChanges,
            bool skipDuplicateNames
        )
        {
            AdminId = adminId;
            CentreId = centreId;
            LinkedToField = linkedToField;
            PrefixGroupName = prefixGroupName;
            PopulateExisting = populateExisting;
            AddNewRegistrants = addNewRegistrants;
            SyncFieldChanges = syncFieldChanges;
            SkipDuplicateNames = skipDuplicateNames;
        }

        public int AdminId { get; set; }
        public int CentreId { get; set; }
        public int LinkedToField { get; set; }
        public bool PrefixGroupName { get; set; }
        public bool PopulateExisting { get; set; }
        public bool AddNewRegistrants { get; set; }
        public bool SyncFieldChanges { get; set; }
        public bool SkipDuplicateNames { get; set; }
    }
}
