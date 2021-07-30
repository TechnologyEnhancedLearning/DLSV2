namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using DigitalLearningSolutions.Data.Models.DelegateGroups;

    public class SearchableDelegateGroupViewModel
    {
        private const string Yes = "Yes";
        private const string No = "No";

        public SearchableDelegateGroupViewModel(Group group)
        {
            Id = group.GroupId;
            Name = group.GroupLabel;
            Description = group.GroupDescription;
            DelegateCount = group.DelegateCount;
            CourseCount = group.CoursesCount;
            LinkedToField = group.LinkedToField;
            LinkedField = group.LinkedToFieldName;
            AddedBy = $"{group.AddedByFirstName} {group.AddedByLastName}";
            AddNewRegistrantsToGroup = group.AddNewRegistrantsToGroup ? Yes : No;
            SyncChangesToRegistrationInfoWithGroupMembership = group.SyncChangesToRegistrationInfoWithGroupMembership ? Yes : No;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public int DelegateCount { get; set; }

        public int CourseCount { get; set; }

        public int LinkedToField { get; set; }

        public string LinkedField { get; set; }

        public string AddedBy { get; set; }

        public string AddNewRegistrantsToGroup { get; set; }

        public string SyncChangesToRegistrationInfoWithGroupMembership { get; set; }
    }
}
