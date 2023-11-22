namespace DigitalLearningSolutions.Web.Helpers.FilterOptions
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public static class ActivityTypeFilterOptions
    {
        private const string Group = "Type";

        public static readonly FilterOptionModel IsCourse = new FilterOptionModel(
            "Course",
            FilteringHelper.BuildFilterValueString(Group, "Course", "true"),
            FilterStatus.Default
        );

        public static readonly FilterOptionModel IsSelfAssessment = new FilterOptionModel(
            "Self assessment",
            FilteringHelper.BuildFilterValueString(Group, "SelfAssessment", "true"),
            FilterStatus.Default
        );
    };
}
