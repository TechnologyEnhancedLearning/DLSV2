namespace DigitalLearningSolutions.Web.Helpers.FilterOptions
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.CourseDelegates;
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

    


}
