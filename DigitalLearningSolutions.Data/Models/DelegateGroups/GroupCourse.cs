namespace DigitalLearningSolutions.Data.Models.DelegateGroups
{
    using System;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public class GroupCourse : BaseSearchableItem
    {
        public int GroupCustomisationId { get; set; }
        public int GroupId { get; set; }
        public int CustomisationId { get; set; }
        public int CourseCategoryId { get; set; }
        public string ApplicationName { get; set; }
        public string? CustomisationName { get; set; }
        public bool IsMandatory { get; set; }
        public bool IsAssessed { get; set; }
        public DateTime AddedToGroup { get; set; }
        public int CurrentVersion { get; set; }
        public int? SupervisorAdminId { get; set; }
        public string? SupervisorFirstName { get; set; }
        public string? SupervisorLastName { get; set; }
        public bool SupervisorAdminActive { get; set; }
        public int CompleteWithinMonths { get; set; }
        public int ValidityMonths { get; set; }
        public bool Active { get; set; }
        public DateTime? ApplicationArchivedDate { get; set; }
        public DateTime? InactivatedDate { get; set; }

        public bool IsUsable => Active && InactivatedDate == null
                                                 && ApplicationArchivedDate == null;

        public string CourseName => string.IsNullOrWhiteSpace(CustomisationName)
            ? ApplicationName
            : ApplicationName + " - " + CustomisationName;

        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? CourseName;
            set => SearchableNameOverrideForFuzzySharp = value;
        }
    }
}
