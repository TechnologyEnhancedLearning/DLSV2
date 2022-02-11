namespace DigitalLearningSolutions.Data.Models.DelegateGroups
{
    public class GroupGenerationDetails
    {
        public GroupGenerationDetails(
            int adminId,
            int centreId,
            int registrationFieldOptionId,
            bool prefixGroupName,
            bool addExistingDelegates,
            bool addNewRegistrants,
            bool syncFieldChanges,
            bool skipDuplicateNames
        )
        {
            AdminId = adminId;
            CentreId = centreId;
            RegistrationFieldOptionId = registrationFieldOptionId;
            LinkedToField = GetLinkedToFieldValue(registrationFieldOptionId);
            PrefixGroupName = prefixGroupName;
            AddExistingDelegates = addExistingDelegates;
            AddNewRegistrants = addNewRegistrants;
            SyncFieldChanges = syncFieldChanges;
            SkipDuplicateNames = skipDuplicateNames;
        }

        public int AdminId { get; set; }
        public int CentreId { get; set; }
        public int RegistrationFieldOptionId { get; set; }
        public int LinkedToField { get; set; }
        public bool PrefixGroupName { get; set; }
        public bool AddExistingDelegates { get; set; }
        public bool AddNewRegistrants { get; set; }
        public bool SyncFieldChanges { get; set; }
        public bool SkipDuplicateNames { get; set; }

        private static int GetLinkedToFieldValue(int registrationFieldOptionId)
        {
            return registrationFieldOptionId switch
            {
                4 => 5,
                5 => 6,
                6 => 7,
                7 => 4,
                _ => registrationFieldOptionId,
            };
        }
    }
}
