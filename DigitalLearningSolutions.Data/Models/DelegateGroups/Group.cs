namespace DigitalLearningSolutions.Data.Models.DelegateGroups
{
    public class Group
    {
        public int GroupId { get; set; }

        public string GroupLabel { get; set; }

        public string? GroupDescription { get; set; }

        public int DelegateCount { get; set; }

        public int CoursesCount { get; set; }

        public string AddedByFirstName { get; set; }

        public string AddedByLastName { get; set; }

        public int LinkedToField { get; set; }

        public string LinkedToFieldName { get; set; }

        public bool AddNewRegistrantsToGroup { get; set; }

        public bool SyncChangesToRegistrationInfoWithGroupMembership { get; set; }
    }
}
