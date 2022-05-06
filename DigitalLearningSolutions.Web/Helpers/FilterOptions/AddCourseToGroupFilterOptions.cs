namespace DigitalLearningSolutions.Web.Helpers.FilterOptions
{
    using DigitalLearningSolutions.Data.Enums;
    using DigitalLearningSolutions.Data.Helpers;
    using DigitalLearningSolutions.Data.Models.Courses;
    using DigitalLearningSolutions.Data.Models.SearchSortFilterPaginate;

    public static class AddCourseToGroupAssessedFilterOptions
    {
        private const string Group = "Assessed";

        public static readonly FilterOptionModel IsAssessed = new FilterOptionModel(
            "Assessed",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseAssessmentDetails.IsAssessed), "true"),
            FilterStatus.Default
        );

        public static readonly FilterOptionModel IsNotAssessed = new FilterOptionModel(
            "Not assessed",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseAssessmentDetails.IsAssessed), "false"),
            FilterStatus.Default
        );
    }

    public static class AddCourseToGroupLearningFilterOptions
    {
        private const string Group = "Learning";

        public static readonly FilterOptionModel HasLearning = new FilterOptionModel(
            "Has learning",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseAssessmentDetails.HasLearning), "true"),
            FilterStatus.Default
        );

        public static readonly FilterOptionModel NoLearning = new FilterOptionModel(
            "No learning",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseAssessmentDetails.HasLearning), "false"),
            FilterStatus.Default
        );
    }

    public static class AddCourseToGroupDiagnosticFilterOptions
    {
        private const string Group = "Diagnostic";

        public static readonly FilterOptionModel HasDiagnostic = new FilterOptionModel(
            "Has diagnostic",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseAssessmentDetails.HasDiagnostic), "true"),
            FilterStatus.Default
        );

        public static readonly FilterOptionModel NoDiagnostic = new FilterOptionModel(
            "No diagnostic",
            FilteringHelper.BuildFilterValueString(Group, nameof(CourseAssessmentDetails.HasDiagnostic), "false"),
            FilterStatus.Default
        );
    }
}
