namespace DigitalLearningSolutions.Web.Helpers.FilterOptions
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;
    using DigitalLearningSolutions.Data.Models.SelfAssessments;

    public static class SelfAssessmentDelegateAccountStatusFilterOptions
    {
        private const string Group = "AccountStatus";

        public static readonly FilterOptionModel Active = new FilterOptionModel(
            "Active",
            FilteringHelper.BuildFilterValueString(Group, nameof(SelfAssessmentDelegate.IsDelegateActive), "true"),
            FilterStatus.Success
        );

        public static readonly FilterOptionModel Inactive = new FilterOptionModel(
            "Inactive",
            FilteringHelper.BuildFilterValueString(Group, nameof(SelfAssessmentDelegate.IsDelegateActive), "false"),
            FilterStatus.Warning
        );
    }

    public static class SelfAssessmentDelegateRemovedFilterOptions
    {
        private const string Group = "AssessmentRemoved";

        public static readonly FilterOptionModel Removed = new FilterOptionModel(
            "Removed",
            FilteringHelper.BuildFilterValueString(Group, nameof(SelfAssessmentDelegate.Removed), "true"),
            FilterStatus.Warning
        );

        public static readonly FilterOptionModel NotRemoved = new FilterOptionModel(
            "Not removed",
            FilteringHelper.BuildFilterValueString(Group, nameof(SelfAssessmentDelegate.Removed), "false"),
            FilterStatus.Default
        );
    }

    public static class SelfAssessmentAssessmentSubmittedFilterOptions
    {
        private const string Group = "AssessmentSubmitted";

        public static readonly FilterOptionModel Submitted = new FilterOptionModel(
            "Submitted",
            FilteringHelper.BuildFilterValueString(Group, nameof(SelfAssessmentDelegate.SubmittedDate), "true"),
            FilterStatus.Warning
        );

        public static readonly FilterOptionModel NotSubmitted = new FilterOptionModel(
            "Not submitted",
            FilteringHelper.BuildFilterValueString(Group, nameof(SelfAssessmentDelegate.SubmittedDate), "false"),
            FilterStatus.Default
        );
    }

    public static class SelfAssessmentSignedOffFilterOptions
    {
        private const string Group = "AssessmentSignedOff";

        public static readonly FilterOptionModel SignedOff = new FilterOptionModel(
            "Signed off",
            FilteringHelper.BuildFilterValueString(Group, nameof(SelfAssessmentDelegate.SignedOff), "true"),
            FilterStatus.Warning
        );

        public static readonly FilterOptionModel NotSignedOff = new FilterOptionModel(
            "Not signed off",
            FilteringHelper.BuildFilterValueString(Group, nameof(SelfAssessmentDelegate.SignedOff), "false"),
            FilterStatus.Default
        );
    }

}
