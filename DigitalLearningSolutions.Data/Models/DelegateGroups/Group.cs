﻿namespace DigitalLearningSolutions.Data.Models.DelegateGroups
{
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public class Group : BaseSearchableItem
    {
        public int GroupId { get; set; }

        public string GroupLabel { get; set; } = string.Empty;

        public string? GroupDescription { get; set; }

        public int DelegateCount { get; set; }

        public int CoursesCount { get; set; }

        public int AddedByAdminId { get; set; }

        public string AddedByFirstName { get; set; } = string.Empty;

        public string AddedByLastName { get; set; } = string.Empty;

        public bool AddedByAdminActive { get; set; }

        public int LinkedToField { get; set; }

        public string LinkedToFieldName { get; set; } = string.Empty;

        public bool ShouldAddNewRegistrantsToGroup { get; set; }

        public bool ChangesToRegistrationDetailsShouldChangeGroupMembership { get; set; }

        public override string SearchableName
        {
            get => SearchableNameOverrideForFuzzySharp ?? GroupLabel;
            set => SearchableNameOverrideForFuzzySharp = value;
        }
    }
}
