namespace DigitalLearningSolutions.Web.ViewModels.TrackingSystem.Delegates.DelegateGroups
{
    using DigitalLearningSolutions.Data.Models.DelegateGroups;
    using DigitalLearningSolutions.Web.Helpers;

    public class SearchableDelegateGroupViewModel
    {
        private const string Yes = "Yes";
        private const string No = "No";

        public SearchableDelegateGroupViewModel(Group group, int? page)
        {
            Id = group.GroupId;
            Name = group.GroupLabel;
            Description = group.GroupDescription;
            DelegateCount = group.DelegateCount;
            CourseCount = group.CoursesCount;
            LinkedToField = group.LinkedToField;
            LinkedField = group.LinkedToFieldName;
            AddedByAdminId = group.AddedByAdminId;
            AddedBy = group.AddedByName;
            ShouldAddNewRegistrantsToGroup = group.ShouldAddNewRegistrantsToGroup ? Yes : No;
            ChangesToRegistrationDetailsShouldChangeGroupMembership =
                group.ChangesToRegistrationDetailsShouldChangeGroupMembership ? Yes : No;
            Page = page;
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public int DelegateCount { get; set; }

        public int CourseCount { get; set; }

        public int LinkedToField { get; set; }

        public string LinkedField { get; set; }

        public int AddedByAdminId { get; set; }

        public string AddedBy { get; set; }

        public string AddedByFilter => nameof(Group.AddedByAdminId) + FilteringHelper.Separator +
                                       nameof(Group.AddedByAdminId) +
                                       FilteringHelper.Separator + AddedByAdminId;

        public string LinkedFieldFilter => nameof(Group.LinkedToField) + FilteringHelper.Separator +
                                           nameof(Group.LinkedToField) +
                                           FilteringHelper.Separator + LinkedToField;

        public string ShouldAddNewRegistrantsToGroup { get; set; }

        public string ChangesToRegistrationDetailsShouldChangeGroupMembership { get; set; }

        public int? Page { get; set; }
    }
}
